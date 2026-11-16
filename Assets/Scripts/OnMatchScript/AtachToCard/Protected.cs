using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protected : MonoBehaviour
{
    [Header("このカードを守っているカード")] public Card protectCard;

    /// <summary>
    /// 守る処理発動
    /// </summary>
    public void InvokeProtect()
    {
        protectCard.Destroyed();
        Destroy(this);
    }
}
