using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(EffectData))]
public class EffectDataEditor : Editor
{
    private ReorderableList statusDatasList;

    private void OnEnable()
    {
        
        //リストの設定
        var listProp = serializedObject.FindProperty("statusDatas");

        statusDatasList = new ReorderableList(serializedObject, listProp);

        statusDatasList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "追加ステータス");

        statusDatasList.elementHeightCallback = index => 40;

        statusDatasList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var elementProperty = listProp.GetArrayElementAtIndex(index);
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, elementProperty, new GUIContent("ステータス" + index));
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EffectData effectData = target as EffectData;

        EditorGUI.BeginChangeCheck();

        var appearCostNugget = serializedObject.FindProperty("appearCostNugget");
        EditorGUILayout.PropertyField(appearCostNugget);

        var effectNuggetsParent = serializedObject.FindProperty("effectNuggetsParent");
        EditorGUILayout.PropertyField(effectNuggetsParent);

        effectData.useRights = EditorGUILayout.IntField("消費する使用権", effectData.useRights);

        var monsterEffectData = serializedObject.FindProperty("monsterEffectData");
        EditorGUILayout.PropertyField(monsterEffectData);

        var itemEffectData = serializedObject.FindProperty("itemEffectData");
        EditorGUILayout.PropertyField(itemEffectData);

        statusDatasList.DoLayoutList();

        EditorGUI.indentLevel++;

        
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(effectData);
        }

    }
}