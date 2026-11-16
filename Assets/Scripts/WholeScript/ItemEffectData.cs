using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectData : TypeEffectData
{
    [Header("アイテムタイプ")] public ItemType itemType;

    public TargetCardConditionsNugget canEquipConditionsNugget;

    public enum ItemType
    {
        Normal,
        Eternal,
        Weapon,
        Field,
        Extra
    }
}
