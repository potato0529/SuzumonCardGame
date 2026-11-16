using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStatus : Status
{
    [Header("アタックスクリプト")] public Attack attack;
    [Header("アタックダメージ変化条件")] public Condition atkDmgCondition;
    [Header("変化後のアタックダメージ")] public int changedAtkDmg;

    public GameObject changedByWhich;

    public override void Initialize(Card addedCard)
    {
        base.Initialize(addedCard);
        attack = card.monsterScr.attack;
        attack.attackStatus = this;
        //card.cardStatuses.statuses[(byte)StatusData.StatusType.Attack] = this;
    }

    public override void ChangedValue(int value)
    {
        base.ChangedValue(value);
        card.monsterScr.attack.attackDamage = statusValue;
    }
}
