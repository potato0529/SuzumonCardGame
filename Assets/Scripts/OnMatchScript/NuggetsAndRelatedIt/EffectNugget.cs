using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Cardクラスが配列で持ち、発動タイミングを持ち、効果を発動したりコストを払ったりして処理するナゲット。
/// </summary>
public abstract class EffectNugget : MonoBehaviour,IENVariable,IOwnerPlayer,IInEffectText,IUsedTargetResult
{
    [Header("ターン内使用可能回数")] public int turnCanUseNum = 1;
    [Header("発動タイミング")] public InvokeTiming invokeTiming;
    [Header("発動場所")] public Card.CardState[] invokeCardState; //現在は常時ナゲットのみ確認。今後はMyTurnなどに必要
    public TotalTargetDecideNugget invokeTimingObserveTarget;

    public enum InvokeTiming
    {
        Appear,
        MyTurn,
        Destroyed,
        SentCementery,
        BeforeAttack,
        AfterAttack,
        BeforeBeAttacked,
        AfterWasAttacked,
        BeforeAttackMonster,
        AfterAttackMonster,
        BeforeAttackPlayer,
        AfterAttackPlayer,
        LifeBlock,
        AppearCost,
        AttackCost,
        AntiqueBurst,
        ChosenEffect,
        Other,
        Bounced,
        BeParalysed,
        TurnStart,
        TurnEnd,
        ReceivedDamage,
        HealedLife,
        Advented,
        LifeBlocked,
        BeforeDestroyed,
        ChangedField,
        Withstand,
        Poisoned,
        Preserved,
        DestroyedByPoison,
        CostForExemptionFromUseRight,
        Defenced,
        Equiped,
        EscapeDestroyTiming
    }


    [HideInInspector] public Player ownerPlayer;
    protected bool paidCost;
    protected bool compUsedEffect;
    
    [HideInInspector] public int turnUsedNum;

    [HideInInspector] public TriggerCondition triggerCondition;

    public Card card { get; set; }
    [Header("効果の使用条件")] public Condition condition;

    public Action callbackAction;
    protected bool existCallback;
    public Card cardRelatedToTiming;

    /// <summary>
    /// 必ず継承先はbaseを使わなければならない
    /// </summary>
    protected virtual void Awake()
    {
        //EffectNuggetは固有カードでもない限りカードデータから生成される際にカードが代入されるし、SPSelectは持ち主のカードはないから、外部クラスにアクセスするとはいえ、Awakeで処理してよいといえる。カードの効果として生成以外の方法で使わないものと、その他はSPSelectしかない

        //この処理は必ずStart()でなければならない(かも)
        ownerPlayer = card.ownerPlayer;
        
    }

