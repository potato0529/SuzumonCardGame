//using AutoGroupGenerator;
using NUnit.Framework;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    [Header("相手のアクションのキュー")] public List<PlayerAction> opponentActionsQueue = new List<PlayerAction>();
    [Header("相手のYesNoの決定キュー")] public List<bool> opponentYesNoDecision = new List<bool>();
    [Header("相手の効果選択決定キュー")] public List<int> opponentEffectChoices = new List<int>();
    [Header("相手の選択したカードキュー")] public Dictionary<int, List<int>> opponentSelectedGameCardIds = new Dictionary<int, List<int>>();
    [Header("相手の選択したサーチカードキュー")] public Dictionary<int, List<int>> opponentSelectedSearchCardIds = new Dictionary<int, List<int>>();
    [Header("相手がサーチウィンドウから使用したカードキュー")] public Dictionary<int, List<PlayerAction>> opponentUsedSearchCards = new Dictionary<int, List<PlayerAction>>();

    [Header("相手のコマンドのキュー")] public List<PlayerCommand> opponentCommandsQueue = new List<PlayerCommand>();
    [Header("選択コマンドキュー")] public List<SelectCardCommand> selectCardCommandsQueue = new List<SelectCardCommand>();
    [Header("選択サーチカードコマンドキュー")] public List<SelectSearchCardCommand> selectSearchCardCommandsQueue = new List<SelectSearchCardCommand>();
    [Header("サーチウィンドウ使用コマンドキュー")] public List<UsedSearchCardCommand> usedSearchCardCommandsQueue = new List<UsedSearchCardCommand>();
    [Header("YesNo決定キュー")] public List<YesNoCommand> yesNoCommandsQueue = new List<YesNoCommand>();
    [Header("効果選択決定キュー")] public List<ChoseEffectCommand> choseEffectCommands = new List<ChoseEffectCommand>();

    public Player opponentPlayer
    {
        get;
        set;
    }

    private void Awake()
    {
        MatchManager.instance.networkPlayer = this;
        opponentPlayer = MatchManager.instance.localPlayer.opponentPlayer;
    }

    private void Update()
    {
        //このキュー処理は、相手のコマンドを受信する側が同期する際タイミングを合わせる（処理中なら重複処理しないよう待つ）ためであり、受信時のみ通る処理である
        if(MatchManager.instance.runningProcess.Count == 0)
        {
            if (opponentActionsQueue.Count > 0)
            {
                PlayerAction nowAction = opponentActionsQueue[0];
                switch (nowAction.actionType)
                {
                    case PlayerAction.OpponentActionType.UseCardNormal:
                        UtilityLog.CardLog("通信により相手が通常使用を行う", MatchManager.instance.GetGameCards()[nowAction.targetGameCardId], UtilityLog.LogCategory.Network);
                        StartCoroutine(opponentPlayer.cardPointer.SetCard(MatchManager.instance.GetGameCards()[nowAction.targetGameCardId], nowAction.setCardPos));
                        break;

                    case PlayerAction.OpponentActionType.ClickedPhaseButton:
                        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "通信により相手がアクションフェイズを終了する");
                        opponentPlayer.PhaseButton();
                        break;

                    case PlayerAction.OpponentActionType.StartAttack:
                        UtilityLog.CardLog("通信により相手が攻撃宣言をする", MatchManager.instance.GetGameCards()[nowAction.targetGameCardId], UtilityLog.LogCategory.Network);
                        MatchManager.instance.GetGameCards()[nowAction.targetGameCardId].monsterScr.AttackButton();
                        break;

                    case PlayerAction.OpponentActionType.EffectButton:
                        UtilityLog.CardLog("通信により相手が起動効果を発動する", MatchManager.instance.GetGameCards()[nowAction.targetGameCardId], UtilityLog.LogCategory.Network);
                        MatchManager.instance.GetGameCards()[nowAction.targetGameCardId].EffectButton();
                        break;

                    case PlayerAction.OpponentActionType.Advent:
                        UtilityLog.CardLog("通信により相手が降臨処理を開始する", MatchManager.instance.GetGameCards()[nowAction.targetGameCardId], UtilityLog.LogCategory.Network);
                        MatchManager.instance.GetGameCards()[nowAction.targetGameCardId].monsterScr.adventMonster.StartAdvent();
                        break;

                    case PlayerAction.OpponentActionType.Defence:
                        UtilityLog.CardLog("通信により相手がディフェンスをする", MatchManager.instance.GetGameCards()[nowAction.targetGameCardId], UtilityLog.LogCategory.Network);
                        MatchManager.instance.GetGameCards()[nowAction.targetGameCardId].monsterScr.DefenceButton();
                        break;
                }

                opponentActionsQueue.RemoveAt(0);
            }

            //新command通信システム
            if(opponentCommandsQueue.Count > 0)
            {
                PlayerCommand nowCommand = opponentCommandsQueue[0];

                Debug.Log("待機列のコマンドを実行します");
                MatchManager.instance.matchActionService.Dispatch(nowCommand);

                opponentCommandsQueue.RemoveAt(0);
            }
        }

        //カード選択キューの処理（新）
        if (selectCardCommandsQueue.Count > 0 && selectCardCommandsQueue[0].selectingId == MatchManager.instance.players[selectCardCommandsQueue[0].player].cardPointer.nowSelectingId)
        {
            SelectCardCommand nowSelectCardCommand = selectCardCommandsQueue[0];

            MatchManager.instance.matchActionService.Dispatch(nowSelectCardCommand);

            selectCardCommandsQueue.RemoveAt(0);
        }

        //相手のカード選択に関する処理(旧)
        //if (opponentPlayer && opponentPlayer.cardPointer.pointerMode == CardPointer.PointerModes.selectCard && opponentSelectedGameCardIds.ContainsKey(opponentPlayer.cardPointer.nowSelectingId))
        //{
        //    foreach(int selectedGameCardId in opponentSelectedGameCardIds[opponentPlayer.cardPointer.nowSelectingId])
        //    {
        //        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "通信により相手のカード選択を反映 カードID:" + selectedGameCardId);
        //        if (selectedGameCardId >= 0)
        //        {
        //            Debug.Log($"どうも{selectedGameCardId},{opponentSelectedGameCardIds[opponentPlayer.cardPointer.nowSelectingId][0]},{opponentPlayer.cardPointer.nowSelectingId},{opponentSelectedGameCardIds[opponentPlayer.cardPointer.nowSelectingId].Count}");
        //            opponentPlayer.cardPointer.SelectedCard(MatchManager.instance.GetGameCards()[selectedGameCardId]);
        //        }
        //        else if(selectedGameCardId == -1)
        //        {
        //            opponentPlayer.cardPointer.SelectedOtherTarget(opponentPlayer.opponentPlayer.selectableTarget);
        //        }
        //        else if(selectedGameCardId == -2)
        //        {
        //            opponentPlayer.cardPointer.SelectedOtherTarget(opponentPlayer.selectableTarget);
        //        }
        //        else if(selectedGameCardId == -3)
        //        {
        //            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のターゲット決定ボタンクリックを受け取ります");
        //            //これoP.cP.DecidedTargetsCP直で呼べばいいのでは
        //            opponentPlayer.cardPointer.DecideTargetButton();
        //        }
        //    }
        //    opponentSelectedGameCardIds[opponentPlayer.cardPointer.nowSelectingId].Clear();
        //}

        //相手のサーチカード選択に関する処理(新)
        if(selectSearchCardCommandsQueue.Count > 0 &&  MatchManager.instance.players[selectSearchCardCommandsQueue[0].player].searchCard.isActiveAndEnabled && selectSearchCardCommandsQueue[0].searchingId == MatchManager.instance.players[selectSearchCardCommandsQueue[0].player].searchCard.searchingId)
        {
            MatchManager.instance.matchActionService.Dispatch(selectSearchCardCommandsQueue[0]);
            
            selectSearchCardCommandsQueue.RemoveAt(0);
            
        }

        if(usedSearchCardCommandsQueue.Count > 0 &&  MatchManager.instance.players[usedSearchCardCommandsQueue[0].player].searchCard.isActiveAndEnabled && usedSearchCardCommandsQueue[0].searchingId == MatchManager.instance.players[usedSearchCardCommandsQueue[0].player].searchCard.searchingId)
        {
            MatchManager.instance.matchActionService.Dispatch(usedSearchCardCommandsQueue[0]);
            
            usedSearchCardCommandsQueue.RemoveAt(0);
        }

        //相手のサーチカード選択に関する処理(旧)
        //if (opponentPlayer && opponentPlayer.searchCard.isActiveAndEnabled)
        //{
        //    if (opponentSelectedSearchCardIds.ContainsKey(opponentPlayer.searchCard.searchingId))
        //    {
        //        foreach (int selectedSearchCardId in opponentSelectedSearchCardIds[opponentPlayer.searchCard.searchingId])
        //        {
        //            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "通信により相手のサーチカード選択を反映 カードID:" + selectedSearchCardId);
        //            opponentPlayer.searchCard.inSearchWindowSCards[selectedSearchCardId].Clicked();
        //        }
        //        opponentSelectedSearchCardIds[opponentPlayer.searchCard.searchingId].Clear();
        //    }

        //    if (opponentUsedSearchCards.ContainsKey(opponentPlayer.searchCard.searchingId))
        //    {
        //        foreach (PlayerAction usedSearchCard in opponentUsedSearchCards[opponentPlayer.searchCard.searchingId])
        //        {
        //            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "通信により相手のサーチカード使用を反映 カードID:" + usedSearchCard.targetGameCardId);
        //            StartCoroutine(opponentPlayer.cardPointer.SetCard(MatchManager.instance.GetGameCards()[usedSearchCard.targetGameCardId], usedSearchCard.setCardPos));
        //        }
        //        opponentUsedSearchCards[opponentPlayer.searchCard.searchingId].Clear();
        //    }           
        //}

        //YesNo処理
        if (yesNoCommandsQueue.Count > 0 && MatchManager.instance.players[yesNoCommandsQueue[0].player].yon.active)
        {
            MatchManager.instance.matchActionService.Dispatch(yesNoCommandsQueue[0]);

            yesNoCommandsQueue.RemoveAt(0);
        }

        //imp:旧yesno処理
        //if (opponentPlayer && opponentPlayer.yon.active && opponentYesNoDecision.Count > 0)
        //{
        //    if (opponentYesNoDecision[0])
        //    {
        //        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "通信により相手のYES判断を反映");
        //        opponentPlayer.yon.yes.ClickedCP();
        //    }
        //    else if (opponentYesNoDecision[0] == false)
        //    {
        //        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "通信により相手のNO判断を反映");
        //        opponentPlayer.yon.no.ClickedCP();
        //    }
        //    opponentYesNoDecision.RemoveAt(0);
        //}

        if (DebugKey.instance.enableDebugLog)
        {
            Debug.Log($"choseEffectCommands.Count:{choseEffectCommands.Count},active:{MatchManager.instance.players[choseEffectCommands[0].player].chooseEffect.active}");
        }

        //効果チョイス処理
        if (choseEffectCommands.Count > 0 && MatchManager.instance.players[choseEffectCommands[0].player].chooseEffect.active)
        {
            //MatchManager.instance.matchActionService.Dispatch(choseEffectCommands[0]);
            MatchManager.instance.matchActionService.executor.Execute(choseEffectCommands[0]);
            choseEffectCommands.RemoveAt(0);
        }

        //imp:旧チョイス処理
        //if (opponentPlayer && opponentPlayer.chooseEffect.active && opponentEffectChoices.Count > 0)
        //{
        //    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"通信により相手の使用効果選択を反映 [{opponentEffectChoices[0]}]");
        //    opponentPlayer.chooseEffect.generatedChoices[opponentEffectChoices[0]].Clicked();
        //    opponentEffectChoices.RemoveAt(0);
        //}
    }

    /// <summary>
    /// アクションキューにアクションを登録
    /// </summary>
    /// <param name="opponentActionType"></param>
    public void AddActionsQueue(PlayerAction opponentAction)
    {
        opponentActionsQueue.Add(opponentAction);
    }

    /// <summary>
    /// 通信により相手のコマンドを受け取り、Queueなどに送る(タイミングを合わせるためにQueueを通った後、コマンドの処理が行われる）（ここでは通信とQueueに送る最低限の処理のみで、それ以上のコマンドごとの特別な処理等を含めてはならない）
    /// </summary>
    /// <param name="commandJson"></param>
    [PunRPC]
    public void ReceiveOpponentCommand(string commandJson)
    {
        Debug.Log($"コマンド {commandJson} を受信しました");

        PlayerCommand restoredCommand = CommandSerializer.Deserialize(commandJson);

        //コマンドごとにそれぞれ対応するキューに振り分ける
        switch (restoredCommand)
        {
            case IActionCommand:
                opponentCommandsQueue.Add(restoredCommand);
                break;

            case SelectCardCommand sCommand:
                selectCardCommandsQueue.Add(sCommand);
                break;

            case SelectSearchCardCommand sCommand:
                selectSearchCardCommandsQueue.Add(sCommand);
                break;

            case UsedSearchCardCommand uCommand:
                usedSearchCardCommandsQueue.Add(uCommand);
                break;

            case YesNoCommand yCommand:
                yesNoCommandsQueue.Add(yCommand);
                break;

            case ChoseEffectCommand cCommand:
                choseEffectCommands.Add(cCommand);
                Debug.Log($"{cCommand.player}の効果選択: {MatchManager.instance.players[cCommand.player].chooseEffect.active}");
                break;

            default:
                Debug.LogError($"不明なコマンドタイプを受信しました: {restoredCommand.GetType()}");
                break;
        }
    }

    /// <summary>
    /// 相手が手札から通常のカード使用を行う
    /// </summary>
    /// <param name="gameCardId"></param>
    [PunRPC]
    public void OpponentUseCardNormal(int useGameCardId,Vector3 thisSetCardPos)
    {
        AddActionsQueue(new PlayerAction(PlayerAction.OpponentActionType.UseCardNormal,useGameCardId,thisSetCardPos));
    }

    /// <summary>
    /// 相手が効果でカードの使用をする
    /// </summary>
    /// <param name="gameCardId"></param>
    [PunRPC]
    public void OpponentUseCardEffect(int useGameCardId, Vector3 thisSetCardPos,int searchingId)
    {
        //af:↑searchingIdなんであるの？コピペして消し忘れたとか？てかこのメソッド参照ないやんどゆこと
        AddActionsQueue(new PlayerAction(PlayerAction.OpponentActionType.UseCardNormal, useGameCardId, thisSetCardPos));
    }

    /// <summary>
    /// 相手が起動効果を発動する
    /// </summary>
    /// <param name="effectInvokerCardId"></param>
    [PunRPC]
    public void OpponentEffectButton(int effectInvokerCardId)
    {
        AddActionsQueue(new PlayerAction(PlayerAction.OpponentActionType.EffectButton,effectInvokerCardId));
    }

    /// <summary>
    /// 相手がカードを選択する
    /// </summary>
    /// <param name="selectedCardId"></param>
    [PunRPC]
    public void OpponentSelectedCard(int selectedCardId,int selectingId)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のカード選択 ID:{selectedCardId}を受け取ります");
        //受け取ったselectingIdが同じ（同じ選択セットで複数枚カードを選択するとき、続けて2枚目以降の選択が送られてきた場合同じIdの選択カードリストに追加する。そうでなければ新しいidとして追加する
        if (opponentSelectedGameCardIds.ContainsKey(selectingId))
        {
            opponentSelectedGameCardIds[selectingId].Add(selectedCardId);
        }
        else
        {
            List<int> selectedCardIds = new List<int>();
            selectedCardIds.Add(selectedCardId);
            opponentSelectedGameCardIds.Add(selectingId,selectedCardIds);
        }
    }

    public void OpponentSelectedCard(SelectCardCommand command)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のカード選択 ID:{command.selectedCardId}を受け取ります");
        //受け取ったselectingIdが同じ（同じ選択セットで複数枚カードを選択するとき、続けて2枚目以降の選択が送られてきた場合同じIdの選択カードリストに追加する。そうでなければ新しいidとして追加する
        selectCardCommandsQueue.Add(command);
    }

    /// <summary>
    /// 相手がサーチカードを選択する
    /// </summary>
    /// <param name="selectedSearchId">選ばれたカードのサーチウィンドウ内のID ※バトル全体で割り振られるカードIDではなくサーチウィンドウ内で別で割り当てられるIDであることに注意</param>
    /// <param name="searchingId"></param>
    [PunRPC]
    public void OpponentSelectedSearchCard(int selectedSearchId, int searchingId)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のサーチ選択 サーチウィンドウ内ID:{selectedSearchId}を受け取ります");
        if (opponentSelectedSearchCardIds.ContainsKey(searchingId))
        {
            opponentSelectedSearchCardIds[searchingId].Add(selectedSearchId);
        }
        else
        {
            List<int> selectedSearchCardIds = new List<int>();
            selectedSearchCardIds.Add(selectedSearchId);
            opponentSelectedSearchCardIds.Add(searchingId, selectedSearchCardIds);
        }
        Debug.Log(opponentPlayer.cardPointer.pointerMode + " " + CardPointer.PointerModes.selectCard + " " + opponentSelectedGameCardIds.ContainsKey(opponentPlayer.cardPointer.nowSelectingId));
    }

    /// <summary>
    /// 相手がサーチカードを選択する
    /// </summary>
    [PunRPC]
    public void OpponentUsedSearchCard(int usedSearchCardId, Vector3 setPos, int searchingId)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network,$"相手のサーチカード使用情報を受け取る ID:{searchingId}");
        if (opponentUsedSearchCards.ContainsKey(searchingId))
        {
            //PlayerActionはrunpro0時のもののみなので、一番近いUseCardNormalをセットしているが、この処理ではtypeは参照されないので問題ない。are:そもそもなんかUseCardNormalがUsedSearchCardに変わってる?けどここに書いてあるTypeが据え置きなのは多分ミスだよな？
            opponentUsedSearchCards[searchingId].Add(new PlayerAction(PlayerAction.OpponentActionType.UseCardNormal,usedSearchCardId,setPos));
        }
        else
        {
            List<PlayerAction> usedSearchCardIds = new List<PlayerAction>();
            usedSearchCardIds.Add(new PlayerAction(PlayerAction.OpponentActionType.UseCardNormal, usedSearchCardId, setPos));
            opponentUsedSearchCards.Add(searchingId,usedSearchCardIds);
        }
    }

    /// <summary>
    /// 相手がYesNoを決める
    /// </summary>
    /// <param name="isYes"></param>
    [PunRPC]
    public void OpponentDecideYesNo(bool isYes)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のYESNO選択を受け取ります{isYes}");
        opponentYesNoDecision.Add(isYes);
    }

    [PunRPC]
    public void OpponentChoseEffect(int choseEffectNum)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手の効果選択を受け取ります[{choseEffectNum}]");
        opponentEffectChoices.Add(choseEffectNum);
    }

    /// <summary>
    /// 相手がフェーズを進める
    /// </summary>
    [PunRPC]
    public void OpponentClickedPhaseButton()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のフェーズボタンクリックを受け取ります");
        AddActionsQueue(new PlayerAction(PlayerAction.OpponentActionType.ClickedPhaseButton));
    }

    /// <summary>
    /// 相手がアタック選択を開始する
    /// </summary>
    [PunRPC]
    public void OpponentStartAttack(int attackerGameCardId)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手の攻撃宣言を受け取ります ID:{attackerGameCardId}");
        AddActionsQueue(new PlayerAction(PlayerAction.OpponentActionType.StartAttack,attackerGameCardId));
    }

    [PunRPC]
    public void OpponentDefence(int defencerGameCardId)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のディフェンスを受け取ります ID:{defencerGameCardId}");
        AddActionsQueue(new PlayerAction(PlayerAction.OpponentActionType.Defence, defencerGameCardId));
    }

    /// <summary>
    /// 相手が起動効果を発動する
    /// </summary>
    /// <param name="adventCardId"></param>
    [PunRPC]
    public void OpponentAdventButton(int adventCardId)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手の降臨処理開始を受け取ります ID:{adventCardId}");
        AddActionsQueue(new PlayerAction(PlayerAction.OpponentActionType.Advent, adventCardId));
    }

    [PunRPC]
    public void SpaceDraw()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のスペースドローを受け取ります");
        foreach (Player player in MatchManager.instance.players)
        {
            player.handScr.Draw();
        }
    }

    [PunRPC]
    public void OpponentSurrender()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手の降参を受け取ります");
        MatchManager.instance.localPlayer.opponentPlayer.Surrender();
    }

    /// <summary>
    /// PunRPCで相手のアクションを受け取った時の共通処理
    /// </summary>
    public void CommunicateActionCommon()
    {
        Debug.Log(MatchManager.instance.runningProcess.Count == 0);
    }
}

/// <summary>
/// あくまでアクションに関する情報を持っているだけでなにか動作を行うことはない。runningProcess0のときのアクションだけじゃなくて間に挟まる判断もこのクラスを使用可能
/// </summary>
public class PlayerAction
{
    public OpponentActionType actionType;
    public int targetGameCardId;
    public Vector3 setCardPos = Vector3.zero;

    public enum OpponentActionType
    {
        UseCardNormal,
        EffectButton,
        ClickedPhaseButton,
        StartAttack,
        Advent,
        Defence
    }

    public PlayerAction(OpponentActionType opponentActionType)
    {
        actionType = opponentActionType;
    }

    public PlayerAction(OpponentActionType opponentActionType, int gameCardId) : this(opponentActionType)
    {
        targetGameCardId = gameCardId;
    }

    public PlayerAction(OpponentActionType opponentActionType, int gameCardId,Vector3 targetSetCardPos) : this(opponentActionType,gameCardId)
    {
        setCardPos = targetSetCardPos;
    }
}
