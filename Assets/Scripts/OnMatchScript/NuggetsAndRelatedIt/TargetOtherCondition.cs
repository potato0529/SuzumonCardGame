using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetOtherCondition : MonoBehaviour
{
    public abstract bool DoesTargetFulfillConditions(Card userCard, Card target);
}
