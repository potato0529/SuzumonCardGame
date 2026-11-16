using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : CardTypeCommon
{
    [Header("アイテムタイプ")] public ItemEffectData.ItemType itemType;

    /// <summary>
    /// 手札のアイテムをクリックしたときに呼び出される。条件を満たすか判断し、満たせばアイテムを使用する
    /// </summary>
    public override bool CanUseCardType()
    {
        return card.effectNuggets[((byte)EffectNugget.InvokeTiming.Appear)] && card.effectNuggets[((byte)EffectNugget.InvokeTiming.Appear)].CanUseCheck();     
    }

    /// <summary>
    /// 効果ではなく通常の方法でアイテムを使用
    /// </summary>
    public void UseItemNormal()
    {
        UtilityLog.CardLog("アイテム使用可能！", card, UtilityLog.LogCategory.none);
        ownerPlayer.usedItemNum++;
        ownerPlayer.turnUsedItemNum++;
        
        if(itemType == ItemEffectData.ItemType.Normal)
            card.ChangeCardState(Card.CardState.Cemetery);
        UseItem();
    }

    /// <summary>
    /// アイテムを使用する
    /// </summary>
    public virtual void UseItem()
    {
        ownerPlayer.playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.UsedItemCount, 1);
        card.ComeInvokeTiming(EffectNugget.InvokeTiming.Appear);
        if (card.effectNuggets[((byte)EffectNugget.InvokeTiming.Appear)]) //使用時効果を持っている
        {
            //card.effectNuggets[((byte)EffectNugget.InvokeTiming.Appear)].UseEffectOrder();
        }
        
    }

    public override void OpenMenu()
    {
        CardInfoMenu cardInfoMenu = ownerPlayer.playerCanvas.cardInfoMenu;
        List<CardButton> cardButtons = new List<CardButton>();

        if(card.effectNuggets[(int)EffectNugget.InvokeTiming.MyTurn])
            cardButtons.Add(cardInfoMenu.effectButton);

        cardInfoMenu.OpenMenuOn3DSpace(card, cardButtons);
    }
}
