using System.Collections.Generic;
using UnityEngine;

public class DeckIdGenerator
{
    public string GenerateDeckId(DeckBoxData deckBoxData)
    {
        string deckDataValue = "";
        List<Card_InDeckBox> deckCards = deckBoxData.cards;

        for (int i = 0; i < deckCards.Count; i++)
        {
            Card_InDeckBox card_InDeckBox = deckCards[i];
            CardData cardData = card_InDeckBox.cardData;//これ
            int cardId = GameManager.instance.cardDataBase.cardDatas.IndexOf(cardData);
            deckDataValue += cardId;
            //最後のみ/をつけない
            if (i < deckCards.Count - 1)
            {
                deckDataValue += "/";
            }
        }

        List<Card_InDeckBox> specialDeckCards = deckBoxData.specialCards;
        if (specialDeckCards.Count > 0)
        {
            deckDataValue += "/";
        }
        for (int i = 0; i < specialDeckCards.Count; i++)
        {
            Card_InDeckBox card_InDeckBox = specialDeckCards[i];
            CardData cardData = card_InDeckBox.cardData;//これ
            int cardId = GameManager.instance.cardDataBase.cardDatas.IndexOf(cardData);
            deckDataValue += cardId;
            //最後のみ/をつけない
            if (i < specialDeckCards.Count - 1)
            {
                deckDataValue += "/";
            }
        }

        return deckDataValue;
    }
}
