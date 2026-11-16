using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettings : MonoBehaviour
{
    [Header("メニュー背景")] public GameObject menuBG;
    [Header("対戦ログコピーNotice")] public OneShotNotice copiedCommandHistoryNotice;

    public void OpenMenu()
    {
        menuBG.SetActive(true);
    }

    public void CloseMenu()
    {
        menuBG.SetActive(false);
    }

    public void Surrender()
    {
        MatchManager.instance.localPlayer.Surrender();
    }

    /// <summary>
    /// 「対戦ログをコピー」ボタンがクリックされる ref+1
    /// </summary>
    public void OnClickedGetCommandHistory()
    {
        string serializedCommandHistory = MatchManager.instance.HistoryRecorder.GetSerializedCommandHistory();
        copiedCommandHistoryNotice.StartFade();
        GUIUtility.systemCopyBuffer = serializedCommandHistory;
        UtilityLog.DebugLog($"{serializedCommandHistory} をコピーしました");
    }
}