    /// <summary>
    /// TriggerEffectManagerの効果発動待機列に加える
    /// </summary>
    public void AddTriggerQueue()
    {
        if (!card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved))
        {
            TriggerEffectManager.instance.AddInvokeTriggerQueue(this);
        }
    }

    public abstract void UseEffectOrder();

    
    public void DecidedUseEffect()
    {
        ownerPlayer.playerCanvas.personalUICanvas.HideCardInfo();
    }

    public abstract void UseEffectOrder(Action callback);

    public abstract bool CanUseCheck();

    /// <summary>
    /// 効果を発動し、このナゲットの処理を開始する
    /// </summary>
    public void InvokeEffect()
    {
        turnUsedNum++;
        card.controlerPlayer.playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.EffectInvoked, 1);
        StartCoroutine(UseEffectNuggetAnim());
    }

    /// <summary>
    /// 効果を発動したときのアニメーションを行い、その後効果を実行する
    /// </summary>
    /// <returns></returns>
    protected IEnumerator UseEffectNuggetAnim()
    {
        UtilityLog.CardLog(card.name + "の効果発動アニメーションを行います", card, UtilityLog.LogCategory.UI);
        Debug.Log(card.name + "の効果発動アニメーションを行います");
        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.EffectProcess,new RunningProcess(ownerPlayer));//もしかしたら別の場所に移動する可能性あり

        MatchManager.instance.matchSound.effectInvoke.PlayOneShot(MatchManager.instance.matchSound.effectInvoke.clip);

        foreach (Player player in MatchManager.instance.players)
        {
            player.playerCanvas.personalUICanvas.ShowMiniCardInfo(card);
        }

        //アニメーション本体
        Vector3 cardOriginPosition = card.transform.position;//演出前のカードの座標

        GameObject animObj = Instantiate(card.invokeEffectAnimObj);
        animObj.transform.position = transform.position + new Vector3(0,2,0); // 0 2 0
        SpriteRenderer animRenderer = animObj.GetComponent<SpriteRenderer>();
        animRenderer.sprite = card.GetComponent<SpriteRenderer>().sprite;
        
        while (animRenderer.color.a >= 0)
        {
            animObj.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * 0.7f;
            animRenderer.color += new Color(0, 0, 0, -0.0175f) * 0.7f;
            //Debug.Log(animObj.transform.localScale + "あ");

            //カードが浮く
            if (animRenderer.color.a >= 1.2f)//0.8f
                card.transform.position += new Vector3(0, 0.3f, 0);

            if (animRenderer.color.a <= 0.3f && card.transform.position.y > cardOriginPosition.y)//0.2f
                card.transform.position += new Vector3(0, -0.3f, 0);

            yield return new WaitForSecondsRealtime(MatchManager.instance.matchConfig.effectAnimFps);
        }
        card.transform.position = cardOriginPosition;

        //旧版
        //while (animRenderer.color.a >= 0)
        //{
        //    animObj.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        //    animRenderer.color += new Color(0, 0, 0, -0.007f);
        //    if (animRenderer.color.a <= 0.45f)
        //        animRenderer.color += new Color(0, 0, 0, -0.015f);
        //    yield return null;
        //}
        Destroy(animObj);

        StartCoroutine(InvokeEffectNugget());
    }

    public abstract IEnumerator InvokeEffectNugget();
    
    /// <summary>
    /// この効果を持つカードの場所が更新される
    /// </summary>
    public void ChangedCardState(Card.CardState cardState)
    {
        //triggerConditionを改良すればEffectNuggetのChangedCardState()は不要になるかも？
        //誘発待機条件があった場合そこに知らせる
        if (triggerCondition != null)
        {
            triggerCondition.ChangedCardState(cardState);
        }
    }

    public abstract string AutoGenerateEffectNuggetText();
    
    #region IENVariable用
    public int GetTurnUsedNum()
    {
        return turnUsedNum;
    }

    public void SetTurnUsedNum(int num)
    {
        turnUsedNum = num;
    }

    public void PaidCost()
    {
        paidCost = true;
        CompOneProcess();
    }

    public void CompUsedEffect()
    {
        Debug.Log("1つの効果が終わりました");
        compUsedEffect = true;
        CompOneProcess();
    }

    /// <summary>
    /// 効果やコストなど、1つの処理が完了する
    /// </summary>
    public void CompOneProcess()
    {
        MatchManager.instance.UpdatedGameStatus();
    }

    public void CompEffectProcess()
    {
        //コールバックが必要な場合
        if (existCallback)
        {
            callbackAction.Invoke();
        }
        else
        {
            Debug.Log("コールバックはありません");
        }
        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.EffectProcess);
        Debug.Log("効果処理完了！-------------------------------------------------------");
    }
    #endregion

    public Player GetPlayer()
    {
        return ownerPlayer;
    }

    public List<Card> GetTargetedResultCards()
    {
        List<Card> cardsToRelatedToTiming = new List<Card>();
        cardsToRelatedToTiming.Add(cardRelatedToTiming);
        return cardsToRelatedToTiming;
    }
}
