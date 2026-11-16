using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cysharp.Threading.Tasks;


public class Player : MonoBehaviour,IAttacked
{
    [Header("先攻")] public bool first;
    [Header("カメラ")] public Camera cameraScr;
    [Header("キャンバス")] public Canvas canvas;
    [Header("キャンバススクリプト")] public MatchPlayerScreen playerCanvas;
    [HideInInspector] public PlayerCamera playerCamera;
    [Header("マッチプレイヤーデータ")] public MatchPlayerData matchPlayerData;
    [Header("ライフ")] public int life = 10;
    [Header("デッキ")] public Deck deck;
    [Header("手札")] public Hand handScr;
    [Header("フィールド")] public FieldCards fieldCards;
    [Header("墓地")] public Cemetery cemeteryScr;
    [Header("スペシャルデッキ")] public SpecialDeck specialDeck;
    [Header("相手プレイヤー")] public Player opponentPlayer;
    [Header("カードポインター")] public CardPointer cardPointer;
    [Header("サーチカード")] public SearchCard searchCard;
    [Header("YES or NO")] public YESorNO yon;
    [Header("効果選択")] public ChooseEffect chooseEffect;
    [Header("特殊選択処理")] public SPSelect spSelect;
    public SelectableTarget selectableTarget;

    [HideInInspector] public int summonedNum = 0;
    [HideInInspector] public int turnSummonedNum = 0;
    [HideInInspector] public int remainingSummonRights;

    [HideInInspector] public int usedItemNum = 0;
    [HideInInspector] public int turnUsedItemNum = 0;
    [HideInInspector] public int remainingUseItemRights;
    private int oldLife;
    [HideInInspector] public int noroiNum;
    private bool isEndMatch;
    public PhaseDetailedProcess phaseProcess;
    [Header("ダメージ音")] public AudioSource damageSound;
    [Header("回復音")] public AudioSource healSound;
    [Header("勝利BGM")] public AudioSource winSound;
    [Header("敗北BGM")] public AudioSource loseSound;
    [Header("プレイヤーマッチログ")] public PlayerMatchLog playerMatchLog;

    [Header("場のカード配置管理")] public FieldCardPlacement fieldCardPlacement;//af:持たせる場所ってここでいいのだろうか　設計上確信はないが、設計の核から離れた場所にあって変更があっても影響範囲が小さいので保留

    public bool isCpu;
    public CpuAgent cpuAgent;//af:CPUはPlayerに持たせるのは少し不適切なのでいずれこの参照は消す

    //通信
    public bool isLocalPlayer;

    #region memo
    //PlayerのAwake()はMatchManagerが処理優先順で先に呼び出される前提です
    #endregion

    #region 
    [HideInInspector] public List<EffectNugget> healLifeTriggers = new List<EffectNugget>();
    #endregion

    private void Awake()
    {
        //先攻が先に処理を行い、その後後攻の処理を起動する 先攻プレイヤーが先にゲームカードを登録　ローカルクライアントも相手プレイヤークライアントからみても必ず先攻からゲームカードIDの登録をはじめ、idの一致を保証する これで必ず一致させなければ決まったカードを対象になにかするとき、誤作動が起きるのでとても重要
        if (first)
        {
            PlayerPropertiesInit();
            opponentPlayer.PlayerPropertiesInit();
        }
        isLocalPlayer = this == MatchManager.instance.localPlayer;
    }

    private void Start()
    {        
        phaseProcess = new PhaseDetailedProcess(this);
    }

    /// <summary>
    /// プレイヤーとプレイヤーが持つデッキなどの情報の初期化 MatchManagerから決まった順番で呼び出されるAwakeのような処理
    /// </summary>
    public void PlayerPropertiesInit()
    {
        playerCamera = cameraScr.GetComponent<PlayerCamera>();
        //デッキデータにあるカードをゲームカードとして実体化し、ゲームデッキに追加する
        deck.Cards.Clear();
        foreach (CardData cardData in matchPlayerData.deckCards)
        {
            deck.Cards.Add(cardData.GenerateBattleCard(this));
        }
        deck.DeckInit();
        specialDeck.SpecialDeckInit();
        foreach (CardData cardData in matchPlayerData.specialDeckCards)
        {
            specialDeck.AddSpecialDeck(cardData.GenerateBattleCard(this));
        }

        //ChangeLife((int)PhotonNetwork.CurrentRoom.CustomProperties["customLife"]);
        ChangeLife(MatchManager.instance.firstLife);
    }

