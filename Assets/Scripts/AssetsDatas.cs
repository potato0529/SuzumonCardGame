using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsDatas : MonoBehaviour
{
    [Header("ゲームカードプレハブ")] public Card gameCardPrefab;
    [Header("モンスターカードプレハブ")] public Monster monsterPrefab;
    [Header("進化モンスタープレハブ")] public Monster evolutionMonsterPrefab;
    [Header("降臨モンスタープレハブ")] public AdventMonster adventMonsterPrefab;
    [Header("アイテムカードプレハブ")] public Item itemPrefab;
    [Header("エターナルカードプレハブ")] public Eternal eternalPrefab;
    [Header("ウェポンプレハブ")] public Weapon weaponPrefab;

    [Header("エフェクトナゲットプレハブ")] public EffectNugget effectNuggetPrefab;
    [Header("ロックオン")] public Lockon lockonPrefab;
    [Header("ステータスアイコン")] public StatusIcon statusIconPrefab;
}
