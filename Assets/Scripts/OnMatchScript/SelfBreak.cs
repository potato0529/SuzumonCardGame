using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfBreak : MonoBehaviour,ITriggers
{
    private Card card;

    public void Added ()
    {
        TriggerEffectManager.instance.AddTurnEndTriggerQueue(new TriggerInfo(TurnEndTrigger,false));
        card = GetComponent<Card>();
    }

    public void TurnEndTrigger()
    {
        card.Destroyed();
    }
}
