using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Cost))]
public class CostInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Cost cost = target as Cost;

        EditorGUI.BeginChangeCheck();

        cost.costType = (Cost.Costs)EditorGUILayout.EnumPopup("CostType", cost.costType);

        if(cost.costType == Cost.Costs.Status)
        {
            cost.statusType = (StatusData.StatusType)EditorGUILayout.EnumPopup("ステータスタイプ", cost.statusType);
        }

        if (cost.costType.IsCostHaveValue())
        {
            var mainValue = serializedObject.FindProperty("mainValue");
            EditorGUILayout.PropertyField(mainValue);
            
            cost.allowBelowValue = EditorGUILayout.Toggle("コストの値未満でもよいか", cost.allowBelowValue);
        }

        if (cost.costType.IsCostTargetCard())
        {
            cost.focusType = (Effect.TargetFocusType)EditorGUILayout.EnumPopup("ターゲットの決定方法", cost.focusType);

            var focusValue = serializedObject.FindProperty("focusValue");
            EditorGUILayout.PropertyField(focusValue);

            var targetDecideNugget = serializedObject.FindProperty("targetDecideNugget");
            EditorGUILayout.PropertyField(targetDecideNugget);            
        }

        cost.allowBelowTargets = EditorGUILayout.Toggle("ターゲットが必要数より少なくても決定してよいか", cost.allowBelowTargets);

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(cost);
        }
    }
}