using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeBlocker : GenericEffectNugget
{
    [Header("デュフォルトライフブロッカー値")] public VariableInt defaultLifeBlockerValue;
    [Header("ライフブロッカー値")] public int lifeBlockerValue;
    [Header("成功時効果")] public EffectNugget effectNugget; //この変数は消してInvokeTimingに追加するかも //そもそも成功時効果をこのナゲットが持てばよくねｗｗｗだったらGeneric形継承か

    protected override void Awake()
    {
        base.Awake();
        card.lifeBlockerEffect = this;
    }
}
