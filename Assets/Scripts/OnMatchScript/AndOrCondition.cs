using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndOrCondition : Condition
{
    [Header("èåè1")] public Condition condition1;
    [Header("AndÇ≈Ç†ÇÈÅifalseÇ»ÇÁorÅj")] public bool isAndCondition = true;
    [Header("èåè2")] public Condition condition2;

    public override bool FulfillConditions(Card card)
    {
        Debug.Log($"{condition1.FulfillConditions(card)},{condition2.FulfillConditions(card)}");
        if (isAndCondition)
        {
            
            return condition1.FulfillConditions(card) && condition2.FulfillConditions(card);
        }
        else
        {
            return condition1.FulfillConditions(card) || condition2.FulfillConditions(card);
        }
    }
}
