using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Effect_Choices))]
public class Effect_ChoicesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Effect_Choices effect = target as Effect_Choices;

        EditorGUI.BeginChangeCheck();

        
        var mainValue = serializedObject.FindProperty("effectNuggetChoices");
        EditorGUILayout.PropertyField(mainValue);

        effect.doIChoose = EditorGUILayout.Toggle( "‘I‘ð‚·‚é‚Ì‚ªŽ©•ª‚©‚Ç‚¤‚©",effect.doIChoose);


        var condition = serializedObject.FindProperty("condition");
        EditorGUILayout.PropertyField(condition);

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(effect);
        }

    }
}