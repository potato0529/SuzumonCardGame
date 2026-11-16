using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEffectData : TypeEffectData
{
    [Header("モンスタータイプ")] public MonsterType monsterType;
    [Header("アタックダメージ")] public int attackDamage = 1;
    [Header("アタック可能回数")] public int turnCanAttackNum = 1;
    [Header("ディフェンス可能回数")] public int turnCanDefenceNum = 1;
    [Header("アタックされない")] public bool notAttacked;
    [Header("アタック条件")] public Condition attackCondition;
    [Header("アタックコスト")] public EffectNugget attackCost;
    [Header("バリアを持たない")] public bool dontHaveBarrier;
    [Header("降臨モンスターデータ")] public AdventMonsterEffectData adventMonsterEffectData;

    public enum MonsterType
    {
        Normal,
        Evolution,
        Advent
    }
}
