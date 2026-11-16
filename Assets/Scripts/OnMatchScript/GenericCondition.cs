using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCondition : Condition
{
    [Header("ğŒ’l1")] public VariableInt value1;
    [Header("”äŠr")] public ConditionCommon.EqualSigns equalSigns;
    [Header("ğŒ’l2")] public VariableInt value2;
    public override bool FulfillConditions(Card card)
    {
        return ConditionCommon.ComparisonValues(equalSigns, value1.GetValue(card), value2.GetValue(card));
    }
}
