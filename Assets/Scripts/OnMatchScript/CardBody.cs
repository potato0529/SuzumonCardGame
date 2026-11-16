using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBody : MonoBehaviour
{
    [Header("カード本体スクリプト")] public Card card; 
    /// <summary>
    /// ref+1
    /// </summary>
    public void PointerClick()
    {
        card.PointerClick();
    }

    /// <summary>
    /// ref+1
    /// </summary>
    public void PointerEnter()
    {
        card.PointerEnter();
    }

    /// <summary>
    /// ref+1
    /// </summary>
    public void PointerExit()
    {
        card.PointerExit();
    }
}
