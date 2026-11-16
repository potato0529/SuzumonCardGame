using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSlot : MonoBehaviour
{
    [Header("このスロットが所有しているデッキ")] public DeckBoxImage boxImage;

    /// <summary>
    /// このスロットに指定されたデッキを割り当てる
    /// </summary>
    /// <param name="thisCardData"></param>
    public void AssignCardToSlot(DeckBoxImage thisDeckBoxImage)
    {
        boxImage = thisDeckBoxImage;
        Debug.Log($"{thisDeckBoxImage} をスロット {this} に割り当てました");
        thisDeckBoxImage.transform.SetParent(transform,false);
        thisDeckBoxImage.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
    }
}
