using UnityEngine;

public class RankFilter : FilterSection
{
    public override bool IsMatch(CardData cardData, int index)
    {
        return cardData.cardType == Card.CardType.Monster && cardData.rank == index;
    }
}
