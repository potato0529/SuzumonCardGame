using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardStatuses : MonoBehaviour
{
    [Header("カード")] public Card card;

    [Header("ステータスリスト")] public Dictionary<StatusData.StatusType,Status> statusesDictionary = new Dictionary<StatusData.StatusType, Status>();

    private void Awake()
    {
        //statuses = new Status[Enum.GetValues(typeof(StatusData.StatusType)).Length];
    }

    /// <summary>
    /// ステータスを追加するか変更する※BaseStatusValueに適用
    /// </summary>
    /// <param name="statusType"></param>
    /// <param name="value"></param>
    /// <param name="isSet">一定値に固定する場合true、足して変動させる場合false</param>
    /// <param name="dontDestroyTiming">このメソッドでのステータス値変更時に、ステータスが削除される条件を満たしていても破壊しないか</param>
    public StatusType SetStatusValue<StatusType>(StatusData.StatusType statusType, int value, bool isSet, bool dontDestroyTiming)
        where StatusType : Status
    {
        //Debug.Log($"ステータス {statusType} をセットします");

        StatusType statusUnique = null;
        Status status = null;
        if (!statusesDictionary.ContainsKey(statusType))//!card.GetComponent(statusType.GetStatusType()) || 
        {
            //card.gameObject.AddComponent(statusType.GetStatusType());//この直後Statusのそれぞれの派生クラスのAwakeによってそれぞれ決まったstatusesのインデックスに代入される
            //status = card.cardStatuses.statuses[(byte)statusType];
            //int statusValue = status.StatusValue;
            //SuzumonTools.SeparateSetOrAdd(ref statusValue, value, isSet);
            ////次のステータス変更処理で誤ってステータスが削除されないための処理
            //if (dontDestroyTiming)
            //    status.BeginDontDestroyTiming();

            //status.AddedStatus(statusType, statusValue,card);
            UtilityLog.CardLog($"{statusType} ステータスを追加し、値{value} を与えました", card, UtilityLog.LogCategory.none);

            //新版
            statusUnique = Activator.CreateInstance(statusType.GetStatusType()) as StatusType;
            status = statusUnique;
            status.Initialize(card);
            int statusValue1 = status.BaseStatusValue;//元status1.StatusValue ダメだったら戻そう
            SuzumonTools.SeparateSetOrAdd(ref statusValue1, value, isSet);            
            status.AddedStatus(statusType, statusValue1,MatchManager.instance.statusDataBase.statusInfos[(byte)statusType]);
            card.cardStatuses.statusesDictionary[statusType] = status;
        }
        else
        {
            //status = card.cardStatuses.statuses[(byte)statusType];
            //int statusValue = status.StatusValue;
            //SuzumonTools.SeparateSetOrAdd(ref statusValue, value, isSet);
            ////次のステータス変更処理で誤ってステータスが削除されないための処理
            //if (dontDestroyTiming)
            //    status.BeginDontDestroyTiming();

            //status.ChangedValue(statusValue);
            //Debug.Log($"{statusType} ステータスを {value} 追加しました(現在:{status.StatusValue})");

            //新版
            statusUnique = card.cardStatuses.statusesDictionary[statusType] as StatusType;
            status = statusUnique;
            int statusValue1 = status.BaseStatusValue;//元status1.StatusValue ダメだったら戻そう
            SuzumonTools.SeparateSetOrAdd(ref statusValue1, value, isSet);
            //次のステータス変更処理で誤ってステータスが削除されないための処理
            if (dontDestroyTiming)
                status.BeginDontDestroyTiming();

            status.BaseStatusValue = statusValue1;
            //status1.ChangedValue(statusValue1); 自身あんまないからダメだったら戻そう
            UtilityLog.CardLog($"{statusType} ステータスを {value} 追加しました(現在:{status.StatusValue})", card, UtilityLog.LogCategory.none);
        }

        //次のステータス変更処理で誤ってステータスが削除されないための処理
        if (dontDestroyTiming)
            status.EndDontDestroyTiming();

        return statusUnique;
    }

    /// <summary>
    /// 変更したいステータスを対象が持っていなければ追加処理をしてからgivedStatusを追加する
    /// </summary>
    /// <param name="addStatusEffect"></param>
    /// <param name="isSet"></param>
    /// <returns></returns>
    public Status SetGivedStatus(Effect addStatusEffect)
    {
        Status status = SetStatusValue<Status>(addStatusEffect.statusType, 0, false,true);
        status.AddGivedStatus(addStatusEffect);
        return status;
    }

    /// <summary>
    /// 保存されているステータスを追加する
    /// </summary>
    /// <param name="value"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    public Preserved AddPreserved(int value, Card card)
    {
        //statuses[(byte)StatusData.StatusType.Preserved] = gameObject.AddComponent<Preserved>();
        ////Debug.Log(card.statusIconsManager.statusIcons[5] + "おおおおおおおおおおおおおおおおおおおお");
        ////card.statusIconsManager.statusIcons[5].AddedStatusScript(preserved);
        //statuses[(byte)StatusData.StatusType.Preserved].SetValue(value);
        //statuses[(byte)StatusData.StatusType.Preserved].GetComponent<Preserved>().preservingParent = card;
        //return statuses[(byte)StatusData.StatusType.Preserved].GetComponent<Preserved>();
        return new Preserved();
    }

    /// <summary>
    /// 保存しているステータスを追加する
    /// </summary>
    /// <param name="value"></param>
    /// <param name="preservedCards"></param>
    public void AddPreserving(int value, List<Card> preservedCards)
    {
        //statuses[(byte)StatusData.StatusType.Preserving] = gameObject.AddComponent<Preserving>();
        //foreach(Card preservedCard in preservedCards)
        //{
        //    statuses[(byte)StatusData.StatusType.Preserved] = preservedCard.cardStatuses.AddPreserved(value, card);
        //    statuses[(byte)StatusData.StatusType.Preserving].GetComponent<Preserving>().AddPreservedCard(preservedCard);
        //}
        //card.statusIconsManager.statusIcons[(byte)StatusData.StatusType.Preserving].LinkStatusScript(statuses[(byte)StatusData.StatusType.Preserving]);
        //statuses[(byte)StatusData.StatusType.Preserving].SetValue(value);
    }

    /// <summary>
    /// nullでないステータスを全て返す（この処理ステータスの種類増えるごとに重くなるんじゃね...)
    /// </summary>
    /// <returns></returns>
    public List<Status> GetActiveStatuses()
    {
        List<Status> activeStatuses = new List<Status>();

        //foreach(Status status in statuses)
        //{
        //    if (status)
        //    {
        //        activeStatuses.Add(status);
        //    }
        //}

        foreach (KeyValuePair<StatusData.StatusType,Status> status in statusesDictionary)
        {
            
            activeStatuses.Add(status.Value);
            
        }
        return activeStatuses;
    }

    /// <summary>
    /// ステータスアイコンを生成する
    /// </summary>
    /// <returns></returns>
    public StatusIcon GenerateStatusIcon()
    {
        return Instantiate(GameManager.instance.assetsDatas.statusIconPrefab);
    }
}
