using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// イベントデータの各要素のinspector表示をカスタマイズするためのDrawerクラス
/// </summary>
[CustomPropertyDrawer(typeof(OperatorUnit))]
public class OperatorUnitDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // List用に1つのプロパティであることを示すためPropertyScopeで囲む
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            // 0指定だとReorderableListのドラッグと被るのでLineHeightを指定
            position.height = EditorGUIUtility.singleLineHeight;

            var operatorTypeRect = new Rect(position)
            {
                y = position.y
            };
            var operatorTypeProperty = property.FindPropertyRelative("operatorType");
            operatorTypeProperty.enumValueIndex = EditorGUI.Popup(operatorTypeRect, "演算子のタイプ", operatorTypeProperty.enumValueIndex, Enum.GetNames(typeof(OperatorUnit.OperatorType)));

            if ((OperatorUnit.OperatorType)operatorTypeProperty.enumValueIndex != OperatorUnit.OperatorType.Absolute)
            {

                var valueSourceRect = new Rect(operatorTypeRect)
                {
                    y = operatorTypeRect.y + EditorGUIUtility.singleLineHeight + 2f
                };

                var valueSourceProperty = property.FindPropertyRelative("valueSource");
                valueSourceProperty.enumValueIndex = EditorGUI.Popup(valueSourceRect, "参照する値の種類", valueSourceProperty.enumValueIndex, Enum.GetNames(typeof(OperatorUnit.ValueSource)));

                switch ((OperatorUnit.ValueSource)valueSourceProperty.enumValueIndex)
                {
                    case OperatorUnit.ValueSource.FixedValue:
                        var termValueRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };

                        var termValueProperty = property.FindPropertyRelative("termValue");
                        termValueProperty.floatValue = EditorGUI.FloatField(termValueRect, "項の値", termValueProperty.floatValue);
                        break;

                    case OperatorUnit.ValueSource.MeetConditionTargetsNum:
                        var conditionsNuggetRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var conditionsNuggetProperty = property.FindPropertyRelative("countCardsConditionsNugget");
                        EditorGUI.PropertyField(conditionsNuggetRect, conditionsNuggetProperty);
                        break;

                    case OperatorUnit.ValueSource.StatusSumOfTargets:

                        var checkStatusTypeRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var checkStatusTypeProperty = property.FindPropertyRelative("checkStatusType");
                        EditorGUI.PropertyField(checkStatusTypeRect, checkStatusTypeProperty);

                        var decideNuggetRect = new Rect(checkStatusTypeRect)
                        {
                            y = checkStatusTypeRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var decideNuggetProperty = property.FindPropertyRelative("tallyStatusesTargetConditions");
                        EditorGUI.PropertyField(decideNuggetRect, decideNuggetProperty);
                        break;

                    case OperatorUnit.ValueSource.UsedEffectResult:
                        EditorGUI.indentLevel++;
                        var referenceEffectRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var referenceEffectProperty = property.FindPropertyRelative("referenceEffect");
                        EditorGUI.PropertyField(referenceEffectRect, referenceEffectProperty);
                        var usedEffectValueTypeRect = new Rect(referenceEffectRect)
                        {
                            y = referenceEffectRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var usedEffectValueTypeProperty = property.FindPropertyRelative("usedEffectValueType");
                        usedEffectValueTypeProperty.enumValueIndex = EditorGUI.Popup(usedEffectValueTypeRect, "使われた効果の値の種類", usedEffectValueTypeProperty.enumValueIndex, Enum.GetNames(typeof(OperatorUnit.UsedEffectValueType)));
                        break;

                    case OperatorUnit.ValueSource.DecideTargetsNum:
                        var totalDecideNuggetRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var totalDecideNuggetProperty = property.FindPropertyRelative("tallyStatusesTargetConditions");
                        EditorGUI.PropertyField(totalDecideNuggetRect, totalDecideNuggetProperty);
                        break;

                    case OperatorUnit.ValueSource.RankSumOfTarget:

                        var rankDecideNuggetRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var rankDecideNuggetProperty = property.FindPropertyRelative("tallyStatusesTargetConditions");
                        EditorGUI.PropertyField(rankDecideNuggetRect, rankDecideNuggetProperty);
                        break;

                    case OperatorUnit.ValueSource.DetailType:

                        var dTypeDecideNuggetRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var dTypeDecideNuggetProperty = property.FindPropertyRelative("tallyStatusesTargetConditions");
                        EditorGUI.PropertyField(dTypeDecideNuggetRect, dTypeDecideNuggetProperty);
                        break;

                    case OperatorUnit.ValueSource.PlayerMatchLog:
                        var playerMatchLogRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var playerMatchLogProperty = property.FindPropertyRelative("intMatchData");
                        EditorGUI.PropertyField(playerMatchLogRect, playerMatchLogProperty);
                        break;

                    case OperatorUnit.ValueSource.PlayerTurnLog:
                        var playerTurnLogRect = new Rect(valueSourceRect)
                        {
                            y = valueSourceRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };
                        var playerTurnLogProperty = property.FindPropertyRelative("turnIntMatchData");
                        EditorGUI.PropertyField(playerTurnLogRect, playerTurnLogProperty);
                        break;
                }
            }
            
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUIUtility.singleLineHeight * 2;

        var valueSourceProperty = property.FindPropertyRelative("valueSource");
        switch ((OperatorUnit.ValueSource)valueSourceProperty.enumValueIndex)
        {
            case OperatorUnit.ValueSource.FixedValue:
                height = 60;
                break;

            case OperatorUnit.ValueSource.UsedEffectResult:
                height = 80;
                break;

            case OperatorUnit.ValueSource.MeetConditionTargetsNum:
            case OperatorUnit.ValueSource.DecideTargetsNum:
            case OperatorUnit.ValueSource.RankSumOfTarget:
            case OperatorUnit.ValueSource.DetailType:
            case OperatorUnit.ValueSource.PlayerMatchLog:
            case OperatorUnit.ValueSource.PlayerTurnLog:
                height = 60; //af:リストの要素数で可変にします　気が向いたら
                break;

            case OperatorUnit.ValueSource.StatusSumOfTargets:
                height = 80;
                break;
        }

        return height;
    }
}

