using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using TMPro;
using Newtonsoft.Json;

public class CardPointer : MonoBehaviour,IOwnerPlayer
{
    [Header("連携しているカメラ")] public Camera cameraScr;
    [Header("マウスポインタ―の場所を示すオブジェクト")] public MousePointerPosObj mousePointerPosObj;
    [Header("プレイヤー")] public Player pScr;
    [Header("ドラッガー使用可能")] public bool active;
    [Header("ガイドテキスト")] public TMP_Text guideText;
    private Plane plane;
    private bool isGrabbing;
    private Card grabbingCard;//ドラッグモードにおいて掴んでいるカード
    private SelectableTarget pointerOverTarget;//ポインターに重なっている選択可能なターゲット
    private Card pointerOverCard;//ポインターに重なっているカード
    private Card clickedCard;//最後にクリックされたときにポインターに重なっていたカード
    private SpriteRenderer cardRenderer;
    private Vector3 beforeDragPos;
    private int needTargetNum;
    [HideInInspector] public Attack attacker;
    private SelectType selectType;
    private List<Card> selectedTargetCards = new List<Card>();
    private List<Card> canSelectCards = new List<Card>(); //従来は最初選択可能だったカード全てを表していたが、既に選択されたカードは取り除くようにする。もしこれで不都合が生じたら戻す。
    private List<SelectableTarget> selectedTargets = new List<SelectableTarget>();
    private byte lockonNum;
    [HideInInspector] public bool searchWindowMode;//効果でサーチウィンドウを展開しているときのみ
    private Action<List<Card>> passTargetCards;
    private Action<List<SelectableTarget>> passTarget;
    private bool decideDoUseCard;
    private bool doUseCard;
    private bool paidUseCost;
    private bool exemptedFromUseRight;
    private bool decidedDoExemptFromUseRight;
    private bool allowBelowTargets;
    private string selectingUIText;
    public Image relatedCardImage;
    private Card relatedCard;
    public CustomButtonAC viewRelatedCardInfoButton;
    public bool isForcedSummon; //warn:ここは割と汎用的なクラスなのにカード使用限定のトグル作るのも流石に無理やりすぎるからいつか直そう

    //マルチプレイ同期関連
    public int nowSelectingId = 0; //このidはゲーム中に発生した選択処理全てに一意の値で割り当てられ、同じ選択はローカルクライアント、相手クライアントどちらも同じidが保証されなければならない。

    public enum PointerModes
    {
        dragCard,
        selectCard,
        cardInfo,
        inactive
    }
    public PointerModes pointerMode = PointerModes.cardInfo;

    public enum SelectType
    {
        none,
        ability,
        attack,
        noroi,
        equip,
        Advent
    }

    [HideInInspector] public Card viewingCard;

    // Start is called before the first frame update
    void Start()
    {
        plane = new Plane(Vector3.up, Vector3.up);
        viewRelatedCardInfoButton.onClickCallback = () => { ViewGuideCardInfo(); };
    }

