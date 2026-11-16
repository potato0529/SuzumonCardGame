using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor (typeof(Effect))]
public class EffectInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Effect effect = target as Effect;

        EditorGUI.BeginChangeCheck();

        effect.effectType = (Effect.EffectTypes)EditorGUILayout.EnumPopup("EffectType", effect.effectType);

        EditorGUI.indentLevel++;

        if (effect.effectType.IsEffectTargetPlayer())
        {
            effect.targetPlayer = (TargetCardConditions.TargetOwners)EditorGUILayout.EnumPopup("対象のプレイヤー", effect.targetPlayer);
        }

        if (effect.effectType.IsEffectHaveValue())
        {
            var mainValue = serializedObject.FindProperty("mainValue");
            EditorGUILayout.PropertyField(mainValue);
        }

        if (effect.effectType.IsEffectMultipleTargetCard())
        {
            effect.focusType2 = (Effect.TargetFocusType)EditorGUILayout.EnumPopup("ターゲットの決定方法", effect.focusType2);
            var focusValue2 = serializedObject.FindProperty("focusValue2");
            EditorGUILayout.PropertyField(focusValue2);
            var secondDecideNugget = serializedObject.FindProperty("secondDecideNugget");
            EditorGUILayout.PropertyField(secondDecideNugget);
        }

        switch (effect.effectType)
        {
            case Effect.EffectTypes.AddStatus:
                effect.statusType = (StatusData.StatusType)EditorGUILayout.EnumPopup("ステータスタイプ", effect.statusType);
                break;

            case Effect.EffectTypes.AntiqueBurst:
                var referenceEffectNugget = serializedObject.FindProperty("referenceEffectNugget");
                EditorGUILayout.PropertyField(referenceEffectNugget);

                var expandProperties = serializedObject.FindProperty("expandProperties");
                EditorGUILayout.PropertyField(expandProperties);
                break;
        }

        EditorGUI.indentLevel--;

        if (effect.effectType.IsEffectTargetCard())
        {
            effect.focusType = (Effect.TargetFocusType)EditorGUILayout.EnumPopup("ターゲットの決定方法", effect.focusType);
            var focusValue = serializedObject.FindProperty("focusValue");
            EditorGUILayout.PropertyField(focusValue);

            var targetDecideNugget = serializedObject.FindProperty("targetDecideNugget");
            EditorGUILayout.PropertyField(targetDecideNugget);
        }

        var condition = serializedObject.FindProperty("condition");
        EditorGUILayout.PropertyField(condition);

        effect.allowBelowTargets = EditorGUILayout.Toggle("ターゲットが必要数より少なくても決定してよいか", effect.allowBelowTargets);

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(effect);
        }
        
    }
}