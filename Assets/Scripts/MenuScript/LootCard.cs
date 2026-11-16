using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCard : MonoBehaviour
{
    [Header("排出率")] public float probability;
    [Header("カードデータ")] public CardData cardData;
    [Header("カードid")] public int cardId;

    [HideInInspector] public float lowerGetProbability;//排出乱数の下限(以上)
    [HideInInspector] public float upperGetProbability;//排出乱数の上限(未満)
}
