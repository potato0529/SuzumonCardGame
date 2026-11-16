using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : CardButton
{
    public override void Clicked()
    {
        MatchManager.instance.matchActionService.Submit(new StartAttackCommand(card.controlerPlayer, MatchManager.instance.GetCardId(card)));
        //card.GetComponent<Monster>().AttackButton();
    }

    public override bool EnableButton()
    {
        return card.monsterScr.attack.CanAttack();
    }
}
