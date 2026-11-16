using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Card : MonoBehaviour,IOwnerPlayer,IAttacked,ICardData
{
    #region フィールド
    [Header("カードデータ")] public CardData cardData;
    [Header("カードの持ち主のプレイヤー")] public Player ownerPlayer;
    [Header("このカードのコントローラー")] public Player controlerPlayer;
    [Header("各カードタイプスクリプト")] public CardTypeCommon cardTypeScr;
    [Header("ランク")] public byte rank;
    [Header("属性")] public CardData.CardAttribute attribute;
    [Header("カテゴリー")] public List<Category> categories = new List<Category>();
    [Header("消費する使用権")] public int useRights = 1;
    [Header("コストにできない")] public bool cantCost;
    [Header("カードレイヤー")] public CardLayer cardLayer;
    [Header("セレクトフレーム")] public GameObject[] selectFrames; //なんか参照無い おそらくmatchCardHighlightに取って代わられた
    [Header("破壊エフェクト")] public GameObject destroyEffectPrefab;
    [Header("効果発動エフェクトプレハブ")] public GameObject invokeEffectAnimObj;
    [HideInInspector] public EffectNugget useCardCost;
    [HideInInspector] public EffectNugget costForExemptionFromUseRight;
    [Header("ステータス")] public CardStatuses cardStatuses;
    [Header("ステータスアイコンマネージャー")] public StatusIconsManager statusIconsManager;
    [Header("カードイメージ")] public SpriteRenderer sr;
    [Header("ボディ")] public GameObject body;
    [Header("効果ナゲットの親")] public Transform effectNuggetsParent;
    public List<Weapon> equippedWeapons = new(); //装備しているウェポン
    public SelectableTarget selectableTarget;

    public bool standbySet;
    [Header("動けなくなった時の音")] public AudioSource beParalysisedSound;
    [Header("保存されたときの音")] public AudioSource preservedSound;
    [Header("毒にかかったときの音")] public AudioSource poisonedSound;
    [Header("手札に加えた時の音")] public AudioSource addHandSound;
    [Header("墓地に送られたときの音")] public AudioSource sentCemeterySound;

    public enum CardType
    {
        none,
        Monster,
        Item,
    }

    public enum CardState
    {
        none,
        Deck,
        Hand,
        Field,
        Cemetery,
        Piled,
        SpecialDeck,
        other//処理の間に、あるStateからいったん切り離されて無所属になっている状態
    }

    public enum Category
    {
        none,
        Potaroid,
        Papuppet,
        BlackHorn,
        Reload,
        APK,
        Tami,
        Ei,
        Torque,
        Universe,
        Borgmar,
        Heartful,
        MirrorCopy,
        Undermine,
        Gate,
        Cooko,
        Harp,
        Shadow,
        FlowerAppliances,
        ElekiChange,
        LifeBlocker,
        ScarlotForest,
        Ultimate,
        Ulti_mate,
        WhiteDream,
        Bloody,
        Unlife,
        Heart,
        Food,
        HellBeast,
        Dreamer,
        Cactus,
        Break
    }

    [HideInInspector] public EffectNugget[] effectNuggets = new EffectNugget[50];
    [HideInInspector] public List<IInEffectText> minimumEffectNuggets = new List<IInEffectText>(); //インデックス番号による識別はない。とにかく効果だったらやりたい一斉処理に利用。
    //効果とはいえるが特殊なクラス
    public LifeBlocker lifeBlockerEffect;
    public AlwaysEffectNugget alwaysEffectNugget;
    public EvolutionNugget evolutionNugget;

    [HideInInspector] public CardType cardType;
    public CardState cardState = CardState.Deck;

    [Header("カードハイライトUI")] public MatchCardHighlight matchCardHighlight;
    private SpriteRenderer[] srSelectFrame;

    private Hand hand;
    [HideInInspector] public Monster monsterScr;
    [HideInInspector] public Item itemScr;
    [HideInInspector] public Weapon weapon;
    [HideInInspector] public Eternal eternal;
# region//ドラッグ処理用の変数
    private bool inCardArea = true;
    private bool overlapOtherCard = false;
    private readonly string cantSetCardAreaTag = "CantSetCardArea";
    private readonly string cardTag = "Card";
    private bool canSetCardArea;
    [HideInInspector] public bool caughtByMouse = false;
    [HideInInspector] public bool viewed;
    #endregion
    private GameObject destroyEffect;
    [HideInInspector] public bool registeredGM;
    [HideInInspector] public bool canBeChosen;//ターゲットの条件に当てはまるとき、黄色く光らせるため
    private bool oldCanBeChosen;
    [HideInInspector] public bool inSearchWindow;
    public MoveToPos moveToPos;
    [HideInInspector] public bool paidUseCost;
    private List<Action<Card.CardState>> changedCardStateCallback = new List<Action<CardState>>();
    public List<Action> leftFieldCallback = new List<Action>();
    public Vector3 originScale = Vector3.zero;

    #endregion

    public void SetOwner(Player thisOwnerPlayer)
    {
        ownerPlayer = thisOwnerPlayer;
        thisOwnerPlayer.deck.Cards.Add(this);
        Debug.Log($"{this} のオーナーは {ownerPlayer}");
    }

    private void Awake()
    {
        monsterScr = GetComponent<Monster>();
        itemScr = GetComponent<Item>();
        moveToPos = GetComponent<MoveToPos>();
        sr = GetComponent<SpriteRenderer>();
        Array.Resize(ref effectNuggets, 50);
        
        if (monsterScr)
        {
            cardType = CardType.Monster;
        }
        else if (itemScr)
        {
            cardType = CardType.Item;
            weapon = GetComponent<Weapon>();
            eternal = GetComponent<Eternal>();
        }

        originScale = transform.localScale;
    }

    void Start()
    {        
        //オーナーの手札スクリプトを取得
        if (ownerPlayer)
        hand = ownerPlayer.handScr;

        useCardCost = effectNuggets[((byte)EffectNugget.InvokeTiming.AppearCost)];
        costForExemptionFromUseRight = effectNuggets[(byte)EffectNugget.InvokeTiming.CostForExemptionFromUseRight];
    }

    void Update()
    {
        
        oldCanBeChosen = canBeChosen;            
    }

    /// <summary>
    /// カードがセット可能な場所にドラッグされているか
    /// </summary>
    /// <returns></returns>
    public bool GetCanSetCardArea()
    {
        inCardArea = true;
        overlapOtherCard = false;
        
        RaycastHit[] hits = Physics.BoxCastAll(transform.position - new Vector3(0, 1.25f, 0), new Vector3(body.transform.localScale.x, 2, body.transform.localScale.z) * 0.5f, Vector3.down, Quaternion.identity, 3);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == cantSetCardAreaTag)
            {

                if ((controlerPlayer == MatchManager.instance.players[0] && hit.transform.position.z < 0) || (controlerPlayer == MatchManager.instance.players[1] && hit.transform.position.z > 0))
                {
                    inCardArea = false;
                }
            }
            //自身以外の、親を持たないカード(カードの下に重なっていない)にhitしている ※もしCardの上にさらに親が追加される場合は変更必須
            if (hit.collider.tag == cardTag && hit.collider.gameObject != gameObject && !hit.collider.gameObject.transform.parent)
            {
                overlapOtherCard = true;
            }
        }
        
        if (inCardArea && !overlapOtherCard)
        {
            canSetCardArea = true;
        }
        else
        {
            canSetCardArea = false;
        }
        return canSetCardArea;
    }

    /// <summary>
    /// カードの判定を可視化する いまんとこ再び使う予定はない
    /// </summary>
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(transform.position - new Vector3(0, 1.25f, 0), new Vector3(transform.localScale.x, 2, transform.localScale.z));
    }
    /// <summary>
    /// ref+1
    /// </summary>
    public void PointerEnter()
    {
        if (cardState == CardState.Hand && !caughtByMouse && !inSearchWindow && !standbySet)
        {
            transform.position = new Vector3(transform.position.x, hand.transform.position.y + 1, hand.transform.position.z);
            transform.localScale = originScale * 1.15f;
        }
    }
    /// <summary>
    /// ref+1
    /// </summary>
    public void PointerExit()
    {
        if (cardState == CardState.Hand && !caughtByMouse && !inSearchWindow && !standbySet)
        {
            //transform.position = new Vector3(transform.position.x, hand.transform.position.y, transform.position.z);
            controlerPlayer.handScr.SortAHand(this);
            transform.localScale = originScale;
        }
    }
    /// <summary>
    /// ref+1
    /// </summary>
    public void PointerClick()
    {
        if(cardState == CardState.Deck && controlerPlayer.isLocalPlayer)
        {
            controlerPlayer.deck.OpenDeckWindow();
        }
    }

    /// <summary>
    /// カードを場に出す(使うとプレイヤーが判断した場合ではなく、様々な選択や条件確認の上システム上で使うことが決定されたとき)
    /// </summary>
    public void AppearCard(bool isEffectSummon)
    {
        switch (cardType)
        {
            case CardType.Monster:
                if (!isEffectSummon)
                {
                    controlerPlayer.remainingSummonRights -= useRights;
                }
                monsterScr.Summon();
                break;

            case CardType.Item:
                if (!isEffectSummon)
                {
                    controlerPlayer.remainingUseItemRights -= useRights;
                }
                itemScr.UseItemNormal();
                break;
        }

        statusIconsManager.OpenIcons();

        AnalyticsRecorder.RecordCardPlayed(cardData,this);
    }

    /// <summary>
    /// 通常使用（権利を消費するやつ）ができるか※使用権不要も含む
    /// </summary>
    /// <returns></returns>
    public bool CanNormalUseCheck()
    {
        return (cardState == Card.CardState.Hand || (cardState == Card.CardState.SpecialDeck && cardData.specialCardData.canSelfSummon)) && CanUseCheck() && controlerPlayer.IsRemainUseRights(this);
    }

    /// <summary>
    /// カードの情報から判断して使用可能かを返す
    /// </summary>
    /// <returns></returns>
    public bool CanUseCheck()
    {
        return (!useCardCost || useCardCost.CanUseCheck()) && cardTypeScr.CanUseCardType() && (!evolutionNugget || evolutionNugget.CanEvolutionCheck()) && !cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved);
    }

    /// <summary>
    /// 使用コストを払う
    /// </summary>
    /// <param name="caller"></param>
    /// <returns></returns>
    public IEnumerator PayUseCost(Action caller)
    {
        UtilityLog.CardLog("使用コストを払う処理を開始します", this, UtilityLog.LogCategory.Cost);
        //召喚が決定されるまでどこにも属さない状態とするwakedehanai
        Card.CardState initialCardState = cardState;
        cardState = Card.CardState.Hand;//warn:手札以外から出すときいいんですか？

        //召喚コストナゲットを起動する ※コールバック　召喚コストが払い終わった時のメソッドを設定
        if (useCardCost)
        {
            useCardCost.UseEffectOrder(PaidSummonCosts);
            //コストを払い終わるまで待つ
            paidUseCost = false;
            while (!paidUseCost)
            {
                yield return null;
            }
        }

        if (evolutionNugget)
        {
            evolutionNugget.EvolutionOrder(PaidSummonCosts);
            //進化素材を選び終わるまで待つ
            paidUseCost = false;
            while (!paidUseCost)
            {
                yield return null;
            }
        }
        
        cardState = initialCardState;

        //呼び出し元にコスト支払い完了を伝える
        caller.Invoke();
    }

    /// <summary>
    /// 使用コストを払う
    /// </summary>
    /// <param name="caller"></param>
    /// <returns></returns>
    public IEnumerator PayExemptionFromUseRightCost(Action caller)
    {
        UtilityLog.CardLog("使用権免除コストを払う処理を開始します", this, UtilityLog.LogCategory.Cost);
        //召喚が決定されるまでどこにも属さない状態とするwakedehanai
        Card.CardState initialCardState = cardState;
        cardState = Card.CardState.Hand;//warn:手札以外から出すときいいんですか？

        //召喚コストナゲットを起動する ※コールバック　召喚コストが払い終わった時のメソッドを設定
        if (costForExemptionFromUseRight)
        {
            costForExemptionFromUseRight.UseEffectOrder(PaidSummonCosts);
            //コストを払い終わるまで待つ
            paidUseCost = false;
            while (!paidUseCost)
            {
                yield return null;
            }
        }

        cardState = initialCardState;

        //呼び出し元にコスト支払い完了を伝える
        caller.Invoke();
    }

    /// <summary>
    /// 召喚コスト/進化処理が終わる
    /// </summary>
    public void PaidSummonCosts()
    {
        UtilityLog.CardLog("処理完了の信号を受け取りました", this, UtilityLog.LogCategory.Cost);
        paidUseCost = true;
    }


    #region カードボタンに関するメソッド
    public void EffectButton()
    {
        if (CanUseMyTurnEffect() && CanMyTurnCommand())
        {
            UtilityLog.CardLog("アクションフェイズエフェクトを発動", this, UtilityLog.LogCategory.none);

            MatchManager.instance.matchActionService.Submit(new EffectButtonCommand(controlerPlayer, MatchManager.instance.GetCardId(this)));

            //imp:旧コマンド/通信システムの処理
            //effectNuggets[((byte)EffectNugget.InvokeTiming.MyTurn)].UseEffectOrder();

            //if (controlerPlayer == MatchManager.instance.localPlayer)
            //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentEffectButton), Photon.Pun.RpcTarget.Others, MatchManager.instance.GetCardId(this));
        }
        else
        {
            UtilityLog.CardLog("起動効果の使用条件を満たしません", this, UtilityLog.LogCategory.none);
        }
    }

    /// <summary>
    /// 自分ターンの起動効果が発動できるかを返す ※「対戦処理中で実際には命令を実行可能できない」等は除き、盤面状況的に発動可能かを返す
    /// </summary>
    /// <returns></returns>
    public bool CanUseMyTurnEffect()
    {
        EffectNugget myTurnEffect = effectNuggets[((byte)EffectNugget.InvokeTiming.MyTurn)];

        return myTurnEffect && myTurnEffect.GetTurnUsedNum() < myTurnEffect.turnCanUseNum && myTurnEffect.CanUseCheck() && !cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved) && controlerPlayer == MatchManager.instance.turnPlayer;
    }

    /// <summary>
    /// 自分ターンに処理中でないときに実行するアクションコマンド（カード使用やアタック/ディフェンス、効果発動、ターンエンドなどができる状態かどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool CanMyTurnCommand()
    {
        //na:この条件で過不足ないかどうか確信なし
        return MatchManager.instance.runningProcess.Count == 0 && MatchManager.instance.phaseManager.nowPhase == PhaseManager.TurnPhases.actionPhase && controlerPlayer.cardPointer.pointerMode == CardPointer.PointerModes.cardInfo;
    }

    public void CloseButton()
    {
        viewed = false;
        controlerPlayer.playerCanvas.cardInfoMenu.CloseMenu(this);
        controlerPlayer.cardPointer.viewingCard = null;
    }
    #endregion

    #region カードの状態を操る

    /// <summary>
    /// CardState変更時、変更前の状態に応じて処理を実行しつつ指定された状態移行を行います。(値が必要ない場合 例:field,hand,cemetery)
    /// </summary>
    /// <param name="changeState">変更したいCardState</param>
    public void ChangeCardState(CardState changeState)
    {
        ChangeCardState(changeState, 0);
    }

    /// <summary>
    /// CardState変更時、変更前の状態に応じて処理を実行しつつ指定された状態移行を行います
    /// </summary>
    /// <param name="changeState">変更したいCardState</param>
    /// <param name="stateValue">必要に応じてその場所に移行するのに必要な値</param>
    public void ChangeCardState(CardState changeState,int stateValue)
    {
        UtilityLog.CardLog("cardStateを" + changeState + "に変更します", this, UtilityLog.LogCategory.ChangeCardState);
        CardState initialCardState = cardState;
        //if (cardState != changeState) これってないとだめなのかな
        //{
        //指定したstateに変更する
        switch (changeState)
        {
            case CardState.Deck:
                controlerPlayer.deck.AddDeck(this, stateValue);
                break;

            case CardState.Hand:
                controlerPlayer.handScr.AddHand(this);
            addHandSound.PlayOneShot(addHandSound.clip);
                break;

            case CardState.Field:
                cardState = CardState.Field;
                controlerPlayer.fieldCards.cards.Add(this);
                SetCardRotation(new Vector3(90, 0, 0));
            break;

            case CardState.Cemetery:
                SentToCemetery();
                break;

            case CardState.Piled:
                moveToPos.StopMoveToVector();
                cardState = CardState.Piled;
                break;

            case CardState.SpecialDeck:
                controlerPlayer.specialDeck.AddSpecialDeck(this);
                break;

            case CardState.other:
                cardState = CardState.other;
                break;
        }

        //変更前のカード状態に応じて処理を実行する
        switch (initialCardState)
        {
            case CardState.Deck:
                LeaveDeck();
                break;

            case CardState.Hand:
                LeaveHand();
                break;

            case CardState.Field:
                LeaveField();
                if (cardLayer && cardLayer.transform.childCount > 0)
                {
                    if (changeState != CardState.Piled)
                    {
                        cardLayer.UnPileTopThis(CardState.Cemetery);
                    }
                    //既に下にカードを重ねていた場合、それらも一緒に新たな親に重ねる。
                    else
                    {
                        cardLayer.UnPileTopThis(CardState.other);
                    }
                }
                break;

            case CardState.Cemetery:
                LeaveCemetery();
                break;

            case CardState.SpecialDeck:
                LeaveSpecialDeck();
                break;

            case CardState.Piled:
                transform.parent.GetComponent<CardLayer>().ReleaseUnderCard(this);
                if (weapon)
                {
                    weapon.equipTarget.card.equippedWeapons.Remove(weapon);
                    weapon.equipTarget = null;
                }
                break;
        }

        //変更先が無効なステータスをリセットする
        foreach (Status status in cardStatuses.GetActiveStatuses())
        {
            if (status.statusInfo.enableState.Count != 0 && !status.statusInfo.enableState.Contains(changeState))
            {
                //status.BaseStatusValue = 0;
                cardStatuses.statusesDictionary.Remove(status.statusInfo.statusType);
            }
        }

        foreach (StatusData statusData in cardData.effectData.statusDatas)
        {
            if (MatchManager.instance.statusDataBase.statusInfos[(byte)statusData.statusType].enableState.Contains(changeState))
            {
                cardStatuses.SetStatusValue<Status>(statusData.statusType, statusData.statusValue, true, false).DefaultStatusValue = statusData.statusValue;
            }
        }
        //}
        //CardState変更時のコールバックを送る
        foreach (Action<CardState> callback in changedCardStateCallback)
        {
            callback.Invoke(changeState);
        }

        //カードの場所が変更されたときゲームの状態が更新された通知をする ※この処理は上の信号を送る処理より後でなければならない
        MatchManager.instance.UpdatedGameStatus();
    }

    #region それぞれのCardStateから離れた時の処理 ※ChangeCardState以外から呼ばないで。もし変えたかったらしっかり全体の仕組みを見直してから。
    /// <summary>
    /// デッキから離れたとき
    /// </summary>
    private void LeaveDeck()
    {
        controlerPlayer.deck.LeaveDeck(this);
    }

    /// <summary>
    /// 手札から離れたとき(手札リストから取り除き、手札を整理するなど)
    /// </summary>
    private void LeaveHand()
    {
        hand.hands.Remove(this); //使ったカードを手札から取り除く
        matchCardHighlight.HighlightBorderEnable(false);
        ResetCardLook();
        if(hand.hands.Count > 0)
        {
            hand.SortHands();
        }
    }

    /// <summary>
    /// 場を離れた時の処理
    /// </summary>
    private void LeaveField()
    {
        UtilityLog.CardLog(name + "が場を離れました", this, UtilityLog.LogCategory.ChangeCardState);
        controlerPlayer.fieldCards.cards.Remove(this);

        //ステータス関連
        statusIconsManager.CloseIcons();
        foreach(Status status in cardStatuses.GetActiveStatuses())
        {
            status.ClearGivedStatus();
        }

        //このカードのメニューが開いていたら閉じる（処理変える可能性あり）
        if(controlerPlayer.playerCanvas.cardInfoMenu.openingMenuCard == this)
        {
            controlerPlayer.playerCanvas.cardInfoMenu.CloseMenu(this);
        }

        //モンスターが場を離れた時の処理
        if (monsterScr)
        {
            monsterScr.barrier.DisableBarrier();
        }

        for(int i = 0; i < leftFieldCallback.Count; i++)
        {
            leftFieldCallback[i].Invoke();
        }
        leftFieldCallback = new List<Action>();

        controlerPlayer.fieldCardPlacement.LeaveField(this);
    }

    /// <summary>
    /// 墓地から離れた時の処理
    /// </summary>
    private void LeaveCemetery()
    {
        controlerPlayer.cemeteryScr.LeaveCemetery(this);
    }

    /// <summary>
    /// スペシャルデッキから離れた時の処理
    /// </summary>
    private void LeaveSpecialDeck()
    {
        controlerPlayer.specialDeck.LeaveSpecialDeck(this);
    }
    #endregion

    /// <summary>
    /// 墓地へ送られる
    /// </summary>
    private void SentToCemetery()
    {
        cardState = CardState.Cemetery;
        SetCardRotation(new Vector3(90, 0, 0));
        UtilityLog.CardLog("墓地に送られます", this, UtilityLog.LogCategory.ChangeCardState);
        ComeInvokeTiming(EffectNugget.InvokeTiming.SentCementery);
        controlerPlayer.playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.SentCemetery, 1);
        controlerPlayer.cemeteryScr.SendCemetery(this);
    }

    //引数effectTypeじゃなくて直接effect送れば良くねって思うけど色々不都合があったからいつか改善できそうならしたい
    /// <summary>
    /// 効果によりこのカードが悪い影響を受ける妨害を受ける(バウンスされるなどの固有処理の先にこれを実行する※これを実行すれば自動で固有処理が実行される)
    /// </summary>
    /// <param name="disruptType">妨害の種類</param>
    public void BeDisruptedByEffect(Effect.EffectTypes effectType, Card disrupter)
    {
        if (cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Dodge) && cardStatuses.statusesDictionary[StatusData.StatusType.Dodge].StatusValue > 0)
        {
            UtilityLog.CardLog("避ける処理を開始します", this, UtilityLog.LogCategory.none);
            cardStatuses.statusesDictionary[StatusData.StatusType.Dodge].DecreaseStatusValue();
            
            EffectManager.instance.AddInterrupt(this);
            //選択可能なカードからもともと選択されているターゲットを除外する※敵(効果を使ったプレイヤー)から見ての条件なのでownerPlayer.opponentPlayerとする
            List<Card> excludedOriginallyTargets = EffectManager.instance.ability.targetDecideNugget.SearchTotalTargets(EffectManager.instance.userCard);

            foreach (Card originallyTarget in EffectManager.instance.targetsSets[0])
            {
                Debug.Log(originallyTarget + "を除外");
                excludedOriginallyTargets.Remove(originallyTarget);
            }
            //上の処理をされたターゲットたちからランダムに効果対象を選ぶ warn:現状その効果で選択可能なターゲットからランダムに選ぶようになっているので、そうじゃなく、選んだターゲットと同じ持ち主の場のカード柄選ぶようにする
            if (excludedOriginallyTargets.Count > 0)
            {
                List<Card> randomizedTarget = new List<Card>
                {
                    SuzumonTools.RandomTargets(excludedOriginallyTargets)
                };
                Debug.Log("避けた結果ランダムに選ばれたターゲットに対して効果を実行します");
                EffectManager.instance.UseEffectForTarget(randomizedTarget);
            }
            else
            {
                Debug.Log("避けられた結果、他にターゲットにするモンスターが存在しなかった為効果が失敗しました");
                //効果は終わったことにする
                EffectManager.instance.EndInterrupt(this);
                EffectManager.instance.EndEffect();
            }
        }
        else
        {
            switch (effectType)
            {
                case Effect.EffectTypes.DestroyCard:
                    BeDestroyed();
                    break;

                case Effect.EffectTypes.SendCemetery:
                    SentToCemetery();
                    break;

                case Effect.EffectTypes.Bounce:
                    Bounced();
                    break;

                case Effect.EffectTypes.Noroi:
                    Noroware();
                    break;
            }
        }
    }

    public void Attacked(Attack attacker,int attackDamage)
    {
        UtilityLog.CardLog(attacker.gameObject.name + "にアタックされました", this, UtilityLog.LogCategory.none);
        if (cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Dodge) && cardStatuses.statusesDictionary[StatusData.StatusType.Dodge].StatusValue > 0)
        {
            Debug.Log(this.name + "の、アタックを避ける処理を開始します");
            cardStatuses.statusesDictionary[StatusData.StatusType.Dodge].DecreaseStatusValue();
            
            //選択可能なカードからもともと選択されているターゲットを除外する
            List<Card> excludedOriginallyTargets = attacker.conditionsNugget.GetMeetConditionsCards(attacker.monster.card);
            attacker.conditionsNugget.DebugLogTargetConditionsNugget();
            //避けたモンスター以外を候補とする
            excludedOriginallyTargets.Remove(this);
            
            //上の処理をされたターゲットたちからランダムに攻撃対象を選ぶ
            if (excludedOriginallyTargets.Count > 0)
            {
                Card randomizedTarget = SuzumonTools.RandomTargets(excludedOriginallyTargets);
                Debug.Log($"避けた結果ランダムに選ばれたターゲット'{randomizedTarget}'に対してアタックします");
                StartCoroutine(attacker.SelectedAttackTarget(randomizedTarget.selectableTarget));
            }
            else
            {
                Debug.Log("避けられた結果、他にターゲットにするモンスターが存在しなかった為アタックが失敗しました");
            }           
        }
        else
        {
            BeDestroyed();
            ComeInvokeTiming(EffectNugget.InvokeTiming.AfterWasAttacked);
        }
    }

    /// <summary>
    /// 破壊される(確定はせず、なんらかの影響によって免れ、免れなかった場合破壊が確定する
    /// </summary>
    public void BeDestroyed()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.ChangeCardState, "破壊されます");
        Protected protect = GetComponent<Protected>();
        if (cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.WithStand) && cardStatuses.statusesDictionary[StatusData.StatusType.WithStand].StatusValue > 0)
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.ChangeCardState, "破壊に耐えます");

            if (cardStatuses.statusesDictionary[StatusData.StatusType.WithStand].StatusValue >= 2)
            {
                monsterScr.monsterAnimation.PlayWithstandAnimation();
            }
            else
            {
                monsterScr.monsterAnimation.PlayWithstandBrokenAnimation();
            }

            cardStatuses.statusesDictionary[StatusData.StatusType.WithStand].DecreaseStatusValue();
            MatchManager.instance.audioManager.PlaySoundClip(MatchManager.instance.audioManager.withStandSound, 0.6f);
            ComeInvokeTiming(EffectNugget.InvokeTiming.Withstand);//この次に処理作るならコルーチンやな
        }
        else if (protect)
        {
            protect.InvokeProtect();
        }
        else
        {
            Destroyed();
        }
    }

    /// <summary>
    /// 破壊される
    /// </summary>
    public void Destroyed()
    {
        StartCoroutine(DestroyedProcessNugget());
    }

    public IEnumerator DestroyedProcessNugget()
    {
        ComeInvokeTiming(EffectNugget.InvokeTiming.BeforeDestroyed);

        while (!MatchManager.instance.CanProceedProcess())
        {
            yield return null;
        }

        destroyEffect = Instantiate(destroyEffectPrefab);
        destroyEffect.transform.position = transform.position;

        ChangeCardState(CardState.Cemetery);

        controlerPlayer.playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.Destroyed, 1);
        ComeInvokeTiming(EffectNugget.InvokeTiming.Destroyed);

        UtilityLog.CardLog(name + "が破壊されました", this, UtilityLog.LogCategory.none);
    }

    /// <summary>
    /// 呪いをかけられる
    /// </summary>
    public void Noroware()
    {
        //controlerPlayer.spSelect.Noroware(this); 旧呪い
        cardStatuses.SetStatusValue<Noroi>(StatusData.StatusType.Noroi, 1, false, false);
    }

    /// <summary>
    /// バウンスされる
    /// </summary>
    public void Bounced()
    {
        ChangeCardState(Card.CardState.Hand);

        ComeInvokeTiming(EffectNugget.InvokeTiming.Bounced);
    }

    /// <summary>
    /// 動けなくなる
    /// </summary>
    public void Paralysed()
    {
        beParalysisedSound.PlayOneShot(beParalysisedSound.clip);
        ComeInvokeTiming(EffectNugget.InvokeTiming.BeParalysed);
    }

    /// <summary>
    /// 毒にかかる
    /// </summary>
    public void Poisoned()
    {
        poisonedSound.PlayOneShot(poisonedSound.clip);
        ComeInvokeTiming(EffectNugget.InvokeTiming.Poisoned);
    }

    /// <summary>
    /// 毒殺される
    /// </summary>
    public void DestroyedByPoison()
    {
        BeDestroyed();
        ComeInvokeTiming(EffectNugget.InvokeTiming.DestroyedByPoison);
    }

    /// <summary>
    /// 保存される
    /// </summary>
    public void Preserved()
    {
        ComeInvokeTiming(EffectNugget.InvokeTiming.Preserved);
        preservedSound.PlayOneShot(preservedSound.clip);
    }

    //Bounced()とAddedToHandの棲み分けは検討中

    /// <summary>
    /// 手札に加えられる
    /// </summary>
    public void AddedToHand()
    {
        ChangeCardState(CardState.Hand);
    }

    /// <summary>
    /// ウェポンを装備したときに呼ばれる
    /// </summary>
    public void Equipped(Weapon equipWeapon)
    {
        equippedWeapons.Add(equipWeapon); //af!:ウェポンの装備が外れた時にリストから除かなければならぬ
    }
    
    #endregion
    public void SetCardRotation(Vector3 changeReferencePosition)
    {
        transform.eulerAngles = changeReferencePosition;

        if (controlerPlayer)
        {
            if(controlerPlayer == MatchManager.instance.players[1])
            {
                transform.eulerAngles += new Vector3(0, 180, 0);
            }
        }
    }

    /// <summary>
    /// このカードが持っている全てのカテゴリのうちどれかが条件のカテゴリに一致するか調べる
    /// </summary>
    /// <param name="conditionCategory"></param>
    /// <returns></returns>
    public bool DoCategoriesMeetCondition(Category conditionCategory)
    {
        foreach(Category category in categories)
        {
            if(category == conditionCategory)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// (enum)InvokeTimingに含まれる発動タイミングのときに呼ばれる ※注意！MyTurnやAppearCostなど特に起きた事象でないものは通らなくてもよいが、それ以外の誘発に反応しうる事象を含んでいる場合、必ずここを通らなければならない。ただし、あくまで全てのInvokeTimingが通る、Effect起動時の絶対的な共通処理ではない
    /// </summary>
    /// <param name="invokeTiming"></param>
    public void ComeInvokeTiming(EffectNugget.InvokeTiming invokeTiming)
    {
        
        if(effectNuggets[(byte)invokeTiming] && effectNuggets[(byte)invokeTiming].triggerCondition == null)
        {
            effectNuggets[(byte)invokeTiming].AddTriggerQueue();
        }

        TriggerEffectManager.instance.HappenedTriggerMatter(this, invokeTiming);
    }

    /// <summary>
    /// カードの色を変える
    /// </summary>
    public void SetColor(Color color)
    {
        body.GetComponent<MeshRenderer>().materials[0].color = color;
        sr.color = color;
    }

    /// <summary>
    /// CardStateが変更されたときのコールバックを設定する
    /// </summary>
    public void SetChangedCardStateCB(Action<Card.CardState> callback)
    {
        changedCardStateCallback.Add(callback);
    }

    /// <summary>
    /// そのカードが、指定されたプレイヤーから見て公開されているかを返す
    /// </summary>
    /// <param name="viewerPlayer"></param>
    /// <returns></returns>
    public bool GetPublicArea(Player viewerPlayer)
    {
        switch (cardState)
        {
            case CardState.Deck:
                return false;

            case CardState.Hand:
                return ownerPlayer == viewerPlayer;

            case CardState.SpecialDeck:
                return ownerPlayer == viewerPlayer;

            case CardState.Field:
                return true;

            case CardState.Cemetery:
                return true;

            case CardState.Piled:
                return true;

            case CardState.other:
                return true;
        }
        return true;
    }

    /// <summary>
    /// 手札に存在するときに位置や見た目などが変化しているカードを元に戻す
    /// </summary>
    public void ResetCardLook()
    {
        transform.localScale = originScale;
    }

    public Player GetPlayer()
    {
        return ownerPlayer;
    }

    #region interface
    public CardData GetCardData()
    {
        return cardData;
    }

    public void SetCardData(CardData thisCardData)
    {
        cardData = thisCardData;
    }
    #endregion
}
