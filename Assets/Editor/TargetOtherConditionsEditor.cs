using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TargetValueCondition))]
public class TargetOtherConditionsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        TargetValueCondition otherConditions = target as TargetValueCondition;

        EditorGUI.BeginChangeCheck();

        otherConditions.conditionType = (TargetValueCondition.ConditionType)EditorGUILayout.EnumPopup("カードのどの値をとるか", otherConditions.conditionType);

        if(otherConditions.conditionType == TargetValueCondition.ConditionType.Status)
        {
            otherConditions.statusType = (StatusData.StatusType)EditorGUILayout.EnumPopup("ステータスタイプ", otherConditions.statusType);
        }

        otherConditions.equalSigns = (ConditionCommon.EqualSigns)EditorGUILayout.EnumPopup("比較記号", otherConditions.equalSigns);

        var value = serializedObject.FindProperty("value");
        EditorGUILayout.PropertyField(value);

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(otherConditions);
        }

    }
}