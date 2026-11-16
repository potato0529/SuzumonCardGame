using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item,IPassTarget,IENVariable //mnp:こいつIENVariableいっらんくね？
{
    [Header("装備可能モンスター条件")] public TargetCardConditionsNugget canEquipConditions;

    public Monster equipTarget;
    protected int turnUsedNum;
    protected int turnCanUseNum;
    protected bool paidCost;
    protected bool compUsedEffect;
    protected bool compEffectProcess;
    public override void UseItem()
    {
        UtilityLog.CardLog("ウェポンアイテムを使います", card, UtilityLog.LogCategory.none);
        
        StartCoroutine(ChooseEquipTarget());        
    }

    public override bool CanUseCardType()
    {
        //装備対象が存在するか
        return canEquipConditions.GetMeetConditionsCards(card).Count > 0;
            
    }

    public virtual IEnumerator AppearWeaponEffect()
    {
        EffectNugget appearEffect = card.effectNuggets[((byte)EffectNugget.InvokeTiming.Appear)];
        if (appearEffect) //使用時効果を持っている
        {
            card.ComeInvokeTiming(EffectNugget.InvokeTiming.Appear);
            //appearEffect.UseEffectOrder();
        }

        while (MatchManager.instance.runningProcess.ContainsKey(MatchManager.RunningProcessType.EffectProcess))
        {
            yield return null;
        }
    }

    IEnumerator ChooseEquipTarget()
    {
        UtilityLog.CardLog("装備するモンスターを選びます", card, UtilityLog.LogCategory.none);
        //コールバックをこのオブジェクトに指定し、選ばれたカードを装備対象とする
        List<Card> equipableCards = canEquipConditions.GetMeetConditionsCards(card);
        ownerPlayer.cardPointer.SelectCard(1,false, equipableCards,  CardPointer.SelectType.equip, PassTargets,"装備するモンスター");
        while (!equipTarget)
        {
            yield return null;
        }
        equipTarget.card.Equipped(this);
        equipTarget.card.cardLayer.PileUnderThis(card);
        UtilityLog.CardLog("装備モンスターを選びました", card, UtilityLog.LogCategory.none);
        equipTarget.card.ComeInvokeTiming(EffectNugget.InvokeTiming.Equiped);
        StartCoroutine(AppearWeaponEffect());
    }

    public void PassTargets(List<Card> targets)
    {
        Monster targetMonster = targets[0].monsterScr;
        equipTarget = targetMonster;
    }

    #region IENVariable用
    
    public virtual void PaidCost()
    {
        paidCost = true;
    }

    public virtual void CompUsedEffect()
    {
        compUsedEffect = true;
    }
    #endregion
}
