using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ValueGenerator : MonoBehaviour
{
    public List<OperatorUnit> operatorUnits = new List<OperatorUnit>();
    public int firstValue;
    private float calculatedValue;

    /// <summary>
    /// 設定された情報をもとに効果の値を生成する
    /// </summary>
    public int GenerateValue(Card card)
    {
        float value = firstValue;
        foreach (OperatorUnit operatorUnit in operatorUnits)
        {
            operatorUnit.card = card;
            calculatedValue = operatorUnit.Calculate(card);
            
            switch (operatorUnit.operatorType)
            {
                case OperatorUnit.OperatorType.Plus:
                    value += calculatedValue;
                    break;

                case OperatorUnit.OperatorType.Minus:
                    value -= calculatedValue;
                    break;

                case OperatorUnit.OperatorType.Times:
                    value *= calculatedValue;
                    break;

                case OperatorUnit.OperatorType.Divided:
                    value /= calculatedValue;
                    break;

                case OperatorUnit.OperatorType.Absolute:
                    value = Math.Abs(value);
                    break;
            }
        }

        Debug.Log("生成された値:" + value);
        return (int)value;
    }

    /// <summary>
    /// Calculate()で実行された時間差で値を受け取る処理ででた値を受け取る
    /// </summary>
    public void ReceiveValue(int result)
    {
        calculatedValue = result;
    }
}



[Serializable]
public class OperatorUnit
{
    public ValueSource valueSource;
    public OperatorType operatorType;
    public float termValue;
    public Ability referenceEffect; //インスペクターで設定 参照する使用済み効果
    public UsedEffectValueType usedEffectValueType;
    public TargetConditionNuggetMB countCardsConditionsNugget; //meetConditionTargetsNum用
    public TotalTargetDecideNuggetMB tallyStatusesTargetConditions; //StatusSumOfTargets用
    public StatusData.StatusType checkStatusType; //StatusSumOfTargets用
    public Card card;
    public PlayerMatchLog.IntMatchData intMatchData; //PlayerMatchLog用
    public PlayerMatchLog.IntMatchData turnIntMatchData; //PlayerTurnLog用

    public enum ValueSource
    {
        FixedValue,
        MyLifeNum,
        OpponentLifeNum,
        MeetConditionTargetsNum,
        StatusSumOfTargets,
        Field,
        UsedEffectResult,
        DecideTargetsNum,
        RankSumOfTarget,
        DetailType,//配列が渡された場合1番目が対象となるため、基本的に1対1でしか比較しない
        ValueGenerator,//未実装
        PlayerMatchLog,
        PlayerTurnLog,
        TurnsPassed,
        none
    }

    public enum OperatorType
    {
        Plus,
        Minus,
        Times,
        Divided,
        Absolute
    }

    public enum UsedEffectValueType
    {
        TargetedCardsNum = 2,
        MainValue = 0,
        ConditionNuggetValue = 1,        
    }

    /// <summary>
    /// 参照する値の種類に応じて、calculatedValueに値を渡す
    /// </summary>
    /// <param name="valueSource"></param>
    public float Calculate(Card userCard)
    {
        switch (valueSource)
        {
            case ValueSource.FixedValue:
                return termValue;

            case ValueSource.MyLifeNum:
                return card.controlerPlayer.life;

            case ValueSource.OpponentLifeNum:
                return card.controlerPlayer.opponentPlayer.life;

            case ValueSource.MeetConditionTargetsNum:
                return countCardsConditionsNugget.conditionsNugget.GetMeetConditionsCards(userCard).Count;

            case ValueSource.StatusSumOfTargets:
                List<Card> meetConditionTargets = tallyStatusesTargetConditions.decideNugget.SearchTotalTargets(card);

                //ステータスを集計する
                int statusesSum = 0;

                foreach(Card card in meetConditionTargets)
                {
                    if (card.cardStatuses.statusesDictionary.ContainsKey(checkStatusType))
                    {
                        statusesSum += card.cardStatuses.statusesDictionary[checkStatusType].StatusValue;
                    }
                }
                return statusesSum;

            case ValueSource.Field:
                return FieldManager.instance.NowFieldNum;

            case ValueSource.UsedEffectResult:
                switch (usedEffectValueType)
                {
                    case UsedEffectValueType.MainValue:
                        return referenceEffect.usedMainValue;

                    case UsedEffectValueType.ConditionNuggetValue:
                        return referenceEffect.usedConditionNuggetValue;

                    case UsedEffectValueType.TargetedCardsNum:
                        return referenceEffect.targetedCardsValue;
                }
                break;

            case ValueSource.DecideTargetsNum:
                return tallyStatusesTargetConditions.decideNugget.SearchTotalTargets(userCard).Count;

            case ValueSource.RankSumOfTarget:
                List<Card> meetRankConditionTargets = tallyStatusesTargetConditions.decideNugget.SearchTotalTargets(card);

                //ステータスを集計する
                int rankSum = 0;
                foreach (Card card in meetRankConditionTargets)
                {
                    if (card.monsterScr)
                    {
                        rankSum += card.rank;
                    }
                }
                return rankSum;

            case ValueSource.DetailType:
                return (int)tallyStatusesTargetConditions.decideNugget.SearchTotalTargets(card)[0].cardData.GetDetailCardType();

            case ValueSource.PlayerMatchLog:
                return card.controlerPlayer.playerMatchLog.GetIntMatchData(intMatchData);

            case ValueSource.PlayerTurnLog:
                return card.controlerPlayer.playerMatchLog.GetTurnIntMatchData(turnIntMatchData);

            case ValueSource.TurnsPassed:
                return MatchManager.instance.phaseManager.turnNum;
        }
        return 0;
    }
}
