using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionCard : MonoBehaviour,ICardData
{
    [Header("カードデータ")] public CardData cardData;
    [Header("所持数")] public int inPossessionNum;

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
