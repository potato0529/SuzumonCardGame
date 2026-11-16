using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseDetailedProcess : ITriggerQueue
{
    private Player player;
    private List<EffectNugget> turnStartTriggerQueue = new List<EffectNugget>();
    private List<EffectNugget> turnEndTriggerQueue = new List<EffectNugget>();
    public List<Paralysis> paralysisStatuses = new List<Paralysis>();
    public List<Poison> poisonStatuses = new List<Poison>();
    public List<Preserved> preservedStatuses = new List<Preserved>();

    private bool decidedNoroiSuffer;

    public PhaseDetailedProcess(Player thisPlayer)
    {
        player = thisPlayer;
    }

    public IEnumerator TurnStartProcess()
    {
        for(int i= poisonStatuses.Count - 1;i>= 0;i--)
        {
            Poison poison = poisonStatuses[i];
            poison.DecreasePoison();
        }

        for (int i = preservedStatuses.Count - 1; i >= 0; i--)
        {
            Preserved preserved = preservedStatuses[i];
            preserved.DecreasePreservation();
        }

        TriggerEffectManager.instance.HappendPlayerTriggerMatter(player, EffectNugget.InvokeTiming.TurnStart);

        while (MatchManager.instance.runningProcess.ContainsKey(MatchManager.RunningProcessType.EffectProcess))
        {
            yield return null;
        }

        MatchManager.instance.phaseManager.EndDetailProcess();
    }

    /// <summary>
    /// ターン終了時の処理
    /// </summary>
    public IEnumerator TurnEndProcess()
    {
        TriggerEffectManager.instance.HappendPlayerTriggerMatter(player, EffectNugget.InvokeTiming.TurnEnd);
        while (MatchManager.instance.runningProcess.ContainsKey(MatchManager.RunningProcessType.EffectProcess))
        {
            yield return null;
        }

        List<Card> meetConditionTargets = player.fieldCards.cards;

        //呪いの数を集計
        int statusesSum = 0;

        foreach (Card card in meetConditionTargets)
        {
            if (card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Noroi))
            {
                statusesSum += card.cardStatuses.statusesDictionary[StatusData.StatusType.Noroi].StatusValue;
            }
        }

        //呪いが1以上あるときのみ処理を行う
        if (statusesSum >= 1 && player.handScr.hands.Count > 0)
        {
            List<Card> noroiSufferTargets = player.handScr.hands;
            player.cardPointer.SelectCard(statusesSum, false, noroiSufferTargets, CardPointer.SelectType.noroi, PassTargetsToNoroi, "呪いで捨てる手札");

            while (!decidedNoroiSuffer)
            {
                yield return null;
            }
            decidedNoroiSuffer = false;
        }

        //動けないステータスの処理
        for (int i = paralysisStatuses.Count - 1; i >= 0; i--)
        {
            Paralysis paralysis = paralysisStatuses[i];
            paralysis.DecreaseParalysis();
        }
        
        yield return null;

        MatchManager.instance.phaseManager.EndDetailProcess();
    }

    public void ChangeTriggerQueue(EffectNugget effectNugget,bool doAdd)
    {
        if (doAdd)
        {
            switch (effectNugget.invokeTiming)
            {
                case EffectNugget.InvokeTiming.TurnStart:
                    turnStartTriggerQueue.Add(effectNugget);
                    break;

                case EffectNugget.InvokeTiming.TurnEnd:
                    turnEndTriggerQueue.Add(effectNugget);
                    break;
            }
        }
        else
        {
            switch (effectNugget.invokeTiming)
            {
                case EffectNugget.InvokeTiming.TurnStart:
                    turnStartTriggerQueue.Remove(effectNugget);
                    break;

                case EffectNugget.InvokeTiming.TurnEnd:
                    turnEndTriggerQueue.Remove(effectNugget);
                    break;
            }
        }
        
    }

    //とりあえず呪いの影響の場合のみ　別の用途でもこのクラスで使う場合は要検討
    public void PassTargetsToNoroi(List<Card> targets)
    {
        foreach (Card target in targets)
        {
            target.ChangeCardState(Card.CardState.Cemetery);
        }
        decidedNoroiSuffer = true;
    }
}
