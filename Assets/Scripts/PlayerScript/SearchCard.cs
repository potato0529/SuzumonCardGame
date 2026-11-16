using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Cysharp.Threading.Tasks;

public class SearchCard : MonoBehaviour,IOwnerPlayer
{
    [Header("持ち主")] public Player pScr;
    [Header("1番目のカードの場所")] public GameObject firstCardPos;
    [Header("ソーシャルディスタンス")] public float cardsDistance;
    [Header("サーチウィンドウカードジェネレーター")] public SearchWindowSlotGenerator searchWindowSlotGenerator;
    [Header("コントロールボタン")] public SearchWindowButton controlButton;//ウィンドウを閉じたり開いたりするボタン
    [Header("サーチガイド")] public TMP_Text guideText;
    private string effectText;

    [HideInInspector] public List<Card> inSearchWindowCards = new List<Card>(); //条件に当てはまったゲームカード
    [HideInInspector] public List<SearchWindowCard> inSearchWindowSCards = new List<SearchWindowCard>(); //サーチウィンドウ内にあるSearchWindowCard。af:inSearchWindowCardsと内容がかぶり過ぎてるから改善します
    private float scale; //大きさではなく差の倍率

    //Search用
    private List<SearchWindowCard> selectedCards = new List<SearchWindowCard>();

    private int needTargetsNum;

    public int searchingId = 0;

    #region EffectSummon用
    private Action<List<Card>> selectedCardsCallback; //表記はselectedCardsだがサーチだけでなく使用されたカードも対応
    private int remainingSummonNum;
    private List<Card> summonedCards = new List<Card>();
    public bool isSummonMode = false; //効果による使用、スペシャルデッキなどその他の使用含め、サーチウィンドウから使用する時全てに対応する
    #endregion
    public bool hidden;
    private Vector2 originPos;

    public enum SearchMode
    {
        none,
        EffectOrSelectProcess,//効果かゲーム上の選択処理
        DirectSpecialDeck//スペシャルデッキをクリックして直接展開する時
    }

    public SearchMode searchMode = SearchMode.none;

    void Awake()
    {
        originPos = GetComponent<RectTransform>().anchoredPosition;

        if(pScr == MatchManager.instance.players[0])
        {
            scale = 1;
        }
        else if(pScr == MatchManager.instance.players[1])
        {
            scale = -1;
        }
    }

    /// <summary>
    /// サーチウィンドウを展開し、引数のリストから条件に当てはまるカードを表示する ※サーチウィンドウを使う処理では最初に必ず通る
    /// </summary>
    /// <param name="userObj"></param>
    /// <param name="searchCards"></param>
    /// <param name="thisConditionsNugget"></param>
    /// <param name="topNum"></param>
    public void Search(List<Card> cards,int thisNeedTargetsNum,string text)
    {
        needTargetsNum = thisNeedTargetsNum;
        
        ArrangeCards(cards);
        RemainCardCheck();
        effectText = text;
        UpdateGuide();

        pScr.playerCanvas.sideSupportGuide.SetGuideText("ウィンドウ内から<b>効果の対象とするカードをクリック</b>して選ぼう。選択済みのカードにはチェックがつくぞ。");

        //通信対戦などの時に、ローカル側と通信側で同じサーチ処理を対応できるようにするIDの値を1増やす（毎回1増やせば同じ番号で異なる処理というのはなくなる）
        searchingId++;
    }

