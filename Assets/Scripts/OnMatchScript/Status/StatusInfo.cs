using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StatusInfo
{
    [Header("ステータスタイプ")] public StatusData.StatusType statusType;
    [Header("アイコンのスプライト")] public Sprite iconSprite;
    [Header("ステータスが0以下で削除するか")] public bool doRemoveLower0Status;
    [Header("ステータスが有効となる場所")] public List<Card.CardState> enableState = new List<Card.CardState>();
    
}
