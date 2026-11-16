using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preserved : Status
{
    public Card preservingParent;
    public Player paralysedTurnPlayer;

    public override void Initialize(Card addedCard)
    {
        base.Initialize(addedCard);
        paralysedTurnPlayer = MatchManager.instance.turnPlayer;
        paralysedTurnPlayer.phaseProcess.preservedStatuses.Add(this);
        card.SetColor(new Color(0, 1, 0.7f, 1));
    }

    /// <summary>
    /// 保存者がフィールドを離れた時に保存を解除する
    /// </summary>
    public void ParentLeaveField()
    {
        SetValue(0);
    }

    public void DecreasePreservation()
    {
        AddValue(-1);        
    }

    public override void ChangedValue(int value)
    {
        base.ChangedValue(value);
        if (statusValue <= 0)
        {
            card.cardStatuses.statusesDictionary.Remove(StatusData.StatusType.Preserved);
            paralysedTurnPlayer.phaseProcess.preservedStatuses.Remove(this);
            card.SetColor(new Color(1, 1, 1, 1));
        }
    }
}
