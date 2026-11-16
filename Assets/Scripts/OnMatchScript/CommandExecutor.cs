using System.Windows.Input;
using UnityEngine;

public class CommandExecutor
{
    public void Execute(PlayerCommand command)
    {
        Debug.Log($"コマンド {command.GetType()} を実行します");
        switch (command)
        {
            case UseCardNormalCommand uCommand:
                UtilityLog.CardLog("Executor:カード使用を反映",MatchManager.instance.GetGameCards()[uCommand.cardId]);
                MatchManager.instance.players[command.player].cardPointer.SetCardTemp(MatchManager.instance.GetGameCards()[uCommand.cardId], uCommand.setPos.ToVector3());
                break;

            case UseCardEffectCommand uCommand:
                break;

            case EffectButtonCommand eCommand:
                UtilityLog.CardLog("Executor:起動効果発動を反映",MatchManager.instance.GetGameCards()[eCommand.effectInvokerCardId]);
                MatchManager.instance.GetGameCards()[eCommand.effectInvokerCardId].effectNuggets[((byte)EffectNugget.InvokeTiming.MyTurn)].UseEffectOrder();
                break;

            case SelectCardCommand sCommand:
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "通信により相手のカード選択を反映 カードID:" + sCommand.selectedCardId);
                int selectedGameCardId = sCommand.selectedCardId;
                Player commandPlayer = MatchManager.instance.players[sCommand.player];
                if (commandPlayer && commandPlayer.cardPointer.pointerMode == CardPointer.PointerModes.selectCard)
                {
                    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "通信により相手のカード選択を反映 カードID:" + selectedGameCardId);
                    if (selectedGameCardId >= 0)
                    {
                        commandPlayer.cardPointer.SelectedCard(MatchManager.instance.GetGameCards()[selectedGameCardId]);
                    }
                    else if (selectedGameCardId == -1)
                    {
                        commandPlayer.cardPointer.SelectedOtherTarget(commandPlayer.opponentPlayer.selectableTarget);
                    }
                    else if (selectedGameCardId == -2)
                    {
                        commandPlayer.cardPointer.SelectedOtherTarget(commandPlayer.selectableTarget);
                    }
                    else if (selectedGameCardId == -3)
                    {
                        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"相手のターゲット決定ボタンクリックを受け取ります");
                        //na:前までCardPointer.DecidedTargetButton()を読んでたんだけど.CardPointer.DecideTargetButton()直で呼んでいいよね？
                        commandPlayer.cardPointer.DecidedTargetsCP();
                    }
                }
                break;

            case SelectSearchCardCommand sCommand:
                UtilityLog.CardLog("Executor:サーチカード選択を反映", MatchManager.instance.GetGameCards()[sCommand.selectedCardId]);
                MatchManager.instance.players[sCommand.player].searchCard.inSearchWindowSCards[sCommand.selectedCardId].Selected();
                break;

            case UsedSearchCardCommand uCommand:
                UtilityLog.CardLog("Executor:サーチカード使用を反映", MatchManager.instance.GetGameCards()[uCommand.usedSearchCardId]);
                MatchManager.instance.players[uCommand.player].cardPointer.SetCardTemp(MatchManager.instance.GetGameCards()[uCommand.usedSearchCardId], uCommand.setPos.ToVector3());

                break;

            case ChoseEffectCommand cCommand:
                Debug.Log("Executor:効果チョイスの決定を反映");
                MatchManager.instance.players[cCommand.player].chooseEffect.generatedChoices[cCommand.choseEffectNum].ChoseThisChoice();
                
                break;

            case PhaseButtonCommand pCommand:
                Debug.Log("Executor:ターンエンドを反映");
                MatchManager.instance.phaseManager.EndActionPhase();
                MatchManager.instance.matchSound.turnEnd.PlayOneShot(MatchManager.instance.matchSound.turnEnd.clip);
                break;

            case StartAttackCommand sCommand:
                UtilityLog.CardLog("Executor:攻撃宣言をする", MatchManager.instance.GetGameCards()[sCommand.attackerCardId]);
                MatchManager.instance.GetGameCards()[sCommand.attackerCardId].monsterScr.AttackButton();
                break;

            case DefenceCommand dCommand:
                UtilityLog.CardLog("Executor:ディフェンスをする", MatchManager.instance.GetGameCards()[dCommand.defencerCardId]);
                MatchManager.instance.GetGameCards()[dCommand.defencerCardId].monsterScr.DefenceButton();
                break;

            case AdventButtonCommand aCommand:
                UtilityLog.CardLog("Executor:降臨処理を開始する", MatchManager.instance.GetGameCards()[aCommand.adventCardId]);
                MatchManager.instance.GetGameCards()[aCommand.adventCardId].monsterScr.adventMonster.DecideSacrifice();
                break;

            case YesNoCommand yCommand:
                if (yCommand.isYes)
                {
                    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "コマンド:YES判断を反映");
                    MatchManager.instance.players[yCommand.player].yon.yes.DecidedCP();
                }
                else
                {
                    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, "コマンド:NO判断を反映");
                    MatchManager.instance.players[yCommand.player].yon.no.DecidedCP();
                }
                break;

            case SpaceDrawCommand sCommand:
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"スペースドローを受け取ります");
                foreach (Player player in MatchManager.instance.players)
                {
                    player.handScr.Draw();
                }
                break;

            case SurrenderCommand sCommand:
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"降参を受け取ります");
                MatchManager.instance.players[sCommand.player].Surrender();
                break;
        }
    }

    #region memo
    //switch (command)
    //{
    //    case UseCardNormalCommand uCommand:
    //        break;

    //    case UseCardEffectCommand uCommand:
    //        break;

    //    case EffectButtonCommand eCommand:
    //        break;

    //    case SelectCardCommand sCommand:
    //        break;

    //    case SelectSearchCardCommand sCommand:
    //        break;

    //    case UsedSearchCardCommand uCommand:
    //        break;

    //    case ChoseEffectCommand cCommand:
    //        break;

    //    case PhaseButtonCommand pCommand:
    //        break;

    //    case StartAttackCommand sCommand:
    //        break;

    //    case DefenceCommand dCommand:
    //        break;

    //    case AdventButtonCommand aCommand:
    //        break;

    //    case SpaceDrawCommand sCommand:
    //        break;

    //    case SurrenderCommand sCommand:
    //        break;
    //}
    #endregion
}
