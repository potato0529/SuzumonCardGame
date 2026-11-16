using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenericEffectNugget : EffectNugget
{
    [Header("効果")] public List<Effect> effects;
    [Header("コスト")] public List<Cost> costs;

    [HideInInspector] public Effect nowEffect;
    [HideInInspector] public Cost nowCost;

    public enum ActivationTiming
    {
        summon,
        actionPhase,
        attack,
        attackMonster,
        attackPlayer,
        breaked,
        antiqueBurst
    }

    protected override void Awake()
    {
        //cardは既にcardDataの処理で入っているので問題ない。Awake起動前に非アクティブにして無理やりタイミング調整するのもきもいけど多分Startだと上手くいかないようになってると思うからしゃあない
        base.Awake();

        //環境誘発効果だった場合
        if (invokeTiming.IsSurrondingTiming() || (invokeTiming.IsCardTiming() && invokeTimingObserveTarget.targetDecideConditions.Length > 0))
        {
            triggerCondition = new TriggerCondition(this);
        }
        else
        {
            card.effectNuggets[((byte)invokeTiming)] = this;
        }

        if (invokeTimingObserveTarget.targetDecideConditions.Length == 0)
        {
            invokeTimingObserveTarget.targetDecideConditions = new TargetDecideCondition[1];
            invokeTimingObserveTarget.targetDecideConditions[0] = new TargetDecideCondition();
            invokeTimingObserveTarget.targetDecideConditions[0].targetConditionType = TargetDecideCondition.TargetConditionType.Myself;
            invokeTimingObserveTarget.targetDecideConditions[0].targetConditionsNugget = new TargetCardConditionsNugget();
        }
        
        card.SetChangedCardStateCB(ChangedCardState);
    }

    public override bool CanUseCheck()
    {
        return turnUsedNum < turnCanUseNum && CanUseEffectAll() && CanCostAll() && (!condition || condition.FulfillConditions(card) && MeetStateCondition());
    }

    public override void UseEffectOrder()
    {
        if (CanUseCheck())
        {
            UtilityLog.CardLog("効果処理を実行できます", card, UtilityLog.LogCategory.EffectNuggetProcess);
            DecidedUseEffect();
            InvokeEffect();
        }
    }

    public override void UseEffectOrder(Action callback)
    {
        if (CanCostAll() && CanUseEffectAll() && condition.FulfillConditions(card))
        {
            Debug.Log("効果処理を実行できます");
            callbackAction = callback;
            existCallback = true;
            DecidedUseEffect();
            InvokeEffect();
        }
    }

    
    /// <summary>
    /// ナゲット内の全てのコストを払うことができるかチェックする
    /// </summary>
    /// <returns>全てのコストを払うことができるか否か</returns>
    public bool CanCostAll()
    {
        if(costs.Count == 0)
        {
            Debug.Log("コストは不要です");
            return true;
        }

        byte nowCost = 0;

        foreach (Cost cost in costs)
        {
            if (!EffectManager.instance.CanCostCheck(ownerPlayer,this,card,cost))
            {
                Debug.Log("払うことのできないコストが見つかりました");
                return false;
            }
            nowCost++;
        }
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Cost,"全てのコストを払うことができます");
        return true;
    }
    
    /// <summary>
    /// ナゲット内の全ての効果のうち1つ以上使えるか確認する。
    /// </summary>
    /// <returns>使用かの名効果があるか否か</returns>
    public bool CanUseEffectAll()
    {
        if(effects.Count == 0)
        {
            Debug.Log("効果がありません");
            return true;
        }

        byte nowEffectNum = 0;

        foreach(Effect effect in effects)
        {
            Effect.EffectTypes thisEffectType = effect.effectType;
            nowEffect = effects[nowEffectNum];
            nowEffectNum++;
            if (EffectManager.instance.CanUseEffectCheck(ownerPlayer,this,card,effect))
            {
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.Effect,"効果が使えます");
                return true;
            }
            else
            {
                Debug.Log("使えない効果があります");
            }
        }
        Debug.Log("使える効果がありませんでした");
        return false;
    }

    /// <summary>
    /// 発動場所の条件を確認する
    /// </summary>
    /// <returns></returns>
    public bool MeetStateCondition()
    {
        if (invokeCardState.Length == 0)
        {
            return true;
        }

        foreach(Card.CardState cardStateCondition in invokeCardState)
        {
            if(card.cardState == cardStateCondition)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 1つの効果が完了したときの共通処理
    /// </summary>
    public void UseEffectCP()
    {
        compUsedEffect = true;
    }

    /// <summary>
    /// 一連の効果処理
    /// </summary>
    /// <returns></returns>
    public override IEnumerator InvokeEffectNugget()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.EffectNuggetProcess,name + "の効果処理を開始します-----------------------------------------------");
        MatchManager.instance.turnUsedEffects.Add(this);
        byte nowCostNum = 0;
        foreach (Cost cost in costs)
        {
            paidCost = false;
            EffectManager.instance.PayCost(ownerPlayer,this,card,cost);
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Cost, "コスト支払いの待機中...");
            while (!paidCost)
            {
                yield return null;
            }
            nowCostNum++;
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Cost,nowCostNum + "個目のコスト完了");
        }

        UtilityLog.CategorizedLog(UtilityLog.LogCategory.EffectNuggetProcess, "コスト処理が終わりました");

        yield return new WaitForSeconds(0.1f);
        byte nowEffectNum = 0;

        foreach (Effect effect in effects)
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.EffectNuggetProcess,nowEffectNum + 1 + "個目の効果(" + effect.effectType + ")開始");
            nowEffect = effects[nowEffectNum];
            compUsedEffect = false;
            //効果の適用条件を満たしていて、効果が1種類のみ(効果が1つでも使用可能でなければ効果を発動できてないから、効果を使えることがすでに証明されている)か、この効果が使用可能である
            if ((!effect.condition || effect.condition.FulfillConditions(card)) && (effects.Count == 1 || EffectManager.instance.CanUseEffectCheck(ownerPlayer,this, card,effect)))
            {
                EffectManager.instance.UseEffect(ownerPlayer, this, card, effect);
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.EffectNuggetProcess, "効果選択の待機中...");
                //効果が完了するまで待つ(効果がない場合はスキップ)
                while (!compUsedEffect && effect.effectType != Effect.EffectTypes.none)
                {
                    //UtilityLog.CategorizedLog(UtilityLog.LogCategory.EffectNuggetProcess, effect.transform.parent.gameObject.name + "の" + effect + "の選択待機中");
                    yield return null;
                }
            }
            else
            {
                Debug.Log("効果が使えませんでした");
            }
            Debug.Log(nowEffectNum + 1 + "個目の効果完了");
            nowEffectNum++;
        }
       
        //子クラス(EffectNugget)に信号を送る
        CompEffectProcess();
    }
    
    public override string AutoGenerateEffectNuggetText()
    {
        string text = "";

        foreach (Cost cost in costs)
        {
            string mainValueText = "";
            if (cost.mainValue.valueType == VariableInt.ValueType.FixedValue)
            {
                mainValueText += cost.mainValue.fixedValue;
            }
            else
            {
                //むずしぎ
            }

            switch (cost.costType)
            {
                case Cost.Costs.hand:
                    text += $"手札を{mainValueText}枚捨て、";
                    break;

                case Cost.Costs.life:
                    text += $"ライフを{mainValueText}払い、";
                    break;

                case Cost.Costs.fieldMonster:
                    text += $"場のモンスターを{mainValueText}体コストとし、";
                    break;
            }
            text += "\n";
        }
        foreach (Effect effect in effects)
        {
            string mainValueText = "";
            if(effect.mainValue.valueType == VariableInt.ValueType.FixedValue)
            {
                mainValueText += effect.mainValue.fixedValue;
            }
            else
            {
                //むずしぎ
            }

            switch (effect.effectType)
            {
                case Effect.EffectTypes.Draw:
                    text += mainValueText + "枚ドローする。";
                    break;

                case Effect.EffectTypes.DestroyCard:
                    text += mainValueText + "体破壊する。";
                    break;

                case Effect.EffectTypes.Damage:
                    text += mainValueText + "ダメージ与える。";
                    break;
            }
            text += "\n";
        }
        return text;
    }
}
