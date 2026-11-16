using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceButton : CardButton
{
    public override void Clicked()
    {
        UtilityLog.CardLog("ディフェンスボタンが押される", card, UtilityLog.LogCategory.none);
        MatchManager.instance.matchActionService.Submit(new DefenceCommand(card.controlerPlayer, MatchManager.instance.GetCardId(card)));
    }

    public override bool EnableButton()
    {
        return card.monsterScr.CanDefence();
    }
}
