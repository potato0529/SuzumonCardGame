using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ValueGenerator))]
public class ValueGeneratorEditor : Editor
{
    private ReorderableList reorderableList;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var firstValue = serializedObject.FindProperty("firstValue");
        EditorGUILayout.PropertyField(firstValue);
        
        if(reorderableList == null)
        {
            //ÉäÉXÉgÇÃê›íË
            var listProp = serializedObject.FindProperty("operatorUnits");

            reorderableList = new ReorderableList(serializedObject, listProp);

            reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "åvéZéÆ(çÄÇí«â¡)");

            reorderableList.elementHeightCallback = index => EditorGUI.GetPropertyHeight(listProp.GetArrayElementAtIndex(index));

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var elementProperty = listProp.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, elementProperty, new GUIContent("çÄ" + index));
            };
        }
        //ï`âÊ
        reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
