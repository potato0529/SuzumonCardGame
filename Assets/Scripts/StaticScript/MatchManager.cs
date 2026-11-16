using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    [Header("フェーズマネージャー")] public PhaseManager phaseManager;
    [Header("プレイヤー")] public Player[] players; //先攻が0、後攻が1
    [Header("プレイヤー1カメラ")] public Camera camera1;
    [Header("プレイヤー2カメラ")] public Camera camera2;
    [Header("プレイヤー1キャンバス")] public Canvas canvas1;
    [Header("プレイヤー2キャンバス")] public Canvas canvas2;
    [Header("処理を実行中のプレイヤー")] public Dictionary<RunningProcessType, RunningProcess> runningProcess = new Dictionary<RunningProcessType, RunningProcess>();  //この処理に含まれるものは、その処理が行われている間カードを使用したりフェーズを進めたりできない状態にするものに限る。
    [Header("シングルモード")] public bool singleMode;//仮
    [HideInInspector] public bool isSoloMode = false;//singleModeはこの変数に変えるかもしれない。ただもしかしたら問題が生じるあるいは完全に代替するものではない可能性もあるので一旦置いとく

    /// <summary>
    /// この処理中はプレイヤーはコマンドを実行できないという処理群
    /// </summary>
    public enum RunningProcessType
    {
        Select,
        EffectProcess,
        YesNo,
        AntiqueBurst,
        EveryTurnNoroi,
        TurnEndTriggerChecking,
        ChooseEffect,
        PhaseProcess,
        MovingCards,
        AttackWhole,
        Attacking,
        UseCard
    }

    [Header("手札・フィールド・墓地のカード")] public List<Card> handFieldCemeteryCards;
    [Header("フィールド")] public FieldManager field;
    [Header("ゲームの状態が更新された通知を送る先"), SerializeField] private List<IUpdatedGameStatus> updatedGameNoticeReceiver = new List<IUpdatedGameStatus>(); //様々なクラスが通知を必要とする可能性があるけどとりあえず今はナゲット限定
    [Header("生成されたカード")] public List<Card> generatedCards = new List<Card>();
    [Header("ダイス")] public Dice dice;
    [Header("ライト")] public Light directionalLight;
    [Header("ボードライト")] public Light boardLight;
    [Header("ステータスデータベース")] public StatusDataBase statusDataBase;
    [Header("マッチログ")] public MatchLog matchLog;
    [Header("アンティークバーストリスト")] public AntiqueBurstList antiqueBurstList;
    [Header("オーディオマネージャー")] public AudioManager audioManager;
    [HideInInspector] public NetworkPlayer networkPlayer;
    public int firstLife;
    [Header("初期ライフ設定")] public int firstLifeConfig;//初期ライフをここで設定する。af:firstLifeの扱いがよくわからないので、今後そこを直してfirstLifeConfigがなくなる可能性あり

    public List<IUpdatedGameStatus> UpdatedGameNoticeReceiver
    {
        get 
        {
            return updatedGameNoticeReceiver;
        }
    }

    public static MatchManager instance;

    public Player turnPlayer;
    [HideInInspector] public List<Monster> turnAttackedMonsters = new List<Monster>();
    [HideInInspector] public List<Monster> turnDefencedMonsters = new List<Monster>();
    [HideInInspector] public List<Monster> turnCommandedMonsters = new List<Monster>();
    [HideInInspector] public List<EffectNugget> turnUsedEffects = new List<EffectNugget>();
    [HideInInspector] public bool isLocalPlayerFirst;
    public Player localPlayer;

    [SerializeField] private List<Card> gameCards = new List<Card>();

    //デッキのシャッフルやサイコロ、etcなど、盤面内で発生する乱数のシード。ミニゲームなどを実装する場合や関係のない内部処理など、盤面外の乱数シードはこれに当てはまらない。
    public int DuelSeed
    {
        get; private set;
    }

    //DuelSeed+1の値で固定
    public int CpuSeed
    {
        get; private set;
    }

    //マジでデュエル内のみとはいえどランダム処理の順番が完全に互いに担保されていないと一瞬でぶっ壊れる。もし今後担保できなそうになったらシャッフルのランダム処理はshuffleRandom、サイコロがdiceRandom、ランダムターゲットはtargetRandomとか分けようかな。流石に同じ種類の処理の順番がごっちゃになることはないと思うから。
    public ReproducibleRandom duelRandom;

    public ReproducibleRandom cpuRandom;

    public MatchSound matchSound;//こいつら統合してもいいかも
    public MatchBGM matchBGM;

    public MatchActionService matchActionService;
    public HistoryRecorder HistoryRecorder { get; private set; }

    public MatchConfig matchConfig;
    public MatchConfigInspector matchConfigInspector;

    #region memo
    //MatchManagerは制作当時マルチプレイを考慮せずに作ったのでこれはサーバーで管理するクラスだと言ってよいでしょう
    #endregion

    private void Awake()
    {
        instance = this;
        phaseManager.matchManager = this;

        //インスペクターのマッチ用コンフィグを反映
        matchConfig = new MatchConfig();
        matchConfig.cardMoveAnimFps = matchConfigInspector.cardMoveAnimFps;
        matchConfig.effectAnimFps = matchConfigInspector.effectAnimFps;
        matchConfig.attackAnimFps = matchConfigInspector.attackAnimFps;
        matchConfig.cardHighlightAnimCycleTime = matchConfigInspector.cardHighlightAnimCycleTime;
        matchConfig.cardHighlightAnimSinScale = matchConfigInspector.cardHighlightAnimSinScale;
        matchConfig.cardHighlightAnimMinScale = matchConfigInspector.cardHighlightAnimMinScale;

        //マッチングの処理
        MatchPlayerData[] matchPlayerDatas = GameManager.instance.matchMaker.GetMatchedPlayers(); //マッチングしたプレイヤー1組を受け取る

        isSoloMode = GameManager.instance.isSoloMode;
        Debug.Log($"{isSoloMode} ソロモードかどうか");

        //ソロモードかどうかでtransmiterがOnlineTransmitterかDummyTransmitterかに変わり、matchActionServiceのインスタンスを生成するときの処理が分岐内に分散しないよう共通化するためにここで定義
        IMatchTransmitter transmitter;

        //先攻後攻処理 af:本来このifの条件式は(!singleMode)となっていた。カメラ切り替えて1人でやるようのsingleModeという変数を用いていたが、しばらく放置していてシーンをまたぐ情報でもないので一旦別の式に変えた。
        if (!isSoloMode) 
        {
            //af:これってsingleModeでもそうじゃなくてもどっちでもやるべきじゃないのか？
           // camera1.targetDisplay = 0;
            camera2.targetDisplay = 0;
            canvas1.targetDisplay = 1;
            canvas2.targetDisplay = 1;

            DuelSeed = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomRandomSeed"];
            Debug.Log("シード" + DuelSeed);
            duelRandom = new ReproducibleRandom(DuelSeed);//2026/06/27時点、CPUの行動決定のためのランダム処理は試合に干渉せず内部的に済ませるものであり、全体として同期すべきものではないためこのときにおいてのみ、duelRandomを使うべきではないと思われる

            //先攻か後攻か
            bool isMasterFirst;

            //先攻/後攻希望が競合した場合はランダムで決定、そうでなければ希望の通りに設定
            if (matchPlayerDatas[0].customFirst == matchPlayerDatas[1].customFirst)
            {
                if (Random.Range(0, 1) == 0)
                {
                    isMasterFirst = true;
                }
                else
                {
                    isMasterFirst = false;
                }
            }
            else if(PhotonNetwork.IsMasterClient)
            {
                isMasterFirst = matchPlayerDatas[0].customFirst;
            }
            else
            {
                isMasterFirst = matchPlayerDatas[1].customFirst;
            }

            if (isMasterFirst == PhotonNetwork.IsMasterClient) //(bool)PhotonNetwork.CurrentRoom.CustomProperties["IsMasterFirst"]
            {
                Debug.Log("あなたは先攻です");
                isLocalPlayerFirst = true;
                camera1.gameObject.SetActive(true);
                camera2.gameObject.SetActive(false);
                canvas1.targetDisplay = 0;

                //先攻のプレイヤーがプレイヤー1のデータに入る
                players[0].matchPlayerData = matchPlayerDatas[0];
                players[1].matchPlayerData = matchPlayerDatas[1];
                localPlayer = players[0];
            }
            else
            {
                Debug.Log("あなたは後攻です");
                isLocalPlayerFirst = false;
                camera2.gameObject.SetActive(true);
                camera1.gameObject.SetActive(false);
                canvas2.targetDisplay = 0;

                //先攻のプレイヤーがプレイヤー1のデータに入る
                players[0].matchPlayerData = matchPlayerDatas[1];
                players[1].matchPlayerData = matchPlayerDatas[0];
                localPlayer = players[1];
            }

            //NetworkPlayerをネットワークオブジェクトとして1個だけ存在するように生成
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate("NetworkPlayer", Vector3.zero, Quaternion.identity);
            }

            transmitter = new OnlineTransmitter();
        }
        else
        {
            //シードのカスタムが入力されていればそれを使う、なければランダムで生成する ※GameManager.instance.matchMenuSettings.Seedは一旦応急措置でGameManagerに置いているだけであり、今後はマッチ設定を受け渡す仕組みを別で用意するため、そのときはこれを書き換える
            if(GameManager.instance.matchMenuSettings.Seed == -1)
                DuelSeed = Random.Range(0, 1000000);
            else
                DuelSeed = GameManager.instance.matchMenuSettings.Seed;

            Debug.Log($"シード:{DuelSeed}");
            duelRandom = new ReproducibleRandom(DuelSeed);

            CpuSeed = DuelSeed + 1; //CPUの行動決定のためのランダム処理はルール上のランダム処理とは混ざらないよう別のシードで処理するが、ログをとるときにCPU戦のときのみ専用シードを入れる仕組みにするのは面倒なのでduelSeed+1で簡単に管理できるように
            cpuRandom = new ReproducibleRandom(CpuSeed);


            //もっと行数短くできると思われ na:↓の条件分岐をduelRandom.RangeからRandom.Rangeに変えた。盤面（デッキシャッフルも含め）に関係する乱数生成ではないし、ノイズになってオンライン戦かCPU戦かで乱数がずれてしまうので。もしこれでバグが増えたら戻すこと。
            if (Random.Range(0, 1) == 0)
            {
                players[0].matchPlayerData = matchPlayerDatas[0];
                players[1].matchPlayerData = matchPlayerDatas[1];
                localPlayer = players[0];

                camera1.gameObject.SetActive(true);
                camera2.gameObject.SetActive(false);
            }
            else
            {
                players[0].matchPlayerData = matchPlayerDatas[1];
                players[1].matchPlayerData = matchPlayerDatas[0];
                localPlayer = players[1];

                camera1.gameObject.SetActive(false);
                camera2.gameObject.SetActive(true);
            }



            //ソロモードのときは通信不要なのでプレイヤーのコマンドの送信先をダミーに
            transmitter = new DummyTransmitter();

            localPlayer.opponentPlayer.isCpu = true;
            localPlayer.opponentPlayer.cpuAgent = new CpuAgent();
            localPlayer.opponentPlayer.cpuAgent.SetStrategy(new CpuRandomStrategy());
        }

        HistoryRecorder = new HistoryRecorder();
        matchActionService = new MatchActionService(transmitter, HistoryRecorder);

        Debug.Log($"{matchPlayerDatas[0]} と {matchPlayerDatas[1]} でデュエル！！");

        turnPlayer = players[0];

        if(localPlayer == players[0])
        {
            firstLife = matchPlayerDatas[0].customLife;
        }
        else
        {
            firstLife = matchPlayerDatas[1].customLife;
        }
        if (firstLife == 0) firstLife = firstLifeConfig;

        MatchStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            string log = "";
            foreach(RunningProcessType processType in runningProcess.Keys)
            {
                log += processType + ",";
            }
            Debug.Log("実行中の処理" + log);
        }

        if (Input.GetKeyDown("d"))
        {
            foreach(Card card in localPlayer.fieldCards.cards)
            {
                card.cardLayer.ArrangementLayer();
            }
            foreach(Card card1 in localPlayer.opponentPlayer.fieldCards.cards)
            {
                card1.cardLayer.ArrangementLayer();
            }
        }
            
        
        if (runningProcess.Count > 0)
        {
            directionalLight.intensity = 0.6f;
            boardLight.intensity = 0.35f;
        }
        else
        {
            directionalLight.intensity = 1.0f;
            boardLight.intensity = 0.6f;
        }        

        //TriggerEffectManager.instance.InvokeQueueTrigger();
    }

    /// <summary>
    /// ゲームに存在するカードを登録する
    /// </summary>
    /// <param name="card"></param>
    public void AddGameCard(Card card)
    {
        gameCards.Add(card);
        //ゲーム内カードidはインデックス番号
    }

    /// <summary>
    /// 試合内の全てのカードを格納する変数gameCardsのGetter
    /// </summary>
    /// <returns></returns>
    public List<Card> GetGameCards()
    {
        return gameCards;
    }

    /// <summary>
    /// 渡されたCardの試合内IDを返す
    /// </summary>
    /// <param name="card">IDを知りたいカード</param>
    /// <returns></returns>
    public int GetCardId(Card card)
    {
        return gameCards.IndexOf(card);
    }

    /// <summary>
    /// ゲームが始まる（試合のシーンが読み込まれたときではなく、最初のターンが始まるとき)
    /// </summary>
    public void MatchStart()
    {
        phaseManager.FirstPhase();
    }

    /// <summary>
    /// 実行中の処理を記憶する
    /// </summary>
    /// <param name="process"></param>
    /// <param name="callerMethodName"></param>
    public void SetRunningProcess(RunningProcessType processType, RunningProcess process, [CallerMemberName] string callerMethodName = "")
    {        
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Whole, "処理" + processType + "処理を開始したメソッド" + callerMethodName);

        if (runningProcess.Count == 0)
            BecomeIncapableOfAction();
        runningProcess.Add(processType,process);
    }

    /// <summary>
    /// 実行中の処理が終了する
    /// </summary>
    /// <param name="process"></param>
    /// <param name="callerMethodName"></param>
    public void CompRunningProcess(RunningProcessType processType, [CallerMemberName] string callerMethodName = "")
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Whole, "処理" + processType + "処理を終了したメソッド" + callerMethodName);
        
        runningProcess.Remove(processType);

        if (runningProcess.Count == 0)
            BecomeCapableOfAction();

        TriggerEffectManager.instance.InvokeQueueTrigger();
    }

    /// <summary>
    /// 処理を実行中のプレイヤーを返す　※複数のプレイヤーが同時に処理をしている状況はないものとする
    /// </summary>
    /// <returns></returns>
    public Player GetPlayerRunningProcess()
    {
        foreach(KeyValuePair<RunningProcessType,RunningProcess> process in runningProcess)
        {
            if (process.Value.relatingPlayer == players[0]) return players[0];
            else if (process.Value.relatingPlayer == players[1]) return players[1];
        }
        return null;
    }

    public bool CanInvokeTriggerEffect()
    {
        int canInvokeCount = runningProcess.Count;
        foreach(KeyValuePair<RunningProcessType,RunningProcess> process in runningProcess)
        {
            if (process.Key.CanInvokeEffectDuringProcess())
            {
                canInvokeCount--;
            }
        }

        return canInvokeCount == 0;
    }

    /// <summary>
    /// アタックなどの処理を進めることができるか (アタック前の処理の塊の中などで見る。）
    /// </summary>
    /// <returns></returns>
    public bool CanProceedProcess()
    {
        return !runningProcess.ContainsKey(RunningProcessType.EffectProcess) && TriggerEffectManager.instance.GetInvokeTriggerQueue().Count == 0;
    }

    /// <summary>
    /// 行動不可能状態になる
    /// </summary>
    public void BecomeIncapableOfAction()
    {
        if (turnPlayer == localPlayer)
        {
            turnPlayer.handScr.RemoveHighlights();
            turnPlayer.fieldCards.RemoveHighlights();
            localPlayer.playerCanvas.cardInfoMenu.UpdateButtonState();
            //localPlayer.playerCanvas.HideActionPhaseGuide(); 現在下のsideSupportGuideの方で試験運用（どちらかでいい）

            //localPlayer.playerCanvas.sideSupportGuide.SetGuideText("");
        }
    }

    /// <summary>
    /// 行動可能状態になる
    /// </summary>
    public void BecomeCapableOfAction()
    {
        if (turnPlayer == localPlayer)
        {
            turnPlayer.handScr.HighlightAvailableHand();
            turnPlayer.fieldCards.HighlightAvailableCard();
            localPlayer.playerCanvas.cardInfoMenu.UpdateButtonState();
            //localPlayer.ViewCondition();
            StartCoroutine(localPlayer.DelayViewCondition());//warn:このままだとカード使用などの処理が終了するごとに、連続で処理が続く場合でもすぐにViewCondition()が呼ばれてしまうため一時的にこのDelay式にしているが、無理やりなので根本的解決が必要
        }
    }

    /// <summary>
    /// ゲームの状態が更新されたことを通知する
    /// </summary>
    public void UpdatedGameStatus()
    {
        Debug.Log("ゲームの状態が更新されました。送るメッセージ数:" + UpdatedGameNoticeReceiver.Count);
        foreach(IUpdatedGameStatus receiver in UpdatedGameNoticeReceiver)
        {
            receiver.UpdatedGameStatus();
        }
        localPlayer.playerCanvas.cardInfoMenu.UpdateButtonState();
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void EndGame()
    {
        matchBGM.StopBGM();
    }

    /// <summary>
    /// 現在そのプレイヤーのターンであるか確かめる
    /// </summary>
    /// <param name="pScr">確かめるプレイヤー</param>
    /// <returns></returns>
    public bool IsMyTurn(Player pScr)
    {
        return turnPlayer == pScr;
    }

    /// <summary>
    /// カードを生成したときにデータを保存し、同名のカードがあったとき名前がかぶらないよう番号を振る
    /// </summary>
    /// <param name="generaredCard"></param>
    /// <returns></returns>
    public int GeneratedCard(Card generaredCard)
    {
        int sameNameNum = 0;
        foreach(Card card in generatedCards)
        {
            if(card.cardData.cardName == generaredCard.cardData.cardName)
            {
                sameNameNum++;
            }
        }
        generatedCards.Add(generaredCard);
        return sameNameNum;
    }

    #region プレイヤーID変換機作ったけどよく考えたらプレイヤーIDは先攻後攻で決まっててローカルも相手も同じだからこれを使う意味がなくなった　よくわからんかったら消してOK
    ///// <summary>
    ///// 通信対戦用のプレイヤーIDを取得する
    ///// </summary>
    ///// <param name="player"></param>
    ///// <returns></returns>
    //public byte GetPlayerId(Player player)
    //{
    //    if(PhotonNetwork.IsMasterClient && player == localPlayer)
    //    {
    //        return 0;
    //    }
    //    else
    //    {
    //        return 1;
    //    }
    //}

    ///// <summary>
    ///// 通信対戦用のプレイヤーIDをもとにプレイヤーの実体を取得する
    ///// </summary>
    ///// <param name="playerId"></param>
    ///// <returns></returns>
    //public Player GetPlayerFromId(byte playerId)
    //{
    //    if(PhotonNetwork.IsMasterClient && playerId == 0)
    //    {
    //        return players[0];
    //    }
    //    else
    //    {
    //        return players[1];
    //    }
    //}
    #endregion

    public byte GetPlayerId(Player player)
    {
        if(player == players[0])
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    //af:今後MatchUIManagerをつくり、各playercanvasやこの処理を分離する

    /// <summary>
    /// UIオブジェクトをワールド座標に合わせて表示する
    /// </summary>
    /// <param name="uiObj"></param>
    /// <param name="worldPos"></param>
    /// <param name="parent"></param>
    /// <param name="camera"></param>
    public void ShowUIAtWorldPosPosition(GameObject uiObj,Vector3 worldPos,Transform parent,Camera camera)
    {
        GameObject generatedUI = Instantiate(uiObj, transform.position, Quaternion.identity);
        generatedUI.transform.SetParent(parent, false);

        // ワールド → スクリーン座標
        Vector3 screenPos = camera.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        parent.GetComponent<RectTransform>(),
        screenPos,
        null,
        out Vector2 anchoredPos
        );

        generatedUI.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
    }
}
