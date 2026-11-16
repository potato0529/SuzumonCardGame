//using UnityEngine;
//using UnityEditor;
//using UnityEditorInternal;

//[CustomEditor(typeof(Test))]
//public class TestEditor : Editor
//{
    
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        Test effectData = target as Test;

//        EditorGUI.BeginChangeCheck();

//        var mainValue = serializedObject.FindProperty("variableInt");
//        EditorGUILayout.PropertyField(mainValue);

//        serializedObject.ApplyModifiedProperties();

//        if (EditorGUI.EndChangeCheck())
//        {
//            EditorUtility.SetDirty(effectData);
//        }

//    }
//}