    void Update()
    {
        //通常のポインターモードの制御
        float dKey = Input.GetAxisRaw("DKey");
        if (pointerMode == PointerModes.cardInfo || pointerMode == PointerModes.dragCard)
        {
            if (dKey > 0)
            {
                pointerMode = PointerModes.dragCard;
            }
            else
            {
                pointerMode = PointerModes.cardInfo;
            }
        }

        if (pScr.isLocalPlayer || MatchManager.instance.singleMode)
        {
            Ray ray = cameraScr.ScreenPointToRay(Input.mousePosition); //レイを設定する

            RaycastHit hit;
            int cardLayer = LayerMask.GetMask("Card");
            int coreLayer = LayerMask.GetMask("Core");
            int buttonLayer = LayerMask.GetMask("Button");

            //if (Physics.Raycast(ray, out hit, Mathf.Infinity,cardLayer) && !grabbingCard)//レイを飛ばす
            //{
            //Debug.Log(hit.collider.gameObject.GetComponent<SelectableTarget>());
            //}
            if (GameManager.instance.mouseRaycast.oldPointerOvered.Count == 0)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, cardLayer))//レイを飛ばす
                {
                    GetPointerOverTargetData(hit.collider.gameObject.GetComponent<SelectableTarget>());
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, coreLayer))
                {
                    GetPointerOverTargetData(hit.collider.gameObject.GetComponent<SelectableTarget>());
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, buttonLayer))
                {
                    pointerOverTarget = hit.collider.gameObject.GetComponent<SelectableTarget>();
                }
                else
                {
                    pointerOverCard = null;
                }
                if (Input.GetMouseButtonDown(0) && pointerOverCard)
                {
                    clickedCard = pointerOverCard;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    clickedCard = null;
                }
            }

            //【タイミングの条件】Dragモードであり、このスクリプトを所有するプレイヤーのターンであり、そのプレイヤーのアクションフェイズであるか、サーチウィンドウモードであり、処理中でない
            if ((pointerMode == PointerModes.cardInfo || pointerMode == PointerModes.dragCard) && ((MatchManager.instance.turnPlayer == pScr && MatchManager.instance.phaseManager.nowPhase == PhaseManager.TurnPhases.actionPhase && MatchManager.instance.runningProcess.Count == 0) || searchWindowMode))//pin
            {
                //クリックされたカードのオーナーが、このスクリプトのオーナーと一致したとき
                if (Input.GetMouseButtonDown(0) && pointerOverCard && pointerOverCard.controlerPlayer == pScr)
                {
                    if (!isGrabbing) //まだ掴んでいないとき
                    {
                        //【↑のパターン1】このスクリプトを所有するプレイヤーのターンであり、そのプレイヤーのアクションフェイズである warn:このアクションフェイズ条件発動タイミングによっては見直さないとじゃね
                        if (MatchManager.instance.turnPlayer == pScr && MatchManager.instance.phaseManager.nowPhase == PhaseManager.TurnPhases.actionPhase)
                        {
                            Debug.Log($"{clickedCard} テスト");
                            //フィールドに存在していてDragCardモードであるか、サーチウィンドウにある error
                            if ((clickedCard.cardState == Card.CardState.Field && pointerMode == PointerModes.dragCard) || (clickedCard.inSearchWindow == true))
                            {
                                CatchCard(clickedCard);
                            }
                            else if (clickedCard.cardState == Card.CardState.Field)
                            {
                                UtilityLog.CategorizedLog(UtilityLog.LogCategory.OtherSystem, "ドラッグモードにしてください");
                            }
                            //【カードの状態の条件】手札またはスペシャルデッキに存在して使用権が残っていて、使用コストがないか、コストを払える
                            if (clickedCard.CanNormalUseCheck())
                            {                                
                                 CatchCard(clickedCard);
                            }
                        }
                        //【↑のパターン2】サーチウィンドウ内である
                        else if (clickedCard.inSearchWindow) //searchWindowMode
                        {
                            CatchCard(clickedCard);
                        }
                    }
                }
                //掴み中
                if (isGrabbing)
                {
                    float rayDistance;
                    plane.Raycast(ray, out rayDistance); //レイを飛ばす

                    mousePointerPosObj.transform.position = ray.GetPoint(rayDistance); //マウスポインタ―のコライダー用オブジェクトをマウスの座標と合わせる

                    grabbingCard.transform.position = ray.GetPoint(rayDistance); //カードをマウスの場所に動かす

                    if (grabbingCard.GetCanSetCardArea()) //移動可能な場所にマウスがあるとき
                    {
                        cardRenderer.color = Color.white;
                    }
                    else
                    {
                        cardRenderer.color = Color.red;
                    }

                    if (Input.GetMouseButtonUp(0)) //マウスを離したとき
                    {
                        UtilityLog.CategorizedLog(UtilityLog.LogCategory.OtherSystem, "ドロップしました");
                        cardRenderer.color = Color.white; //カードの色を元に戻す
                        grabbingCard.caughtByMouse = false; //カードを掴まれていない状態とする
                        isGrabbing = false;
                        //設置可能な場合
                        if (grabbingCard.GetCanSetCardArea())
                        {
                            StartCoroutine(SetCard(grabbingCard, grabbingCard.transform.position));
                        }
                        //設置不可能な場合
                        else
                        {
                            CantSetCard();
                        }
                    }
                }
            }
        }

        if (pointerMode == PointerModes.selectCard)
        {
            //DisplayGuide(selectingUIText + " を選んでください  あと" + (needTargetNum - selectedTargets.Count) + "枚"); ここにあると表示を更新する処理が選択処理中毎フレーム呼ばれて流石にくそ


            //カーソルがあたっているカードの情報を表示する
            if (pointerOverCard && pointerOverCard.GetPublicArea(pScr) && !GameManager.instance.mouseRaycast.IsPointerOverBlockingUI())
                pScr.playerCanvas.personalUICanvas.ShowMiniCardInfo(pointerOverCard);

            if (Input.GetMouseButtonDown(0))
            {
                //カードスクリプトを持っていて、条件に当てはまり、まだ選択されていない
                if (pointerOverCard)
                {
                    if (canSelectCards.Contains(clickedCard) && !clickedCard.cardState.IsCardStateNeedSearchWindow())
                    {
                        MatchManager.instance.matchActionService.Submit(new SelectCardCommand(pScr, MatchManager.instance.GetCardId(clickedCard), nowSelectingId));
                        SelectedOneTargetCP();//warn:SelectedCard()内のSelectedOneTargetCP()がコメントアウトされている部分のコメント参照

                        //imp:旧通信＋ローカル処理
                        //SelectedCard(clickedCard);
                        ////選択されたカードidを相手クライアントに送る。
                        //if (pScr == MatchManager.instance.localPlayer)
                        //{
                        //    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"カード選択を同期します カードID:{MatchManager.instance.GetGameCards().IndexOf(clickedCard)}");
                        //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentSelectedCard), Photon.Pun.RpcTarget.Others, MatchManager.instance.GetGameCards().IndexOf(clickedCard), nowSelectingId);
                        //}

                        if (!searchWindowMode)
                        {

                        }
                        else if (clickedCard.inSearchWindow)
                        {
                            clickedCard.inSearchWindow = false;
                        }
                    }
                    else
                    {
                        Debug.Log("条件に当てはまりません");
                    }
                }
                else if (selectType == SelectType.attack && pointerOverTarget && pointerOverTarget.gameObject.tag == "Player")
                {
                    Debug.Log("アタックモードであり、プレイヤーが選択されました");
 
                    //選択を同期
                    MatchManager.instance.matchActionService.Submit(new SelectCardCommand(pScr,-1, nowSelectingId));
                    //imp:コマンド方式にする前の通信と実行処理
                    //SelectedOtherTarget(pointerOverTarget);
                    //if (pScr == MatchManager.instance.localPlayer)
                    //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentSelectedCard), Photon.Pun.RpcTarget.Others, -1, nowSelectingId);
                }
                else
                {
                    Debug.Log("有効なターゲットをクリックできていません");
                }
            }

            //必要なターゲットの数ターゲットが選択されたか、もう条件に当てはまるターゲットがいないか、モンスターが攻撃のリクエストをしていてプレイヤーがクリックされたとき
            if (selectedTargets.Count == needTargetNum)
            {
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "選ばれたターゲット数が必要数を満たしました");
                DecidedTargetsCP();
            }
            else if (selectType != SelectType.attack && selectType != SelectType.Advent && canSelectCards.Count == 0)
            {
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "もう選択可能なターゲットがいません");
                DecidedTargetsCP();
            }

        }
        //通常モード(カードをタップして情報を見れる
        if(pointerMode == PointerModes.cardInfo || true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (clickedCard)
                {
                    ViewCardInfo(clickedCard);
                }
                else
                {
                    if (!pScr.playerCanvas.canvasPointer.overlapMiniCardInfoBG)
                    {
                        pScr.playerCanvas.personalUICanvas.HideMiniCardInfo();
                    }
                }
            }
        }
    }

    #region SelectやDragの途中で呼び出されるメソッド
    /// <summary>
    /// カードが選択されたとき
    /// </summary>
    /// <param name="card"></param>
    public void SelectedCard(Card card)
    {
        selectedTargetCards.Add(card);
        selectedTargets.Add(card.selectableTarget);
        card.selectableTarget.LockOnThis();
        lockonNum++;
        canSelectCards.Remove(card);
        card.matchCardHighlight.HighlightBorderEnable(false);

        //SelectedOneTargetCP();//warn: 一時的にこの処理はSelectedCardから必ずくっついてではなく呼ぶべき処理の時だけ、SelectedCardを呼んだ次の処理にSelectedOneTargetCP()を呼ぶことにする。この対応は、新しい機能を次に作る前に最優先で行う。
        UtilityLog.CardLog(card.gameObject.name + " をロックオンしました", card, UtilityLog.LogCategory.Pointer);
    }

    /// <summary>
    /// カード以外のターゲットが選択されたとき
    /// </summary>
    /// <param name="selectableTarget"></param>
    public void SelectedOtherTarget(SelectableTarget selectableTarget)
    {
        selectedTargets.Add(selectableTarget);
        selectableTarget.LockOnThis();
        lockonNum++;

        SelectedOneTargetCP();
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, selectableTarget.gameObject.name + " をロックオンしました");
    }

    /// <summary>
    /// カードが1枚選ばれたときの共通処理 ※SelectedCard()とSelectdOtherTarget()からしか呼ばれない状態だけど他にカードが選ばれた場合に通るルートないよね？
    /// </summary>
    public void SelectedOneTargetCP()
    {
        //カード選択後もまだ選択処理が続く場合
        //af:これと似た条件チェックがUpdateにもあり、本来Updateで実行するよりここで統一して確認したほうがよいと考えられるが、共通化する場合やや手間がかかるので一旦分散させておく。
        if (selectedTargets.Count != needTargetNum && !(selectType != SelectType.attack && selectType != SelectType.Advent && canSelectCards.Count == 0))
        {
            //Debug.Log($"{selectedTargets.Count != needTargetNum},{!(selectType != SelectType.attack && selectType != SelectType.Advent && canSelectCards.Count == 0)} カードを１枚選択後、まだ選択が続きます");

            //この段階ですでにcardSelectCardsが行進されていて、selectTypeが初期化されていない必要があるので注意
            pScr.OnCardSelectionAvailable(canSelectCards, selectType);
        }

        DisplayGuide(selectingUIText + " を選んでください  あと" + (needTargetNum - selectedTargets.Count) + "枚");//warn:今までUpdate内に合った処理を急遽持ってきた。ここに置くべきものかはまだ検討してないので時間ができ次第、被害が出る前に検討すること。
    }

    /// <summary>
    /// カードを設置可能エリアにドラッグアンドドロップしたときの処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetCard(Card setCard,Vector3 setPos)
    {
        UtilityLog.CardLog("カードをセットします", setCard, UtilityLog.LogCategory.Pointer);
        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.UseCard, new RunningProcess(pScr));
        setCard.standbySet = true;

        //このドロップが場に出す時であるか否か
        bool isUse = setCard.cardState != Card.CardState.Field;
        if (!isUse)
        {
            setCard.transform.position = new Vector3(setPos.x, 0.1f, setPos.z); //カードを降ろす
            setCard.standbySet = false;//af:今後standbySetをfalseにするとかUseCardをコンプするとかそこら辺の終了処理をまとめたほうがいいかも
            MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.UseCard);
        }
        else
        {
            //前回の結果が残ってしまっている為
            exemptedFromUseRight = false;
            //サーチウィンドウ(効果召喚)なら無条件で召喚権不要
            if (!searchWindowMode && setCard.costForExemptionFromUseRight && setCard.costForExemptionFromUseRight.CanUseCheck())
            {
                if (setCard.controlerPlayer.remainingSummonRights >= 1)
                {
                    pScr.yon.ChooseStart(ExemptedUseRightYes, ExemptedUseRightNo, "召喚権無しで出しますか？",false,true);

                    while (!decidedDoExemptFromUseRight)
                    {
                        yield return null;
                    }
                    decidedDoExemptFromUseRight = false;
                }
                else
                {
                    exemptedFromUseRight = true;
                }

                if (exemptedFromUseRight)
                {
                    UtilityLog.CardLog("召喚権なしで出します", setCard, UtilityLog.LogCategory.Pointer);
                    StartCoroutine(setCard.PayExemptionFromUseRightCost(PaidSummonCosts));

                    while (!paidUseCost)
                    {
                        yield return null;
                    }
                    paidUseCost = false;
                }
                else
                {
                    UtilityLog.CardLog("召喚権ありで出します", setCard, UtilityLog.LogCategory.Pointer);
                }
            }
            //場に出すコストが必要な場合
            if (!isForcedSummon && (setCard.useCardCost || setCard.evolutionNugget))
            {
                UtilityLog.CardLog("場に出すコストが必要です", setCard, UtilityLog.LogCategory.Pointer);

                UtilityLog.CardLog("場に出すコストを払うか決めます", setCard, UtilityLog.LogCategory.Pointer);

                pScr.yon.ChooseStart(DoSummonYes, DoSummonNo,"場に出すコストを払いますか？",true,false);

                while (!decideDoUseCard)
                {
                    yield return null;
                }
                decideDoUseCard = false;

                if (doUseCard)
                {
                    UtilityLog.CardLog("場に出すコストを払います", setCard, UtilityLog.LogCategory.Pointer);
                    StartCoroutine(setCard.PayUseCost(PaidSummonCosts));
                }
                else
                {
                    UtilityLog.CardLog("場に出すコストを払わず、召喚をやめました", setCard, UtilityLog.LogCategory.Pointer);
                    setCard.standbySet = false;//na
                    MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.UseCard);
                    CantSetCard();
                }
            }
            else
            {
                UtilityLog.CardLog("場に出すコストは不要です", setCard, UtilityLog.LogCategory.Pointer);
                paidUseCost = true;
                doUseCard = true;
            }

            //場に出す場合(場に出すコストが不要な場合or場に出すコストが必要であり、コストを払うと選択したとき)
            if (doUseCard || isForcedSummon)
            {
                isForcedSummon = false;
                doUseCard = false;

                while (!paidUseCost)
                {
                    yield return null;
                }
                paidUseCost = false;
                UtilityLog.CardLog(setCard.cardState + "から出す", setCard, UtilityLog.LogCategory.Pointer);
                //カードの効果を発動する
                //召喚する場合

                setCard.transform.position = new Vector3(setPos.x, 0.1f, setPos.z); //カードを降ろす                
                
                setCard.transform.hasChanged = false;
                
                if (isUse)
                {
                    UtilityLog.CardLog("場に出します", setCard, UtilityLog.LogCategory.Pointer);
                    setCard.standbySet = false;
                    
                    setCard.AppearCard(searchWindowMode || exemptedFromUseRight);
                    //進化モンスターの召喚後位置がリセットされないようもう一度レイヤーを再配置する 本当は進化で重ねる処理は召喚処理後にするべき　むずい
                    setCard.cardLayer.ArrangementLayer();

                    //固有効果(召喚したモンスターになにかを付与するためにターゲットを渡すとき) ※この処理はいらん可能性あり（現状はいらんけど一応残しとく）
                    if (EffectManager.instance.uniqueEffectNugget)
                    {
                        IPassTarget iPT = EffectManager.instance.uniqueEffectNugget.GetComponent(typeof(IPassTarget)) as IPassTarget;
                        
                        List<Card> cards = new List<Card>();
                        cards.Add(setCard);
                        iPT.PassTargets(cards);
                    }
                    MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.UseCard);

                    #region カード使用Commandが完全には正式なCommandになれていないので応急処置。それが解決したらすぐこのregionを消し、その他微調整頑張って。とりあえず正式なコマンドになれば余計にRecord処理も別で書かなくていいので単純にコマンドを適切に生成とSubmitさえできていれば大丈夫なはず
                    PlayerCommand generatedUseCardCommand;

                    if (!searchWindowMode)
                    {
                        generatedUseCardCommand = new UseCardNormalCommand(pScr, MatchManager.instance.GetGameCards().IndexOf(setCard), setPos);
                    }
                    else
                    {
                        generatedUseCardCommand = new UsedSearchCardCommand(pScr, MatchManager.instance.GetGameCards().IndexOf(setCard), setPos, pScr.searchCard.searchingId);
                    }
                    //af:warn:極めて強引な実装です。どうせカード使用のコマンドを正式なコマンドにすれば要らなくなるからこの設計に悩んだところでしょうがないなと思ったので一番楽な処置をとります。とにかく、カード使用コマンドを正式なコマンドにできたらもちろんこのifも消す
                    if (!pScr.isCpu)
                        MatchManager.instance.matchActionService.TempRecord(generatedUseCardCommand);
                    #endregion

                    //カードセットを同期する処理 af:isSoloModeの条件はカード使用処理の完全コマンド化がまだ完了していないため。それが完了したらisSoloModeの条件は不要
                    if (pScr == MatchManager.instance.localPlayer && !MatchManager.instance.isSoloMode)
                    {
                        if (!searchWindowMode)
                        {
                            UtilityLog.CardLog("通常使用を同期 カードID:" + MatchManager.instance.GetGameCards().IndexOf(setCard),setCard, UtilityLog.LogCategory.Network);
                            //MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentUseCardNormal), Photon.Pun.RpcTarget.Others, MatchManager.instance.GetGameCards().IndexOf(setCard), setPos);
                            string commandJson = CommandSerializer.Serialize(generatedUseCardCommand);
                            Debug.Log($"通常使用で送信するJson:{commandJson}");

                            MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.ReceiveOpponentCommand), Photon.Pun.RpcTarget.Others, commandJson);
                        }
                        else
                        {
                            UtilityLog.CardLog("効果使用を同期 カードID:" + MatchManager.instance.GetGameCards().IndexOf(setCard), setCard, UtilityLog.LogCategory.Network);

                            string commandJson = CommandSerializer.Serialize(generatedUseCardCommand);
                            Debug.Log($"効果使用で送信するJson:{commandJson}");

                            MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.ReceiveOpponentCommand), Photon.Pun.RpcTarget.Others, commandJson);

                            //imp:旧通信処理
                            //MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentUsedSearchCard), Photon.Pun.RpcTarget.Others, MatchManager.instance.GetGameCards().IndexOf(setCard), setPos,pScr.searchCard.searchingId);
                        }
                    }

                    if (searchWindowMode)
                    {
                        setCard.controlerPlayer.searchCard.gameObject.SetActive(true);
                        //この処理が同期処理より前じゃないとsearchWindowModeがfalseになっちまう←なんの同期処理？
                        setCard.controlerPlayer.searchCard.Summoned(setCard);
                    }
                    else
                    {

                    }
                }
            }
        }
        
        setCard = null; //掴んでいない状態とする
    }

    /// <summary>
    /// モンスターを召喚するかの選択の結果を受け取る
    /// </summary>
    /// <param name="result"></param>
    public void DoSummonYes()
    {
        Debug.Log("(YES)モンスターを召喚する");
        decideDoUseCard = true;
        doUseCard = true;
    }

    public void DoSummonNo()
    {
        Debug.Log("(NO)モンスターを召喚しない");
        decideDoUseCard = true;
        doUseCard = false;
    }

    public void ExemptedUseRightYes()
    {
        decidedDoExemptFromUseRight = true;
        exemptedFromUseRight = true;
    }

    public void ExemptedUseRightNo()
    {
        decidedDoExemptFromUseRight = true;
        exemptedFromUseRight = false;
    }

    /// <summary>
    /// モンスター召喚コストを払い終わった時に呼び出される
    /// </summary>
    private void PaidSummonCosts()
    {
        paidUseCost = true;
        Debug.Log("召喚コストが払い終わった信号を受け取りました");
    }

    private void CantSetCard()
    {
        Debug.Log("設置不可能です");
        //フィールド内で移動していた場合
        if (grabbingCard.cardState == Card.CardState.Field)
        {
            //ドラッグする前の場所に戻す
            grabbingCard.transform.position = beforeDragPos;
        }

        //元の場所に戻す
        switch (grabbingCard.cardState)
        {
            case Card.CardState.Deck:
                grabbingCard.controlerPlayer.deck.ArrangementDeck();
                break;

            case Card.CardState.Cemetery:
                grabbingCard.controlerPlayer.cemeteryScr.ArrangementCemetery();
                break;

            case Card.CardState.Hand:
                grabbingCard.controlerPlayer.handScr.SortHands();
                break;

            case Card.CardState.SpecialDeck:
                grabbingCard.controlerPlayer.specialDeck.ArrangementDeck();
                break;
        }

        //サーチウィンドウからドラッグしていた場合
        if (searchWindowMode)
        {            
            Debug.Log("サーチウィンドウを再展開します");
            pScr.searchCard.ReOpenWindow();
            
        }
        grabbingCard = null;
    }

    /// <summary>
    /// ターゲットが決まり、ターゲットを渡してSelect処理を終了する共通処理
    /// </summary> 
    public void DecidedTargetsCP()
    {
        //選ばれたカードを列挙し、処理する
        foreach (Card target in selectedTargetCards)
        {
            target.selectableTarget.targeted = false;
            UtilityLog.CardLog("今回のSelect処理で選ばれたカード", target, UtilityLog.LogCategory.Pointer);
        }

        //できればpassTargetの型を統一してわざわざ分岐なんてしなくていいようにしたいんだけど余裕がないからそれはあとで任せる。もしそれが実現したらpassTargetだけローカルに保存してやる。
        //値をローカルに保存
        SelectType tempSelectType = selectType;
        Action<List<Card>> tempPassTargetCards = passTargetCards;
        Action<List<SelectableTarget>> tempPassTarget = passTarget;
        List<Card> tempSelectedTargetCards = selectedTargetCards;
        List<SelectableTarget> tempSelectedTargets = selectedTargets;

        EndSelect();

        //呼び出し元に選ばれたターゲットを返す
        if (tempSelectType != SelectType.Advent && tempSelectType != SelectType.attack)
        {
            tempPassTargetCards(tempSelectedTargetCards);
        }
        else
        {
            tempPassTarget(tempSelectedTargets);
        }
        //ここで確実にSelect処理が終了
    }

    /// <summary>
    /// ターゲット決定ボタンが押された※ref+1 このメソッド内の処理は通信の受信側では呼ばれない。呼ばれないと困るシステム処理はここに置いてはならず、DecidedTargetsCP()に入れること
    /// </summary>
    public void DecideTargetButton()
    {
        MatchManager.instance.matchActionService.Submit(new SelectCardCommand(pScr,-3, nowSelectingId));
        SelectedOneTargetCP();//warn:SelectedCard()内のSelectedOneTargetCP()がコメントアウトされている部分のコメント参照 ※今までと同じ挙動を担保するためにこの処理を置いたが、ターゲット決定ボタンの後ってこの処理不要なのでは？（推測）
        //imp:旧通信実行処理
        //DecidedTargetsCP();
        //if (pScr == MatchManager.instance.localPlayer)
        //{
        //    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"ターゲット決定を同期します");
        //    //MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentClickedDecideTargetButton), Photon.Pun.RpcTarget.Others);
        //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentSelectedCard), Photon.Pun.RpcTarget.Others, -3, nowSelectingId);
        //}
    }

    /// <summary>
    /// SelectCard処理を終了する
    /// </summary>
    private void EndSelect()
    {
        pointerMode = PointerModes.cardInfo;
        HideGuide();
        relatedCardImage.sprite = null;
        relatedCard = null;
        lockonNum = 0;

        //セレクトフレームを非表示にする
        foreach(Card card in canSelectCards)
        {
            card.matchCardHighlight.HighlightBorderEnable(false);
        }

        //ターゲット決定ボタンを非表示にする
        if (allowBelowTargets)
        {
            pScr.playerCanvas.targetsDecideButton.gameObject.SetActive(false);
            allowBelowTargets = false;
        }

        //selectType毎の初期化処理
        switch (selectType)
        {
            case SelectType.attack:
                pScr.playerCanvas.directAttackIcon.gameObject.SetActive(false);
                break;

            case SelectType.Advent:
                pScr.playerCanvas.lifeSacrificeButton.gameObject.SetActive(false);
                break;
        }

        //サーチウィンドウを展開していた場合
        if (searchWindowMode)
        {
            //あんま細かいこと覚えてないけど修正しちゃったからもしこのせいでサーチウィンドウがうまく動かんかったら元に戻そう
            pScr.searchCard.EndSearch();
            searchWindowMode = false;
        }
        attacker = null;
        needTargetNum = 0;
        //ターゲットのロックオン状態を元に戻す
        foreach (SelectableTarget selectableTarget in selectedTargets)
        {
            selectableTarget.UnLockOnThis();
        }
        //初期化
        selectedTargets = new List<SelectableTarget>();
        selectedTargetCards = new List<Card>();
        canSelectCards = new List<Card>();

        //imp:opponentSelectedGameCardIdsじゃなくてselectCardCommandsQueueに変わったのは前提としてもそもそもこれ必要だった？
        //終了した選択処理の選択セットを削除する
        //if(!MatchManager.instance.isSoloMode)
        //    MatchManager.instance.networkPlayer.opponentSelectedGameCardIds.Remove(nowSelectingId);

        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.Select);
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer,"Selectプロセスが終了しました");
    }

    #endregion

    #region Select開始・リセットメソッド
    /// <summary>
    /// 効果、進化素材、呪いの代償、装備カードなどを選ぶ（アタックや降臨を除く、特殊な処理が必要ない選択処理）
    /// </summary>
    /// <param name="effect">効果を使用したカード</param>
    /// <param name="tNum">対象の数</param>
    /// <param name="cardState">カードの属する場所</param>
    /// <param name="cardType">カードの種類</param>
    /// <param name="targetOwners">対象のカードの持ち主</param>
    public void SelectCard(int tNum, bool allowBellowTargets, List<Card> cards, SelectType thisSelectType, Action<List<Card>> whereToPassTarget,string selectText)
    {
        SelectCardCP(tNum,allowBellowTargets, cards, thisSelectType, whereToPassTarget,selectText);
        SearchProcess(cards,selectText);
    }

    /// <summary>
    /// アタックの対象を選ぶ
    /// </summary>
    /// <param name="thisAttacker"></param>
    /// <param name="tNum"></param>
    /// <param name="cardState"></param>
    /// <param name="cardType"></param>
    /// <param name="targetOwners"></param>
    public void SelectAttackCard(Attack thisAttacker, int tNum, List<Card> cards, Action<List<SelectableTarget>> whereToPassTarget)
    {
        attacker = thisAttacker;
        //アタックでのターゲット選択であるという情報を足す(アタックされないモンスターを選択可能ターゲットから除外するのに必要)←既に渡されたcardsの時点で除外されているのでは？このメモの意図がわからないが昔のものなのだろうか
        passTarget = whereToPassTarget;
        pScr.playerCanvas.directAttackIcon.gameObject.SetActive(true);
        SelectCardCP(tNum,false, cards, SelectType.attack, null,"アタック対象");

        if(cards.Count >= 1)
            pScr.playerCanvas.sideSupportGuide.SetGuideText("<b>アタック</b>する対象を選ぼう。相手モンスターにアタックすれば破壊でき、画面上部のハート形のボタンを押せば相手ライフにアタックしてダメージを与えることができる。");//warn:SelectCardCPから呼んで表示するガイドをここで上書きするとかいうまじでゴリ押しなので、早めに改善せよ

    }

    /// <summary>
    /// 降臨の生贄を選ぶ
    /// </summary>
    /// <param name="thisAttacker"></param>
    /// <param name="tNum"></param>
    /// <param name="thisConditionsNugget"></param>
    /// <param name="whereToPassTarget"></param>
    public void SelectAdventCost(int tNum, List<Card> cards, Action<List<SelectableTarget>> whereToPassTarget)
    {
        passTarget = whereToPassTarget;
        pScr.playerCanvas.lifeSacrificeButton.gameObject.SetActive(true);
        SelectCardCP(tNum,false, cards, SelectType.Advent, null,"降臨の生贄");
    }

    /// <summary>
    /// SelectCard処理の共通処理
    /// </summary>
    /// <param name="tNum"></param>
    /// <param name="cardState"></param>
    /// <param name="cardType"></param>
    /// <param name="targetOwners"></param>
    /// <param name="thisSelectType"></param>
    public void SelectCardCP(int tNum,bool thisAllowBelowTargets, List<Card> cards,  SelectType thisSelectType, Action<List<Card>> whereToPassTarget,string selectText)
    {
        pointerMode = PointerModes.selectCard;       
        needTargetNum = tNum;
        canSelectCards = cards.ToList();
        allowBelowTargets = thisAllowBelowTargets;
        selectType = thisSelectType;
        passTargetCards = whereToPassTarget;
        lockonNum = 0;
        selectingUIText = selectText;
        nowSelectingId++;

        DisplayGuide(selectingUIText + " を選んでください  あと" + (needTargetNum - selectedTargets.Count) + "枚");//warn:今までUpdate内に合った処理を急遽持ってきた。ここに置くべきものかはまだ検討してないので時間ができ次第、被害が出る前に検討すること。

        //warn:Count== 0だと選択処理はいらないからこの条件付けたけど適当だし適切じゃないかもしれない。あとでちゃんと設計考えて
        if(cards.Count >= 1)
            pScr.playerCanvas.sideSupportGuide.SetGuideText("対象のカードを選択しよう。<color=yellow>黄色くハイライトされたカード</color>を<b>クリック</b>すると選択できる。カードの情報を見たい場合は、そのカードに<b>カーソルを合わせる</b>だけでOK。");

        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.Select, new RunningProcess(pScr));
        //選択可能なターゲットを残し決定しても良い効果の場合
        if (allowBelowTargets)
        {
            pScr.playerCanvas.targetsDecideButton.gameObject.SetActive(true);
        }

        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "Selectプロセスを開始");

        foreach(Card card in canSelectCards)
        {
            card.matchCardHighlight.HighlightBorderEnable(true);
            card.matchCardHighlight.ChangeHighlightBorderColor(new Color(1, 0.97f, 0.1f, 0.85f));
        }

        //選択が不要な場合（ターゲット数任意でなく、全て対象にでき、選択肢がなく、選択可能なカードが存在する or アタッカーが存在(モンスターがアタックするselectである)し、相手の場にモンスターがいない）af:直接攻撃時の選択不要処理の条件式が二度手間なので後で省くかも&もしかしたらこの処理はクリックで選択されるたびに行うようにするかもattacker.player.opponentPlayer.fieldCards.GetFieldMonsters().Count == 0
        if ((!allowBelowTargets && !ExistChoices() && canSelectCards.Count != 0 && !attacker) || (attacker && canSelectCards.Count == 0))
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "対象が少ないため自動選択をします");
            if (attacker && canSelectCards.Count == 0)
            {
                SelectedOtherTarget(attacker.player.opponentPlayer.selectableTarget);
            }
            else if(selectType == SelectType.Advent)
            {

            }
            else
            {
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "カードを自動選択します");
                for (int i= canSelectCards.Count - 1; i >= 0;i--)
                {
                    Card autoSelectedCard = canSelectCards[i];
                    UtilityLog.CardLog("自動選択するカード1枚", autoSelectedCard, UtilityLog.LogCategory.Pointer);
                    if (!searchWindowMode)
                    {
                        SelectedCard(autoSelectedCard);
                        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "カードを自動選択しました");
                    }
                    else if (autoSelectedCard.inSearchWindow)
                    {
                        autoSelectedCard.inSearchWindow = false;
                        SelectedCard(autoSelectedCard);
                        UtilityLog.CardLog("サーチウィンドウ内のカードを自動選択しました", autoSelectedCard, UtilityLog.LogCategory.Pointer);
                    }
                }

                SelectedOneTargetCP();//warn:SelectedCard()内のSelectedOneTargetCP()がコメントアウトされている部分のコメント参照
            }
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "自動選択処理を終了しました");
        }
        else
        {
            //この段階でcanSelectCardsとselectTypeが既にセットされている必要があることに注意
            pScr.OnCardSelectionAvailable(canSelectCards, selectType);

            Debug.Log("自動選択は行われませんでした。cause:" + $"{!allowBelowTargets},{!ExistChoices()},{canSelectCards.Count != 0},{!attacker}");
        }
    }

    /// <summary>
    /// ターゲットの場所がデッキ又は墓地だった場合にサーチウィンドウでターゲットを表示するための処理
    /// </summary>
    /// <param name="thisConditionsNugget"></param>
    /// <param name="tNum"></param>
    /// <param name="topNum"></param>
    public void SearchProcess(List<Card> cards,string selectText)
    {
        //ターゲットの場所の条件がデッキ又は墓地だった場合、サーチウィンドウを展開して選択する
        foreach(Card card in cards)
        {
            bool doBreak = false; //foreachを止めるか判断するためのフラグ。switch内からforeachをbreakすることができない為。
            switch (card.cardState)
            {
                case Card.CardState.Deck:
                case Card.CardState.SpecialDeck:
                case Card.CardState.Cemetery:
                case Card.CardState.Piled:
                    searchWindowMode = true;
                    pScr.searchCard.searchMode = SearchCard.SearchMode.EffectOrSelectProcess;
                    pScr.searchCard.gameObject.SetActive(true); //サーチウィンドウをアクティブにする            
                    pScr.searchCard.Search(cards, needTargetNum,selectText); //サーチウィンドウで対象カードをサーチする
                    doBreak = true;
                    break;
            }

            if (doBreak)
            {
                break;
            }
        }
    }


    #endregion

    
    /// <summary>
    /// 選択肢があるか調べる（選択可能なカードが必要選択数以下であるか)
    /// </summary>
    /// <returns>選択可能なカードが必要選択数以下であるか否か</returns>
    public bool ExistChoices()
    {
        //多分選択開始時以外に実行したら正しい結果にならない
        if (canSelectCards.Count > needTargetNum - selectedTargetCards.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GetPointerOverTargetData(SelectableTarget thisTarget)
    {
        pointerOverTarget = thisTarget; //レイが当たったカードを保存
        pointerOverCard = thisTarget.GetComponent<Card>(); //カードのスクリプトを保存
    }

    public Player GetPlayer()
    {
        return pScr;
    }

    public void CatchCard(Card card)
    {
        UtilityLog.CardLog("カードをキャッチする", card, UtilityLog.LogCategory.Pointer);
        isGrabbing = true;
        grabbingCard = card;//掴む
        grabbingCard.caughtByMouse = true;
        cardRenderer = card.GetComponent<SpriteRenderer>();
        beforeDragPos = card.transform.position;
        card.ResetCardLook();
    }

    /// <summary>
    /// 降臨コスト処理において、ライフコストが選ばれたときに呼ばれる ref+
    /// </summary>
    public void SelectedLifeCost()
    {
        Debug.Log("ライフコストが選ばれました");

        MatchManager.instance.matchActionService.Submit(new SelectCardCommand(pScr, -2, nowSelectingId));
        SelectedOneTargetCP();//warn:SelectedCard()内のSelectedOneTargetCP()がコメントアウトされている部分のコメント参照
        //imp:旧通信実行処理
        //SelectedOtherTarget(pScr.selectableTarget);
        ////選択を同期
        //if (pScr == MatchManager.instance.localPlayer)
        //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentSelectedCard), Photon.Pun.RpcTarget.Others, -2, nowSelectingId);
    }

    /// <summary>
    /// 相手プレイヤーに直接アタック ref+
    /// </summary>
    public void DirectAttack()
    {
        //mnp:ライフコストって何言ってんだこれ
        Debug.Log("ライフコストが選ばれました");

        MatchManager.instance.matchActionService.Submit(new SelectCardCommand(pScr, -1, nowSelectingId));
        SelectedOneTargetCP(); //warn:SelectedCard()内のSelectedOneTargetCP()がコメントアウトされている部分のコメント参照
        //imp:旧通信実行処理
        //SelectedOtherTarget(pScr.opponentPlayer.selectableTarget);
        ////選択を同期
        //if (pScr == MatchManager.instance.localPlayer)
        //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentSelectedCard), Photon.Pun.RpcTarget.Others, -1, nowSelectingId);
    }

    /// <summary>
    /// 特定のカードのMiniCardInfoCanvasを表示する
    /// </summary>
    public void ViewCardInfo(Card viewCard)
    {
        if(viewCard.GetPublicArea(pScr))
            pScr.playerCanvas.personalUICanvas.ShowMiniCardInfo(viewCard);

        //フィールドに存在する自分がコントロールするカード
        if (viewCard && viewCard.cardState == Card.CardState.Field && viewCard.controlerPlayer == pScr)
        {
            //閲覧状態のカードを切り替える
            if (viewingCard)
            {
                viewingCard.viewed = false;
                viewingCard = null;
            }
            viewCard.viewed = true;
            viewingCard = viewCard;

            //カードのメニューを開く
            viewingCard.cardTypeScr.OpenMenu();
        }
        else if (viewingCard && !pointerOverTarget)
        {

        }
    }

    /// <summary>
    /// ガイドを表示する
    /// </summary>
    public void DisplayGuide(string text)
    {
        UtilityLog.DebugLog("お");
        //もしテキストの上に他の親を追加するなら変更しなきゃ
        guideText.transform.parent.gameObject.SetActive(true);
        guideText.text = text;
    }

    /// <summary>
    /// ガイドを隠す
    /// </summary>
    public void HideGuide()
    {
        guideText.text = "";
        guideText.transform.parent.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.66f);
        guideText.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// ガイドに選択の関連カードを表示する
    /// </summary>
    public void SetCardImage(Card imageCard)
    {
        relatedCardImage.sprite = imageCard.cardData.sprite;
        relatedCard = imageCard;
    }

    /// <summary>
    /// ガイドに表示されたのカード情報を表示する ref+1
    /// </summary>
    public void ViewGuideCardInfo()
    {
        ViewCardInfo(relatedCard);
    }


    public void SetCardTemp(Card setCard, Vector3 setPos)
    {
        StartCoroutine(SetCard(setCard, setPos));
    }
}
