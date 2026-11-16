using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventButton : CardButton
{
    public override void Clicked()
    {
        card.monsterScr.adventMonster.StartAdvent();
    }

    public override bool EnableButton()
    {
        return card.monsterScr.adventMonster.CanAdventCheck();
    }
}
