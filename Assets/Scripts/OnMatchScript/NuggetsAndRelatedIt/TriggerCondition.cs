using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerCondition
{
    public EffectNugget effectNugget;

    private Card card;
    private bool isWaitState;
    public Action<EffectNugget,bool> registeredTriggerQueueDelegate;

    public TriggerCondition(EffectNugget thisEffectNugget)
    {
        effectNugget = thisEffectNugget;
        card = effectNugget.card;
        effectNugget.triggerCondition = this;
        ChangedCardState(card.cardState);
    }

    /// <summary>
    /// カードのCardStateが変更されたとき、誘発の待機条件に応じて待機状態を更新する
    /// </summary>
    /// <param name="cardState">変更された後のカードのCardState</param>
    public void ChangedCardState(Card.CardState cardState)
    {
        Debug.Log("TriggerCondition CardStateが変更されました");
        bool matchCondition = false;
        //いずれかの条件が一致すれば待機状態とする
        foreach(Card.CardState stateCondition in effectNugget.invokeCardState)
        {
            if (cardState == stateCondition)
            {
                matchCondition = true;
                break;
            }
        }
        if(effectNugget.invokeCardState.Length == 0)
        {
            matchCondition = true;
        }

        if (matchCondition)
        {
            WaitState();
        }
        else
        {
            NonWait();
        }
                
    }

    /// <summary>
    /// 待機状態とする
    /// </summary>
    public void WaitState()
    {
        Debug.Log(effectNugget + "を待機状態とします");
        
        //非待機状態が待機状態に更新されるとき、キューに追加する
        if (!isWaitState)
        {
            Action<EffectNugget,bool> triggerQueue = null;
            
            //プレイヤー環境条件でもいけそうなのであれば共通処理にする　処理も違うし引数も違うからさすがにきついかも
            if (effectNugget.invokeTiming.IsCardTiming())
            {
                triggerQueue = TriggerEffectManager.instance.ChangeSpecialTriggerQueue;
            }
            else if(effectNugget.invokeTiming.IsSurrondingTiming())
            {
                triggerQueue = TriggerEffectManager.instance.ChangeTriggerQueue;
            }
            
            triggerQueue(effectNugget,true);
            registeredTriggerQueueDelegate = triggerQueue;
        }
        isWaitState = true;
    }

    /// <summary>
    /// 非待機状態とする
    /// </summary>
    public void NonWait()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.EffectNuggetProcess, effectNugget + "を非待機状態とします");
        //待機状態が非待機状態に更新されるとき、キューから削除する
        if (isWaitState)
        {
            registeredTriggerQueueDelegate(effectNugget, false);
            registeredTriggerQueueDelegate = null;
            
        }
        isWaitState = false;
    }
}
