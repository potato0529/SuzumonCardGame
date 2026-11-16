using UnityEngine;

public class CardTypeFilter : FilterSection
{
    public override bool IsMatch(CardData cardData, int index)
    {
        return (byte)cardData.cardType == index;
    }
}
