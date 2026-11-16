using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistMonsterCondition :Condition
{
    [Header("カウントするカード条件")] public TotalTargetDecideNugget decideNugget;
    public ConditionCommon.EqualSigns equalSigns;
    public VariableInt variableInt;

    public override bool FulfillConditions(Card card)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Condition, "ExistMonsterCondition.FulfillConditions");
        return ConditionCommon.ComparisonValues(equalSigns, decideNugget.SearchTotalTargets(card).Count, variableInt.GetValue(card));
    }
}
