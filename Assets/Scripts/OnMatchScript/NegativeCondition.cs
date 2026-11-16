using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeCondition : Condition
{
    [Header("”Û’è‚·‚éğŒ")] public Condition condition;

    public override bool FulfillConditions(Card card)
    {
        return !condition.FulfillConditions(card);
    }
}
