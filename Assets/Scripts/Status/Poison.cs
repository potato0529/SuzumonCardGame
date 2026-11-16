using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Status
{
    public int poisonDamage;
    public Player poisonedTurnPlayer;

    public override void Initialize(Card addedCard)
    {
        base.Initialize(addedCard);
        poisonedTurnPlayer = MatchManager.instance.turnPlayer;
        poisonedTurnPlayer.phaseProcess.poisonStatuses.Add(this);
    }

    public override void AddedStatus(StatusData.StatusType statusType, int value, StatusInfo thisStatusInfo)
    {
        base.AddedStatus(statusType, value, thisStatusInfo);
        poisonDamage = value;       
    }

    public void DecreasePoison()
    {
        AddValue(-1);
        
    }

    public override void ChangedValue(int value)
    {
        base.ChangedValue(value);
        if (statusValue <= 0)
        {
            card.cardStatuses.statusesDictionary.Remove(StatusData.StatusType.Poison);
            poisonedTurnPlayer.phaseProcess.poisonStatuses.Remove(this);
            card.DestroyedByPoison();
            card.controlerPlayer.ReceiveDamage(poisonDamage);
        }
    }

    public override void LeftField()
    {
        base.LeftField();
        card.cardStatuses.statusesDictionary.Remove(StatusData.StatusType.Poison);
        poisonedTurnPlayer.phaseProcess.poisonStatuses.Remove(this);
    }
}
