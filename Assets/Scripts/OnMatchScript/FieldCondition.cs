using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCondition : Condition
{
    [Header("フィールド値")] public int fieldNum;
    public override bool FulfillConditions(Card card)
    {
        return FieldManager.instance.NowFieldNum == fieldNum;
    }
}
