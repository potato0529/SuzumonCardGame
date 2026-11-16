using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ItemEffectData))]
public class ItemEffectDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ItemEffectData itemEffectData = target as ItemEffectData;

        EditorGUI.BeginChangeCheck();

        itemEffectData.itemType = (ItemEffectData.ItemType)EditorGUILayout.EnumPopup("ƒAƒCƒeƒ€‚ÌŽí—Þ", itemEffectData.itemType);

        switch (itemEffectData.itemType)
        {
            case ItemEffectData.ItemType.Normal:
                break;

            case ItemEffectData.ItemType.Eternal:
                break;

            case ItemEffectData.ItemType.Weapon:
                var canEquipConditionsNugget = serializedObject.FindProperty("canEquipConditionsNugget");
                EditorGUILayout.PropertyField(canEquipConditionsNugget);
                break;
        }
        
        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(itemEffectData);
        }
    }
}