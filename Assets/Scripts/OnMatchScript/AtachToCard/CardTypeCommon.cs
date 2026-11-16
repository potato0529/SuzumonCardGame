using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTypeCommon : MonoBehaviour
{
    [HideInInspector] public Player ownerPlayer;
    [Header("カード")] public Card card;

    protected virtual void Awake()
    {
        card = GetComponent<Card>();
    }

    protected virtual void Start()
    {        
        ownerPlayer = card.ownerPlayer;       
    }

    public virtual void OpenMenu()
    {

    }

    /// <summary>
    /// カードタイプごとの固有の使用条件
    /// </summary>
    /// <returns></returns>
    public virtual bool CanUseCardType()
    {
        return true;
    }
}