    /// <summary>
    /// 効果のサーチ処理そのものは行わないが、サーチウィンドウを展開しカードを並べる
    /// </summary>
    /// <param name="cards"></param>
    public void ArrangeCards(List<Card> cards) 
    {
        //na:今までは inSearchWindowCards = cards; という形で.ToList()を使っていなかったが、ToList()に変更した。流石にこれに変更して問題ないどころか前の状態の方が問題置きそうだが、流石に十分な検討なしで変更するのは不安なのでなんか問題があったら戻して
        inSearchWindowCards = cards.ToList();
        inSearchWindowCards = SuzumonTools.SortCard<Card>(inSearchWindowCards);

        inSearchWindowSCards = searchWindowSlotGenerator.EnumerateCards(inSearchWindowCards);
        controlButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// サーチウィンドウのカードが選択される(召喚モードの時は呼ばれない）
    /// </summary>
    /// <param name="searchWindowCard"></param>
    public void SelectedCard(SearchWindowCard searchWindowCard)
    {
        selectedCards.Add(searchWindowCard);
        if(selectedCards.Count == needTargetsNum)
        {
            foreach (SearchWindowCard selectedCard in selectedCards)
                pScr.cardPointer.SelectedCard(selectedCard.sourceCard);

            pScr.cardPointer.SelectedOneTargetCP();//warn:SelectedCard()内のSelectedOneTargetCP()がコメントアウトされている部分のコメント参照
        }
        else
        {
            UpdateGuide();
            RemainCardCheck();
        }
    }

    /// <summary>
    /// サーチウィンドウ展開時とカード選択時に残っているカードが選択可能か確認し、不可能であればサーチを終了する
    /// </summary>
    public void RemainCardCheck()
    {
        //効果召喚の際は最初しかRemainCardCheck呼ばれないから実質問題ないけどちょっと怪しくね？何も出せないのに効果召喚始まることはないから理論上大丈夫だけど構造的に怪しい
        if (inSearchWindowCards.Count > 0) return;

        //サーチ終了処理
        Debug.Log("サーチ終了処理をします");
        foreach (SearchWindowCard selectedCard in selectedCards)
            pScr.cardPointer.SelectedCard(selectedCard.sourceCard);

        pScr.cardPointer.SelectedOneTargetCP();//warn:SelectedCard()内のSelectedOneTargetCP()がコメントアウトされている部分のコメント参照
        pScr.cardPointer.DecidedTargetsCP();
    }

    #region サーチウィンドウ使用の関連メソッド
    /// <summary>
    /// サーチウィンドウを利用する召喚の共通処理
    /// </summary>
    /// <param name="callback"></param>
    public void StartSummon(Action<List<Card>> callback)
    {
        selectedCardsCallback = callback;
        isSummonMode = true;
    }

    /// <summary>
    /// サーチウィンドウを展開しつつ、サーチウィンドウからの召喚に対応する状態になる
    /// </summary>
    /// <param name="userObj"></param>
    /// <param name="thisConditionsNugget"></param>
    /// <param name="topNum"></param>
    /// <param name="effectNugget">召喚効果が完了したときに完了した合図を送るため</param>
    /// <param name="summonNum"></param>
    public void EffectSummon(List<Card> cards, Action<List<Card>> callback,int summonNum,string text)
    {
        searchMode = SearchMode.EffectOrSelectProcess;
        remainingSummonNum = summonNum;
        StartSummon(callback);

        Search(cards, summonNum,text);

        OnSummonAvailable().Forget();//上のSearch()でinSearchWindowCardsが既にセットされていなければ呼んではならない
    }

    /// <summary>
    /// スペシャルデッキから通常使用する
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="callback"></param>
    public void SpecialDeckSummon(List<Card> cards,Action<List<Card>> callback)
    {
        searchMode = SearchMode.DirectSpecialDeck;
        StartSummon(callback);
        ArrangeCards(cards);
    }

    /// <summary>
    /// 効果召喚で一体召喚するごとに呼ばれる。召喚されたときにサーチウィンドウ側がすべき処理をする。
    /// </summary>
    /// <param name="summonedCard"></param>
    public void Summoned(Card summonedCard)
    {
        UtilityLog.CardLog("効果で1体召喚しました", summonedCard, UtilityLog.LogCategory.none);
        remainingSummonNum--;
        summonedCards.Add(summonedCard);
        inSearchWindowCards.Remove(summonedCard);//ref1
        //少し無理に結び付けてる可能性があるから周辺の処理を改良する場合要注意
        EffectManager.instance.ability.targetedCards.Add(summonedCard);
        if (remainingSummonNum <= 0 || inSearchWindowCards.Count <= 0)
        {
            EndSearch();
            pScr.cardPointer.searchWindowMode = false;
        }
        else
        {
            //inSearchWindowCards.Remove(summonedCard);ref1:タイミングが合理的じゃないと考えて移動したが、この影響でバグが生じる危険性有
            ReOpenWindow();
            UpdateGuide();

            OnSummonAvailable().Forget();
        }
        //Debug.Log(inSearchWindowCards.Count);

        searchWindowSlotGenerator.RemoveSearchWindowCard(summonedCard);
    }
    #endregion

    /// <summary>
    /// サーチウィンドウを閉じ、ウィンドウにあったカードの状態をリセットする　※注意　Effectでカードの状態を変更する場合変更する処理の前に行う
    /// </summary>
    public void EndSearch()
    {
        Debug.Log("サーチプロセスが終了しました");
        //foreach(Card card in cards)
        //{
        //    card.inSearchWindow = false;

        //    card.SetCardRotation(new Vector3(90,0,0));

        //    switch (card.cardState)
        //    {
        //        case Card.CardState.Deck:
        //            pScr.deck.ArrangementDeck();
        //            break;

        //        case Card.CardState.Cemetery:
        //            pScr.cemeteryScr.ArrangementCemetery();
        //            break;

        //        case Card.CardState.Hand:
        //            pScr.handScr.SortHands();
        //            break;
        //    }
        //}
        //もしかしたらここらへんの処理はCardSlotGenerator側に任せた方がよい可能性
        foreach(SearchWindowCard searchWindowCard in searchWindowSlotGenerator.searchWindowCards)
        {
            Destroy(searchWindowCard.gameObject);
        }
        searchWindowSlotGenerator.generatedListedUICards = new List<ListedUICard>();


        if (isSummonMode)
        {
            selectedCardsCallback(summonedCards);
            summonedCards = new List<Card>();
        }
        //(callback.GetComponent(typeof(IENVariable)) as IENVariable).CompUsedEffect();//もしかしたらEffect限定じゃなくしないといけない可能性 とりあえずIENで妥協するけどあんま信頼できない

        inSearchWindowCards = new List<Card>();//Clear()はびみょいから変えたけど問題があったら戻そう
        needTargetsNum = 0;
        selectedCards = new List<SearchWindowCard>();//Clear()はびみょいから変えたけど問題があったら戻そう
        isSummonMode = false;
        controlButton.CloseButton();
        effectText = "";

        searchMode = SearchMode.none;
        GetComponent<RectTransform>().anchoredPosition = originPos;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// サーチウィンドウのカードを隠し、サーチウィンドウを非表示にする(非アクティブにする)　※完全に終了しているわけじゃあない
    /// </summary>
    public void CloseWindow()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// 状況を見るために一時的にウィンドウを隠す(ref+1)
    /// </summary>
    public void HideWindow()
    {
        hidden = true;
        
        CloseWindow();
    }

    //CloseWindow()で閉じられた後、ウィンドウを元の状態で再表示する。
    public void ReOpenWindow()
    {
        Debug.Log("サーチウィンドウを再展開します");
        GetComponent<RectTransform>().anchoredPosition = originPos;

    }

    /// <summary>
    /// ガイドを更新
    /// </summary>
    public void UpdateGuide()
    {
        if (!isSummonMode)
        {
            //guideText.text = effectText + " を選んでください  あと" + (needTargetsNum - selectedCards.Count) + "枚";
        }
        else
        {
            //guideText.text = effectText + " を選んでください  あと" + remainingSummonNum + "枚";
        }
    }

    public Player GetPlayer()
    {
        return pScr;
    }

    /// <summary>
    /// 効果使用処理が開始したり、2枚以上使用できる処理で一度使用されるなどして、仕様ができるようになったときに呼ばれる
    /// </summary>
    public async UniTask OnSummonAvailable()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
        pScr.OnEffectUseAvailable(inSearchWindowCards);
    }
}
