using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class TotalTargetDecideNugget
{
    [Header("ターゲット決定条件(overload")] public TargetDecideCondition[] targetDecideConditions = new TargetDecideCondition[0];

    //使いまわせるようにコンディションタイプを増やしているが、カードに付属しているものかどうかなどで対応しているものは限られる。カードに付属していないのに付属カード参照が必要なタイプを選択したときはエラーになるという前提。本来こういう例外を実現できてしまうのはあまりよくないが、利便性を高めるためにそこは正しく判断しようという感じ。
    public enum TargetConditionType
    {
        Select,
        Myself,
        AllCondition,
        TopNthCard,
        TopNCards,
        SelectedTargets, //前の効果で既に選択されたターゲット
        EquipCard //このウェポン(カード)を装備しているカード
    }

    public virtual List<Card> SearchMeetConditionTypeCards()
    {
        return new List<Card>();
    }

    /// <summary>
    /// DecideConditionsを元に全てのカード候補を取得する。
    /// </summary>
    /// <param name="player"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    public List<Card> SearchTotalTargets(Card card, params Func<Card, bool>[] expandTargetConditions)
    {
        List<Card> searchedCards = new List<Card>();
        foreach(TargetDecideCondition targetDecideCondition in targetDecideConditions)
        {
            if (!targetDecideCondition.isExcludeCondition)
            {
                searchedCards.AddRange(targetDecideCondition.SearchTargets(card, expandTargetConditions));
                searchedCards.Distinct();
            }
            //除外条件の場合
            else
            {
                foreach(Card excludeCard in targetDecideCondition.SearchTargets(card))
                {
                    if (searchedCards.Contains(excludeCard))
                    {
                        searchedCards.Remove(excludeCard);
                    }
                }
            }
        }
        return searchedCards;
    }

    public void SerializableChange()
    {

    }
}