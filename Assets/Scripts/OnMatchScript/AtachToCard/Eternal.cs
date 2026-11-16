using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eternal : Item
{
    public override bool CanUseCardType()
    {
        return ownerPlayer.IsRemainUseRights(card);

    }

    public override void UseItem()
    {
        UtilityLog.CardLog("エターナルアイテムを使います", card, UtilityLog.LogCategory.none);

        card.ChangeCardState(Card.CardState.Field);
        
        if (card.effectNuggets[((byte)EffectNugget.InvokeTiming.Appear)])
        {
            UtilityLog.CardLog("まずエターナルの出た時効果を使います", card, UtilityLog.LogCategory.none);
            card.ComeInvokeTiming(EffectNugget.InvokeTiming.Appear);
            //card.effectNuggets[((byte)EffectNugget.InvokeTiming.Appear)].UseEffectOrder();
            
        }
        else
        {
            UtilityLog.CardLog("このエターナルには出た時効果がありません", card, UtilityLog.LogCategory.none);
        }        
    }
}
