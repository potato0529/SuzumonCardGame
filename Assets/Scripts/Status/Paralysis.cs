using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralysis : Status
{
    public override void Initialize(Card addedCard)
    {
        base.Initialize(addedCard);
        //TriggerEffectManager.instance.AddTurnEndTriggerQueue(new TriggerInfo(DecreaseParalysis,true));
        card.ownerPlayer.phaseProcess.paralysisStatuses.Add(this);
    }

    public void DecreaseParalysis()
    {
        AddValue(-1);
        if(statusValue <= 0)
        {
            card.cardStatuses.statusesDictionary.Remove(StatusData.StatusType.Paralysis);
            card.controlerPlayer.phaseProcess.paralysisStatuses.Remove(this);
        }        
    }
}
