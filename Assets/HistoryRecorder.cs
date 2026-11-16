using System;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;

public class HistoryRecorder
{
    private List<PlayerCommand> commandHistory = new List<PlayerCommand>();

    /// <summary>
    /// 行われたCommandを記録する
    /// </summary>
    /// <param name="command"></param>
    public void Record(PlayerCommand command)
    {
        commandHistory.Add(command);
    }

    /// <summary>
    /// シリアライズされたcommandHistoryの内容を取得する
    /// </summary>
    /// <returns></returns>
    public string GetSerializedCommandHistory()
    {
        string serializedCommandHistory = $"Seed:{MatchManager.instance.DuelSeed}";

        DeckIdGenerator deckIdGenerator = new DeckIdGenerator();
        //serializedCommandHistory += $"| {deckIdGenerator.GenerateDeckId(MatchManager.instance.players[0].matchPlayerData.deckData)} || {MatchManager.instance.players[1].matchPlayerData.deckId} |";
        serializedCommandHistory += $"| {MatchManager.instance.players[0].matchPlayerData.deckId}  ||  {MatchManager.instance.players[1].matchPlayerData.deckId} |";

        //さらに下のswitchで置き換え中　シリアライズ式にしたければ代わりにこっちのコメントアウトを外してもよい
        //foreach (PlayerCommand command in commandHistory)
        //{
        //    serializedCommandHistory += CommandSerializer.Serialize(command) + "|";
        //    UtilityLog.DebugLog($"ヒストリーシリアライズ中 現在:{serializedCommandHistory}");
        //}

        foreach (PlayerCommand command in commandHistory)
        {
            string commandText = $"Type:{command.GetType()}|P:{command.player}|";
            switch (command)
            {
                case UseCardNormalCommand uCommand:
                    //commandText += $"Card:{MatchManager.instance.GetGameCards()[uCommand.cardId].cardData.cardName}";
                    commandText += $"Card:{uCommand.cardId}";
                    break;

                case UseCardEffectCommand uCommand:
                    break;

                case EffectButtonCommand eCommand:
                    commandText += $"Card:{eCommand.effectInvokerCardId}";

                    break;

                case SelectCardCommand sCommand:
                    commandText += $"Card:{sCommand.selectedCardId}";
                    break;

                case SelectSearchCardCommand sCommand:
                    commandText += $"Card:{sCommand.selectedCardId}";
                    break;

                case UsedSearchCardCommand uCommand:
                    commandText += $"Card:{uCommand.usedSearchCardId}";
                    break;

                case ChoseEffectCommand cCommand:
                    commandText += $"ChoseNum{cCommand.choseEffectNum}";
                    break;

                case PhaseButtonCommand pCommand:

                    break;

                case StartAttackCommand sCommand:
                    commandText += $"Card:{sCommand.attackerCardId}";
                    break;

                case DefenceCommand dCommand:
                    commandText += $"Card:{dCommand.defencerCardId}";
                    break;

                case AdventButtonCommand aCommand:
                    commandText += $"Card:{aCommand.adventCardId}";
                    break;

                case YesNoCommand yCommand:
                    commandText += $"isYes{yCommand.isYes}";
                    break;

                case SpaceDrawCommand sCommand:

                    break;

                case SurrenderCommand sCommand:
                    break;

            }

            commandText += "||";

            serializedCommandHistory += commandText;
        }

        return serializedCommandHistory;

    }
}
