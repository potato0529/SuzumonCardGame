using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

/// <summary>
/// イベントデータの各要素のinspector表示をカスタマイズするためのDrawerクラス
/// </summary>
[CustomPropertyDrawer(typeof(StatusData))]
public class StatusDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // List用に1つのプロパティであることを示すためPropertyScopeで囲む
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            // 0指定だとReorderableListのドラッグと被るのでLineHeightを指定
            position.height = EditorGUIUtility.singleLineHeight;

            var valueTypeRect = new Rect(position)
            {
                y = position.y
            };
            var valueTypeProperty = property.FindPropertyRelative("statusValue");
            valueTypeProperty.intValue = EditorGUI.IntField(valueTypeRect, "値のタイプ(決め方)", valueTypeProperty.intValue);

            var statusTypeRect = new Rect(valueTypeRect)
            {
                y = valueTypeRect.y + EditorGUIUtility.singleLineHeight + 2f
            };
            var statusTypeProperty = property.FindPropertyRelative("statusType");
            statusTypeProperty.enumValueIndex = EditorGUI.Popup(statusTypeRect, "種類", statusTypeProperty.enumValueIndex, Enum.GetNames(typeof(StatusData.StatusType)));


            property.serializedObject.ApplyModifiedProperties();
        }
    }
}