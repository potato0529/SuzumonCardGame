using System;
using UnityEngine;
[Serializable]
public class VariableInt
{
    public ValueType valueType;
    public int fixedValue;
    public ValueGenerator valueGenerator;

    public enum ValueType
    {
        FixedValue,
        ValueGenerator
    }

    public int GetValue(Card card)
    {
        if (valueType == ValueType.FixedValue)
        {
            return fixedValue;
        }
        else
        {
            return valueGenerator.GenerateValue(card);
        }
    }
}