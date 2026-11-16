using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMatchLog : MonoBehaviour
{
    public enum IntMatchData
    {
        SummonedCount,
        UsedItemCount,
        EffectInvoked,
        Attacked,
        Defenced,
        Destroyed,
        SentCemetery,
        ReceivedDamageTimes,
        ReceivedDamageSum,
        HealedLifeTimes,
        HealedLifeSum
    }

    private Dictionary<IntMatchData, int> intMatchDatas = new Dictionary<IntMatchData, int>();
    private Dictionary<IntMatchData, int> myTurnIntMatchDatas = new Dictionary<IntMatchData, int>();//毎自分ターン内でのカウント。今のところの仕様では、自分ターン開始時と終了時にリセットされる。

    private void Awake()
    {
        foreach(PlayerMatchLog.IntMatchData intMatchData in Enum.GetValues(typeof(PlayerMatchLog.IntMatchData)))
        {
            intMatchDatas[intMatchData] = 0;
            myTurnIntMatchDatas[intMatchData] = 0;
        }
    }

    /// <summary>
    /// int型でカウントされるマッチデータを加算する
    /// </summary>
    /// <param name="intMatchData"></param>
    /// <param name="value"></param>
    public void AddIntMatchData(IntMatchData intMatchData, int value)
    {
        intMatchDatas[intMatchData] += value;
        myTurnIntMatchDatas[intMatchData] += value;
    }

    /// <summary>
    /// intMatchDataのGetter
    /// </summary>
    /// <param name="intMatchData"></param>
    /// <returns></returns>
    public int GetIntMatchData(IntMatchData intMatchData)
    {
        return intMatchDatas[intMatchData];
    }

    /// <summary>
    /// turnIntMatchDataのGetter
    /// </summary>
    /// <param name="intMatchData"></param>
    /// <returns></returns>
    public int GetTurnIntMatchData(IntMatchData intMatchData)
    {
        return myTurnIntMatchDatas[intMatchData];
    }

    /// <summary>
    /// turnIntMatchDataを管理しているため、ターン終了時に呼ばれてリセットする
    /// </summary>
    public void OnTurnEnd()
    {
        foreach (PlayerMatchLog.IntMatchData intMatchData in Enum.GetValues(typeof(PlayerMatchLog.IntMatchData)))
        {
            myTurnIntMatchDatas[intMatchData] = 0;
        }
    }

    /// <summary>
    /// turnIntMatchDataを管理しているため、ターン開始時に呼ばれてリセットする
    /// </summary>
    public void OnTurnStart()
    {
        foreach (PlayerMatchLog.IntMatchData intMatchData in Enum.GetValues(typeof(PlayerMatchLog.IntMatchData)))
        {
            myTurnIntMatchDatas[intMatchData] = 0;
        }
    }
}
