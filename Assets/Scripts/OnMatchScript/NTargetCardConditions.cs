using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class NTargetCardConditionsNugget
{
    [Header("条件の候補"), SerializeField] public TargetCardConditions[] overloadConditions = new TargetCardConditions[0];

    [HideInInspector] public TotalTargetDecideNugget decideNugget;

    public void DebugLogTargetConditionsNugget()
    {
        foreach (TargetCardConditions condition in overloadConditions)
        {
            condition.DebugLogConditions();
        }
    }

    /// <summary>
    /// このターゲット条件だとサーチウィンドウを展開する必要があるか調べる
    /// </summary>
    /// <returns></returns>
    public bool DoNeedSearchWindow()
    {
        foreach (TargetCardConditions targetCardConditions in overloadConditions)
        {
            if (targetCardConditions.targetCardState == Card.CardState.Deck || targetCardConditions.targetCardState == Card.CardState.Cemetery || targetCardConditions.targetCardState == Card.CardState.SpecialDeck)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 関数で条件を設定してオーバーロードコンディションのうちどれか1つでもその条件に当てはまるか調べる
    /// </summary>
    /// <param name="funcToCheck"></param>
    /// <returns></returns>
    public bool ConditionsCheck(Func<TargetCardConditions, bool> funcToCheck)
    {
        foreach (TargetCardConditions targetCardConditions in overloadConditions)
        {
            if (funcToCheck(targetCardConditions))
            {
                return true;
            }
        }
        return false;
    }
}
