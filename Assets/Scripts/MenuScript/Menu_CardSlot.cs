using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_CardSlot : MonoBehaviour
{
    [Header("このスロットが所有しているカード")] public ListedCard listedCard;

    /// <summary>
    /// このスロットに指定されたカードを割り当てる
    /// </summary>
    /// <param name="thisCardData"></param>
    public void AssignCardToSlot(ListedCard thisListedCard)
    {
        listedCard = thisListedCard;
        Debug.Log($"{thisListedCard} をスロット {this} に割り当てました");
        thisListedCard.transform.SetParent(transform);
        thisListedCard.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
    }
}
