using UnityEngine;
using Unity.Services.Analytics;

public class CardPlayedEvent : Unity.Services.Analytics.Event
{
    public CardPlayedEvent() : base("cardPlayed")
    {

    }

    public int CardId
    {
        set => SetParameter("cardId", value);
    }
}
