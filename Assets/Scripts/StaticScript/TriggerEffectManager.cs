using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerEffectManager : MonoBehaviour
{
    public static TriggerEffectManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 条件誘発効果で、発動条件を満たすまでの待機列
    /// </summary>
    private List<EffectNugget> triggerQueue = new List<EffectNugget>();

    private List<EffectNugget> playerTriggerQueue = new List<EffectNugget>();

    /// <summary>
    /// ターン終了時に発動する効果の待機列
    /// </summary>
    private List<TriggerInfo> turnEndTriggers = new List<TriggerInfo>();

    /// <summary>
    /// アンティークバーストの発動待機列
    /// </summary>
    public List<AntiqueBurst> antiqueQueue = new List<AntiqueBurst>();

    /// <summary>
    /// 発動条件を満たした誘発効果を、順番に処理していくための待機列
    /// </summary>
    private List<EffectNugget> invokeTriggerQueue = new List<EffectNugget>();

    private List<EffectNugget> adventedTriggerQueue = new List<EffectNugget>();

    #region アンティーク処理
    /// <summary>
    /// アンティークバーストが起動し、進行を開始するときキューに追加する
    /// </summary>
    /// <param name="triggerEffect"></param>
    public void AddAntiqueQueue(AntiqueBurst antiqueBurst)
    {
        Debug.Log($"アンティークバースト {antiqueBurst} を開始");
        antiqueQueue.Add(antiqueBurst);
    }

    /// <summary>
    /// アンティークバーストの値を1つ進める(お互いの毎ターン開始時に発動し、ゲーム内のアンティークバーストの値とは違い、その2倍の値で内部処理する)
    /// </summary>
    public void GoOnAntique()
    {
        Debug.Log("アンティークカウントを進めます");
        foreach (AntiqueBurst ab in antiqueQueue)
        {
            ab.goesOnTurn++;
            if (ab.startedTurnPlayer == MatchManager.instance.turnPlayer) ab.antiqueCount--;
            MatchManager.instance.antiqueBurstList.GoOnAntique(ab);

            Debug.Log($"{ab} の経過カウント:{ab.goesOnTurn}");
        }
        StartCoroutine(InvokeAntique());
    }

    /// <summary>
    /// 発動タイミングになったアンティークバーストを全て順番に発動する
    /// </summary>
    /// <returns></returns>
    IEnumerator InvokeAntique()
    {
        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.AntiqueBurst, null);

        bool hasInvoked = true;
        while (hasInvoked)
        {
            hasInvoked = false;
            foreach (AntiqueBurst ab in antiqueQueue)
            {
                //Debug.Log($"{ab.goesOnTurn} {ab.burstTime * 2}");
                if (ab.goesOnTurn >= ab.burstTime * 2)
                {
                    Debug.Log($"アンティークバースト {ab} のカウントが0になり、{ab.invokeEffectNugget} を起動します");
                    ab.invokeEffectNugget.AddTriggerQueue();
                    RemoveAntiqueQueue(ab);
                    ab.RemoveAntiqueBurst();
                    hasInvoked = true;

                    while (MatchManager.instance.runningProcess.ContainsKey(MatchManager.RunningProcessType.EffectProcess))
                    {
                        yield return null;
                    }
                    break;
                }
            }
        }

        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.AntiqueBurst);
    }

    public void RemoveAntiqueQueue(AntiqueBurst antiqueBurst)
    {
        MatchManager.instance.antiqueBurstList.RemoveAntiqueList(antiqueBurst);
        antiqueQueue.Remove(antiqueBurst);        
    }
    #endregion
    /// <summary>
    /// ターン終了時に発動する待機列にある効果を順番にすべて発動する
    /// </summary>
    /// <returns></returns>
    public IEnumerator InvokeTET()
    {
        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.TurnEndTriggerChecking,null);

        bool hasInvoked = true;
        while (hasInvoked)
        {
            hasInvoked = false;
            foreach (TriggerInfo tet in turnEndTriggers)
            {
                //え？ActionとかIsSurviveプロパティとか何も機能しなく根？大丈夫？
                tet.Action();
                if (!tet.IsSurvive)
                {
                    turnEndTriggers.Remove(tet);
                    hasInvoked = true;
                }

                //warn:ターン終了時自壊は効果だけど効果じゃないもの。要改善
                while (MatchManager.instance.runningProcess.ContainsKey(MatchManager.RunningProcessType.EffectProcess))
                {
                    yield return null;
                }
                break;
            }
        }

        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.TurnEndTriggerChecking);
    }

    #region 誘発待機処理
    /// <summary>
    /// 特定条件の誘発効果を持つカードが条件を満たすのを待つのみの状態になった場合キューに追加する
    /// </summary>
    /// <param name="triggerEffect">引数がnullだとしてもエラーを起こさずfalseを返してくれる</param>
    /// <returns>引数がnullでなければtrue,nullであればfalseを返す</returns>
    public void ChangeSpecialTriggerQueue(EffectNugget triggerEffect,bool doAdd)
    {
        if (doAdd)
        {
            triggerQueue.Add(triggerEffect);
        }
        else
        {
            triggerQueue.Remove(triggerEffect);
        }
    }

    public void HappenedTriggerMatter(Card focusCard, EffectNugget.InvokeTiming invokeTiming)
    {
        foreach(EffectNugget effectNugget in triggerQueue)
        {
            //Debug.Log(triggerQueue + "," + effectNugget.card);
            if(effectNugget.invokeTiming == invokeTiming)
            {
                foreach(Card meetConditionCard in effectNugget.invokeTimingObserveTarget.SearchTotalTargets(effectNugget.card))
                {
                    if(meetConditionCard == focusCard)
                    {
                        effectNugget.cardRelatedToTiming = focusCard;
                        effectNugget.AddTriggerQueue();
                    }
                }
            }
        }
    }

    public void HappendPlayerTriggerMatter(Player focusPlayer, EffectNugget.InvokeTiming invokeTiming)
    {
        foreach (EffectNugget effectNugget in playerTriggerQueue)
        {
            if (effectNugget.invokeTiming == invokeTiming)
            {
                //エラー これどうなったんかな　しばらく起きてないから治った可能性もある
                TargetCardConditions.TargetOwners targetOwner = effectNugget.invokeTimingObserveTarget.targetDecideConditions[0].targetConditionsNugget.overloadConditions[0].targetOwner;
                if ((targetOwner == TargetCardConditions.TargetOwners.Either) || (targetOwner == TargetCardConditions.TargetOwners.Myself && effectNugget.card.controlerPlayer == focusPlayer) || (targetOwner == TargetCardConditions.TargetOwners.Opponent && effectNugget.card.controlerPlayer != focusPlayer))
                {
                    effectNugget.AddTriggerQueue();
                }
                    
            }
        }
    }

    /// <summary>
    /// ターン終了時に発動する効果が待機中になった場合キューに追加する
    /// </summary>
    /// <param name="triggerEffect"></param>
    public void AddTurnEndTriggerQueue(TriggerInfo triggerInfo)
    {
        turnEndTriggers.Add(triggerInfo);
    }

    /// <summary>
    /// 条件を満たした誘発効果を待機列の最後尾に追加する ※EffectNugget.AddInvokeTrigger()でしか呼べないルールにした方が共通処理通れて安心なのでトリマそういう方向で。何か問題あったら頑張るしかない
    /// </summary>
    /// <param name="triggerEffect">待機させる処理</param>
    public void AddInvokeTriggerQueue(EffectNugget triggerEffect)
    {
        Debug.Log(triggerEffect + "を発動誘発待機列に追加します");
        invokeTriggerQueue.Add(triggerEffect);
        InvokeQueueTrigger();
    }

    /// <summary>
    /// 条件を満たした誘発効果の待機列を取得する
    /// </summary>
    /// <returns></returns>
    public List<EffectNugget> GetInvokeTriggerQueue()
    {
        return invokeTriggerQueue;
    }

    /// <summary>
    /// 可能な場合待機列を消化していく
    /// </summary>
    public void InvokeQueueTrigger()
    {
        //現在実行中の処理がなく、待機している処理がある
        //もし条件が増えたらInvokeQueueTriggerの呼び出しタイミングは見直したほうがいい
        if (MatchManager.instance.CanInvokeTriggerEffect() && invokeTriggerQueue.Count > 0)
        {
            //最前列の処理を起動するためのインターフェースを取得

            Debug.Log("待機中の誘発処理 " + invokeTriggerQueue[0] + " を起動します");
            //最前列の処理を起動
            invokeTriggerQueue[0].UseEffectOrder();
            //起動した処理をキューから取り除く
            invokeTriggerQueue.Remove(invokeTriggerQueue[0]);
            if(MatchManager.instance.CanInvokeTriggerEffect() && invokeTriggerQueue.Count > 0)
            {
                InvokeQueueTrigger();
            }
        }
    }
    #endregion

    #region 様々なゲーム内の出来事の信号を受け取る
    public void ChangeTriggerQueue(EffectNugget effectNugget,bool doAdd)
    {
        if (doAdd)
        {
            playerTriggerQueue.Add(effectNugget);
        }
        else
        {
            playerTriggerQueue.Remove(effectNugget);
        }
    }

    #endregion
}
