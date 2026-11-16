using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiqueBurstList : MonoBehaviour
{
    public InMatchListedUICard listedUICardPrefab;
    private List<AntiqueBurst> antiqueQueue = new List<AntiqueBurst>();
    public EitherAntiqueBurstList[] eitherAntiqueBursts = new EitherAntiqueBurstList[2];

    private void Start()
    {
        antiqueQueue = TriggerEffectManager.instance.antiqueQueue;
        eitherAntiqueBursts[0].gameObject.SetActive(true);
        eitherAntiqueBursts[0].gameObject.SetActive(false);
        eitherAntiqueBursts[1].gameObject.SetActive(true);
        eitherAntiqueBursts[1].gameObject.SetActive(false);
    }

    /// <summary>
    /// アンティークバーストが開始し、リストに追加される
    /// </summary>
    /// <param name="antiqueBurst"></param>
    public void StartAntiqueBurst(AntiqueBurst antiqueBurst)
    {
        eitherAntiqueBursts[0].StartAntiqueBurst(antiqueBurst);
        eitherAntiqueBursts[1].StartAntiqueBurst(antiqueBurst);
    }

    /// <summary>
    /// アンティークバーストが発動してアンティークリストから取り除かれる 今は使えない
    /// </summary>
    /// <param name="antiqueBurst"></param>
    public void InvokedAntiqueBurst(AntiqueBurst antiqueBurst)
    {
        RemoveAntiqueList(antiqueBurst);
    }

    /// <summary>
    /// アンティークバーストが中断してアンティークリストから取り除かれる 今は使えない
    /// 
    /// </summary>
    public void CanceledAntiqueBurst(AntiqueBurst antiqueBurst)
    {
        RemoveAntiqueList(antiqueBurst);
    }

    public void RemoveAntiqueList(AntiqueBurst antiqueBurst)
    {
        Card sourceCard = antiqueBurst.invokeEffectNugget.card;
        int thisIndex = antiqueQueue.IndexOf(antiqueBurst);
        eitherAntiqueBursts[0].RemoveAntiqueList(antiqueBurst, thisIndex);
        eitherAntiqueBursts[1].RemoveAntiqueList(antiqueBurst, thisIndex);
    }

    public void GoOnAntique(AntiqueBurst antiqueBurst)
    {
        int thisIndex = antiqueQueue.IndexOf(antiqueBurst);
        eitherAntiqueBursts[0].GoOnAntique(antiqueBurst,thisIndex);
        eitherAntiqueBursts[1].GoOnAntique(antiqueBurst,thisIndex);
    }
}
