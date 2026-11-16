using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListedUICard : MonoBehaviour,ICardData
{
    [Header("カードデータ")] public CardData cardData;

    #region interface
    public CardData GetCardData()
    {
        return cardData;
    }

    public void SetCardData(CardData thisCardData)
    {
        cardData = thisCardData;
    }
    #endregion
}