    // Update is called once per frame
    void Update()
    {
        if(life != oldLife)
        {
            PairUIs.instance.UpdatePlayerHP(this, life - oldLife);
        }
        oldLife = life;

        //ライフが0以下になる
        if(life <= 0 && !isEndMatch)
        {
            Debug.Log($"{this} のライフが0になりました！");
            isEndMatch = true;
            LoseGame();
            opponentPlayer.WinGame();
        }
    }

    /// <summary>
    /// NextPhaseボタンが押される（参照＋１）
    /// </summary>
    public void PhaseButton()
    {
        Debug.Log($"フェーズボタンが押された 実際にフェーズを進めれるか: { MatchManager.instance.turnPlayer == this} ,{MatchManager.instance.phaseManager.nowPhase == PhaseManager.TurnPhases.actionPhase}, {MatchManager.instance.runningProcess.Count == 0}");
        if (MatchManager.instance.turnPlayer == this && MatchManager.instance.phaseManager.nowPhase == PhaseManager.TurnPhases.actionPhase && MatchManager.instance.runningProcess.Count == 0)
        {
            
            //imp:旧通信システム
            //if (this == MatchManager.instance.localPlayer)
            //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentClickedPhaseButton), Photon.Pun.RpcTarget.Others);

            MatchManager.instance.matchActionService.Submit(new PhaseButtonCommand(this));
        }
    }

    public void TurnStart()
    {
        Debug.Log(this + "のターン開始");
        playerCanvas.turnFrame.sprite = playerCanvas.myTurnFrameSprite;
        opponentPlayer.playerCanvas.turnFrame.sprite = playerCanvas.opponentTurnFrameSprite;
        remainingSummonRights = 1;
        remainingUseItemRights = 1;
        playerMatchLog.OnTurnStart();
    }

    public void StartActionPhase()
    {
        //このメソッド謎。確信はないが、おそらくこのStartActionPhaseをなくし、phaseProcess.TurnStartProcessをPhaseManagerのActionPhase()に入れるべきなのではないか。Triggerなどの問題でPhaseDetailedProcessにも分散する必要があって不可能なら、何とか調整してほしい。少なくとも一度ここを経由するのは謎
        //cardPointer.active = trueもStartActionPhaseAndCanCommand()(改名前）に入れた方がいいのでは？
        cardPointer.active = true;
        StartCoroutine(phaseProcess.TurnStartProcess());
    }

    /// <summary>
    /// アクションフェイズが開始し、プレイヤーが行動可能になったタイミング af:本来このメソッドの名前をStartActionPhaseにしたいが、元々あるStartActionPhaseがあるせい。そこに詳細を示すが、明らかに謎のルートなのでそのメソッドをいずれ変更する
    /// </summary>
    public void StartActionPhaseAndCanCommand()
    {
        if (isCpu)
        {
            cpuAgent.CpuFlow(this).Forget();
        }
    }

    public void EndActionPhase()
    {
        cardPointer.active = false;
        StartCoroutine(phaseProcess.TurnEndProcess());
    }

    public void TurnEnd()
    {
        turnSummonedNum = 0;
        turnUsedItemNum = 0;
        fieldCards.TurnEnd();
        playerMatchLog.OnTurnEnd();

        playerCanvas.sideSupportGuide.SetGuideText("相手ターン中だ。相手の行動を見守ろう。");
    }

    /// <summary>
    /// Handスクリプトのドローメソッドを実行
    /// </summary>
    public Card Draw()
    {
        Card drawnCard = handScr.Draw();

        return drawnCard;
    }

