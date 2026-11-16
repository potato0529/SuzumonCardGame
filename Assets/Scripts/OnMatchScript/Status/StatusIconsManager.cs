using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// それぞれのアイコンの有効・無効が切り替わった場合必ずリストは更新される
/// </summary>
public class StatusIconsManager : MonoBehaviour
{
    [Header("カード")] public Card card;
    [Header("ステータスアイコンスロット")] public GameObject[] statusIconSlots;
    [Header("StatusIcons(ステータスアイコンたちの親オブジェクト)")] public GameObject statusIconsParent;
    [Header("ステータスアイコンリスト(StatusTypeEnum)")] public StatusIcon[] statusIcons = new StatusIcon[Enum.GetValues(typeof(StatusData.StatusType)).Length];
    [Header("表示可能なステータスアイコンリスト")] private List<StatusIcon> enableIcons = new List<StatusIcon>();
    [Header("アクティブなステータスアイコンリスト")] public List<StatusIcon> activeIcons = new List<StatusIcon>();
    [Header("優先度最大値(大きいほど優先順位は低い")] public int maximumPriolity;

    public bool isOpen;

    private void Awake()
    {
        statusIcons = new StatusIcon[Enum.GetValues(typeof(StatusData.StatusType)).Length];
    }

    /// <summary>
    /// 有効なアイコンのみを適切な表示順に並べ替え、activeIconsとして表示する。無効なアイコンは取り除く。
    /// </summary>
    public void SortIconList()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, "アイコンを並べ替え、表示します");
        //優先順位に基づいてリストを並べ替える
        for (int loops = 0; loops <= maximumPriolity; loops++)
        {
            foreach (StatusIcon enableIcon in enableIcons)
            {
                //優先順位が一致したアイコンをアクティブリストにソートする
                if(enableIcon.displayPriority == loops)
                {
                    enableIcon.DisplayIcon();
                }
            }
        }

        if (activeIcons.Count != 0)
        {
            for (int count = activeIcons.Count - 1; count >= 0; count--)
            {
                if (!activeIcons[count].EnabledIcon)
                {
                    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, activeIcons[count] + "は無効じゃよ");
                    activeIcons[count].HiddenIcon();
                }
            }
        }

        Vector3 cardPosOnScreen = card.controlerPlayer.cameraScr.WorldToScreenPoint(card.transform.position);

        //アクティブリストのそれぞれのアイコンの座標を設定する
        for(int loops = 0;loops < activeIcons.Count;loops++)
        {            
             activeIcons[loops].gameObject.transform.position = statusIconSlots[loops].transform.position;//error?

            if(!activeIcons[loops].objToUGUI.linked)
                activeIcons[loops].objToUGUI.GenerateUGUI();

            if (activeIcons[loops].status != null) //pin status
            {
                activeIcons[loops].objToUGUI.UpdateInfo(activeIcons[loops].status.StatusValue);
            }
        }
    }

    /// <summary>
    /// カードが出て、表示可能なステータスアイコンを表示するモードとなる
    /// </summary>
    public void OpenIcons()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, "アイコンを開きます");
        isOpen = true;
        SortIconList();
    }

    /// <summary>
    /// カードが場から離れ、アイコンを表示できないモードとなり、表示しているステータスアイコンを非表示にする
    /// </summary>
    public void CloseIcons()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, "アイコンを閉じます");
        isOpen = false;
        for(int index = activeIcons.Count - 1;index >= 0; index--)
        {
            activeIcons[index].HiddenIcon();
        }
    }

    /// <summary>
    /// enableIconsのリスト版セッター(Add)
    /// </summary>
    /// <param name="statusIcon"></param>
    public void AddEnableList(StatusIcon statusIcon)
    {
        enableIcons.Add(statusIcon);
        if (isOpen)
        {
            SortIconList();
        }
    }

    /// <summary>
    /// enableIconsのリスト版セッター(Remove)
    /// </summary>
    /// <param name="statusIcon"></param>
    public void RemoveEnableList(StatusIcon statusIcon)
    {
        enableIcons.Remove(statusIcon);
        if (isOpen)
        {
            SortIconList();
        }
    }

    /// <summary>
    /// ※アイコンがStatusの情報を取得できるまでは使っちゃだめよ
    /// </summary>
    /// <param name="status"></param>
    /// <param name="effectStatusIconObj"></param>
    //public void AddUniqueStatus(Status statusType, int statusValue,GameObject effectStatusIconObj)
    //{
    //    Debug.Log("固有ステータスの初期設定をします");
    //    effectStatusIconObj.transform.SetParent(statusIconsParent.transform);
    //    UniqueStatusIcon statusIcon = effectStatusIconObj.GetComponent<UniqueStatusIcon>();
    //    statusIcon.LinkStatusScript(statusType);
    //    statusIcon.AddedUniqueIcon(this);
    //    statusType.SetValue(statusValue);
    //}

    /// <summary>
    /// ※アイコンがStatusの情報を取得できるまでは使っちゃだめよ
    /// </summary>
    /// <param name="status"></param>
    /// <param name="effectStatusIconObj"></param>
    public void AddStatus(Status status, int statusValue, StatusIcon effectStatusIcon)
    {
        //Debug.Log("ステータスの初期設定をします");
        effectStatusIcon.transform.SetParent(statusIconsParent.transform);
        effectStatusIcon.LinkStatusScript(status);
        effectStatusIcon.statusIconsManager = this;
        //status.SetValue(statusValue);
        status.BaseStatusValue = statusValue;//アイコンのクラスで値設定しないといけないのちょっとなぁ...
    }

    /// <summary>
    /// ステータスタイプに対応するstatusIconsのインデックス番号を返す
    /// </summary>
    /// <param name="checkStatus"></param>
    /// <returns></returns>
    public int GetIndexNumCorrespoindingStatus(Type checkStatus)
    {
        //af:もともとStatus型からGetTypeしていたが、直接Typeを受け取るようにしたので、もしそれで不都合があったらオーバーロードで2パターン用意する。
        for(int index = 0;index < statusIcons.Length;index++)
        {
            //if(checkStatus == statusIcons[index].statusType)
            {
                return index;
            }
        }
        throw new System.Exception($"指定されたタイプ {checkStatus} はステータスアイコンリストに存在しませんでした");
    }
}
