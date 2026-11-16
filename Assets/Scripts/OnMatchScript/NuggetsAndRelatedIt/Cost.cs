using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cost : Ability
{
    [Header("コストの種類")] public Costs costType;
    [HideInInspector] public bool allowBelowValue;

    public enum Costs
    {
        none = 0,
        hand = 1,
        life = 2,
        fieldMonster = 3,
        SendCemeteryCard = 7,
        DestroyFieldMonster = 6,
        Status = 4,
        Bounce = 5,
        ReturnDeck = 8
    }

    /// <summary>
    /// コストを払うためのタイプに応じた追加条件を取得する
    /// </summary>
    /// <param name="userCard"></param>
    /// <returns></returns>
    public Func<Card, bool> GetCostExpandCondition(Card userCard)
    {
        switch (costType)
        {
            case Costs.Status:
                return (card) => { return mainValue.GetValue(userCard) >= 1 || (card.cardStatuses.statusesDictionary.ContainsKey(statusType) && (allowBelowValue || (card.cardStatuses.statusesDictionary[statusType].StatusValue + mainValue.GetValue(userCard)) >= 0)); };

        }
        return (card) => { return true; };
    }
}
