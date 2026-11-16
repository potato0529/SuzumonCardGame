using System.Collections.Generic;
using UnityEngine;

public class AttributeFilter : FilterSection
{
   
    public override bool IsMatch(CardData cardData, int index)
    {
        return cardData.cardType == Card.CardType.Monster && (byte)cardData.attribute == index;
    }
}
    
