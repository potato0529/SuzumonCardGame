using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : MonoBehaviour
{
    /// <summary>
    /// カードスクリプトを受け取り、そのカードの情報と条件の値を照らし合わせる。
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public virtual bool FulfillConditions(Card card)
    {
        return true;
    }

    
}

public static class ConditionCommon
{
    public enum EqualSigns
    {
        Equal,
        AGreater,
        AGreaterOrEqual,
        BGreater,
        BGreaterOrEqual
    }

    /// <summary>
    /// 二つの値を比較する
    /// </summary>
    /// <param name="equalSigns"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static bool ComparisonValues(EqualSigns equalSigns, int value1, int value2)
    {
        bool result = false;

        switch (equalSigns)
        {
            case EqualSigns.Equal:
                result = value1 == value2;
                break;

            case EqualSigns.AGreater:
                result = value1 > value2;
                break;

            case EqualSigns.AGreaterOrEqual:
                result = value1 >= value2;
                break;

            case EqualSigns.BGreater:
                result = value1 < value2;
                break;

            case EqualSigns.BGreaterOrEqual:
                result = value1 <= value2;
                break;

            default:
                return true;
        }
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Condition, $"比較条件: {value1} {equalSigns} {value2} = {result}");

        return result;
    }
}