using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectData : MonoBehaviour
{
    [Header("出すコストナゲット")] public EffectNugget appearCostNugget;
    [Header("効果ナゲットの親空オブジェクト")] public Transform effectNuggetsParent;
    [Header("消費する使用権")] public int useRights = 1;
    [Header("使用権免除コスト")] public EffectNugget costForExemptionFromUseRight;
    [Header("デュフォルトステータス")] public List<StatusData> statusDatas = new List<StatusData>();
    [Header("モンスター効果データ")] public MonsterEffectData monsterEffectData;
    [Header("アイテム効果データ")] public ItemEffectData itemEffectData;
    public List<int> testList = new List<int>();

}

[Serializable]
public class StatusData
{
    public int statusValue;
    public StatusType statusType;

    public enum StatusType
    {
        Attack = 0,
        WithStand = 1,
        Dodge = 2,
        Paralysis = 3,
        Preserved = 4,
        Poison = 5,
        Energy = 10,
        Electricity = 6,
        Mining = 7,
        FoodBreak = 8,
        Heartful = 9,
        BreakRush = 11,
        SnowScene = 12,
        Note = 13,
        Order = 14,
        Blood = 15,
        SpellMemory = 16,
        Noroi = 17
    }
}
