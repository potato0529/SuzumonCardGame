using UnityEngine;

public class CardUsabilityFilter : FilterSection
{
    public override bool IsMatch(CardData cardData, int index)
    {
        return (byte)cardData.usabilityType == index;
    }
}
