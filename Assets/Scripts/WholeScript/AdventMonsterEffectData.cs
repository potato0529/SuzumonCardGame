using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventMonsterEffectData : MonoBehaviour
{
    [Header("モンスターエフェクトデータ")] public MonsterEffectData monsterEffectData;
    [Header("降臨数※")] public int adventNum;//エディタ拡張にしようかな またはAdventDataとかつくるか
    [Header("降臨後の効果データ")] public EffectData adventedEffectData;
}