    /// <summary>
    /// アタックされてダメージを受ける
    /// </summary>
    /// <param name="attackDamage"></param>
    public void Attacked(Attack attacker,int attackDamage)
    {
        ReceiveDamage(attackDamage);
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"></param>
    public void ReceiveDamage(int damage)
    {
        life -= damage;
        damageSound.PlayOneShot(damageSound.clip);

        playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.ReceivedDamageTimes, 1);
        playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.ReceivedDamageSum, damage);
        TriggerEffectManager.instance.HappendPlayerTriggerMatter(this, EffectNugget.InvokeTiming.ReceivedDamage);
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Whole, $"プレイヤー{this}が {damage} ダメージを受けました。現在のライフ{life}");
    }

    /// <summary>
    /// ライフを回復する
    /// </summary>
    /// <param name="heal"></param>
    public void HealLife(int heal)
    {
        Debug.Log("回復します");
        life += heal;

        healSound.PlayOneShot(healSound.clip);

        playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.HealedLifeTimes, 1);
        playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.HealedLifeSum, heal);

        Debug.Log("回復トリガーの効果を実行します");
        TriggerEffectManager.instance.HappendPlayerTriggerMatter(this,EffectNugget.InvokeTiming.HealedLife);
        
    }

    /// <summary>
    /// ライフを一定の値に設定する
    /// </summary>
    public void ChangeLife(int lifeNum)
    {
        int beforeLife = life;
        life = lifeNum;

        if(beforeLife > lifeNum)
        {
            playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.ReceivedDamageTimes, 1);
            playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.ReceivedDamageSum, beforeLife - lifeNum);
        }
        else if(beforeLife < lifeNum)
        {
            playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.HealedLifeTimes, 1);
            playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.HealedLifeSum, lifeNum - beforeLife);
        }
    }

    /// <summary>
    /// このプレイヤーがゲームに勝利する
    /// </summary>
    public void WinGame()
    {
        matchPlayerData.winCount++;
        StartCoroutine(playerCanvas.WinEffect());
        if(isLocalPlayer)
            winSound.PlayOneShot(winSound.clip);
        GameManager.instance.userData.userExperience.AddExp(100);
        MatchManager.instance.EndGame();
    }

    /// <summary>
    /// このプレイヤーがゲームに負ける
    /// </summary>
    public void LoseGame()
    {
        matchPlayerData.loseCount++;
        StartCoroutine(playerCanvas.LoseEffect());
        if (isLocalPlayer)
            loseSound.PlayOneShot(loseSound.clip);
        //MatchManager.instance.EndGame(); win側が送信すればいいかな？
    }

    /// <summary>
    /// 残りの使用権と使用するカードに必要な使用権を照らし合わせて、そのカードが使用可能か確認する※使用権免除条件があれば、その条件を満たしていれば使用可能とする
    /// </summary>
    /// <param name="useCard"></param>
    /// <returns></returns>
    public bool IsRemainUseRights(Card useCard)
    {
        if (useCard.costForExemptionFromUseRight && useCard.costForExemptionFromUseRight.CanUseCheck())
            return true;

        if(useCard.cardType == Card.CardType.Monster)
        {
            return CanSummonCard(useCard);
        }
        else if(useCard.cardType == Card.CardType.Item)
        {
            return CanUseItemCard(useCard);
        }
        return false;
    }

    /// <summary>
    /// 残りの召喚権と召喚するカードに必要な召喚権を照らし合わせて、そのカードが召喚可能か確認する
    /// </summary>
    /// <param name="summonCard"></param>
    /// <returns></returns>
    private bool CanSummonCard(Card summonCard)
    {
        return remainingSummonRights - summonCard.useRights >= 0; 
    }

    /// <summary>
    /// 残りのアイテム権と使用するカードに必要なアイテム権を照らし合わせて、そのカードが使用可能か確認する
    /// </summary>
    /// <param name="useItemCard"></param>
    /// <returns></returns>
    private bool CanUseItemCard(Card useItemCard)
    {
        return remainingUseItemRights - useItemCard.useRights >= 0;
    }

    /// <summary>
    /// 自分ターンに行うアクションができるか
    /// </summary>
    /// <returns></returns>
    public bool CanMyTurnAction()
    {
        return MatchManager.instance.turnPlayer == this && MatchManager.instance.runningProcess.Count == 0;
    }

    /// <summary>
    /// ViewCondition()を時間差をつけ、ディレイ後に処理中でなければ実行する
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayViewCondition()
    {
        yield return new WaitForSeconds(0.22f);
        if(MatchManager.instance.runningProcess.Count == 0)
            ViewCondition();
    }

    public void ViewCondition()
    {
        bool existCanSummonMonster = ExistCanSummonCard();
        bool existCanUseItem = ExistCanUseItem();

        //このガイドが邪魔で、いずれチュートリアルと、もっと直感的にわかりやすいUIで補助する予定
        string guideText = "";

        if(existCanSummonMonster && existCanUseItem)
        {
            guideText = "<b>手札</b>に使用可能な<link=\"monster><u><color=#FF9900>モンスター</color></u></link>と<link=\"item><u><color=#4CFF00>アイテム</color></u></link>があるぞ！手札から自分の場に<link=\"monster><u><b>ドラッグ＆ドロップ</b></u></link>して使用しよう！";
        }
        else if (existCanSummonMonster && !existCanUseItem)
        {
            guideText = "<b>手札</b>に召喚可能な<link=\"monster><u><color=#FF9900>モンスター</color></u></link>があるぞ！手札から自分の場に<link=\"monster><u><b>ドラッグ＆ドロップ</b></u></link>して召喚しよう！";
        }
        else if(!existCanSummonMonster && existCanUseItem)
        {
            guideText = "<b>手札</b>に使用可能な<link=\"monster><u><color=#4CFF00>アイテム</color></u></link>があるぞ！手札から自分の場に<link=\"monster><u><b>ドラッグ＆ドロップ</b></u></link>して使用しよう！";
        }
        else
        {
            List<Card> canAttackCards = fieldCards.GetMeetConditionFieldCard((card) => card.monsterScr && card.monsterScr.attack.CanAttack());
            if(canAttackCards.Count > 0)
            {
                guideText = "<color=#FF9900>アタック/ディフェンス可能なモンスター</color>が" + canAttackCards.Count + "体いるぞ。<b>自分のモンスターをクリック</b>して<color=red>「ATTACK」</color>または<color=#3bc4d9>ディフェンス</color>をクリックしよう！";
            }
            else
            {
                List<Card> canUseEffectCards = fieldCards.GetMeetConditionFieldCard((card) => card.effectNuggets[(int)EffectNugget.InvokeTiming.MyTurn] && card.effectNuggets[(int)EffectNugget.InvokeTiming.MyTurn].CanUseCheck());
                if(canUseEffectCards.Count > 0)
                {
                    guideText = "場に<b>効果を使えるカード</b>があるぞ！<color=#4CFF00>光っているカード</b>をクリックして<color=#4CFF00>「EFFECT」</b>をクリックしよう！";
                }
                else
                {
                    guideText = "<b>右のENDボタン</b>をクリックして<b>ターンを終了</b>しよう！";
                }
            }

        }

        //playerCanvas.DisplayActionPhaseGuide(guideText);
        playerCanvas.sideSupportGuide.SetGuideText(guideText);
        //ガイドここまで
    }

    public bool ExistCanSummonCard()
    {
        foreach(Card card in handScr.hands)
        {
            if(card.monsterScr && card.CanNormalUseCheck())
                return true;
        }
        return false;
    }

    public bool ExistCanUseItem()
    {
        foreach (Card card in handScr.hands)
        {
            if (card.itemScr && card.CanNormalUseCheck())
                return true;
        }
        return false;
    }

    /// <summary>
    /// 降参する
    /// </summary>
    public void Surrender()
    {
        ChangeLife(0);
        if (isLocalPlayer)
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"サレンダーを同期します");
            MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentSurrender), Photon.Pun.RpcTarget.Others);
        }
    }

    /// <summary>
    /// カード選択ができる状態（カード選択処理開始時と1枚選ばれてもう一枚選ぶ必要があるときどちらも含む)Playerが操作主体に応じて自動で行うべき処理などがあればする。af:ここをPlayerControllerの責務にする予定。また、今はcardsとselectTypeを受け取る形にしているが、どのくらいCardPointerの情報と連携するのが適切かは迷い中
    /// </summary>
    /// <param name="canSelectCards"></param>
    public void OnCardSelectionAvailable(List<Card> canSelectCards,CardPointer.SelectType selectType)
    {
        if (isCpu)
        {
            cpuAgent.SelectCardOrder(this,canSelectCards,selectType);
        }
    }

    /// <summary>
    /// YesNo処理が開始し、必要であればリアクションをする（今のところCPUのみ反応して自動で処理する必要がある）af:今後IPlayerControllerに移す
    /// </summary>
    public void OnYesNoStarted(bool doSendCommand)
    {
        if (isCpu)
        {
            cpuAgent.DecideYesNo(this, doSendCommand).Forget();
        }
    }

    /// <summary>
    /// 複数効果選択処理が開始し、必要であればリアクションする 注釈は上のOn~系に同じ
    /// </summary>
    /// <param name="effectChoices"></param>
    public void OnEffectChoosingStarted(List<AEffectChoice> effectChoices)
    {
        if (isCpu)
        {
            UtilityLog.DebugLog("CPUの選択");
            cpuAgent.DecideEffectChoice(this, effectChoices).Forget();
        }
    }

    /// <summary>
    /// 効果使用処理が開始し、必要であればリアクションする　注釈は上のOn~系に同じ
    /// </summary>
    /// <param name="canSelectCards"></param>
    public void OnEffectUseAvailable(List<Card> canSelectCards)
    {
        if (isCpu)
        {
            cpuAgent.EffectUseOrder(this, canSelectCards);
        }
    }
}
