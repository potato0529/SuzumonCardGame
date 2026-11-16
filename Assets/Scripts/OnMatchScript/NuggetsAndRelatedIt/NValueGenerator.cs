using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NValueGenerator
{
    public List<OperatorUnit> operatorUnits = new List<OperatorUnit>();
    public int firstValue;
    private float calculatedValue;

    private void Start()
    {
        foreach (OperatorUnit operatorUnit in operatorUnits)
        {
            //operatorUnit.valueGenerator = this;

        }
    }

    /// <summary>
    /// 設定された情報をもとに効果の値を生成する
    /// </summary>
    public int GenerateValue(Card card)
    {
        float value = firstValue;
        foreach (OperatorUnit operatorUnit in operatorUnits)
        {
            operatorUnit.card = card;
            calculatedValue = operatorUnit.Calculate(card);

            switch (operatorUnit.operatorType)
            {
                case OperatorUnit.OperatorType.Plus:
                    value += calculatedValue;
                    break;

                case OperatorUnit.OperatorType.Minus:
                    value -= calculatedValue;
                    break;

                case OperatorUnit.OperatorType.Times:
                    value *= calculatedValue;
                    break;

                case OperatorUnit.OperatorType.Divided:
                    value /= calculatedValue;
                    break;
            }
        }

        Debug.Log("生成された値:" + value);
        return (int)value;
    }

    /// <summary>
    /// Calculate()で実行された時間差で値を受け取る処理ででた値を受け取る
    /// </summary>
    public void ReceiveValue(int result)
    {
        calculatedValue = result;
    }
}