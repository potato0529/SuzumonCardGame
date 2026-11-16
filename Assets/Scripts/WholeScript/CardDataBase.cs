using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    [Header("ゲームに存在するすべてのカード")] public List<CardData> cardDatas = new List<CardData>();

    /// <summary>
    /// 渡されたCardDataのIDを返す
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public int GetCardId(CardData cardData)
    {
        return cardDatas.IndexOf(cardData);
    }
}
