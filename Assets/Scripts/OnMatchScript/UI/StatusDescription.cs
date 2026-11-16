using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusDescription : MonoBehaviour
{
    [Header("ステータスガイド")] public GameObject statusDescription;
    [Header("ステータスガイドテキスト")] public TMP_Text statusDescriptionText;
    [Header("リンクしているステータス")] public StatusIcon statusIcon;

    /// <summary>
    /// ステータスの説明を表示するref+1
    /// </summary>
    public void DisplayStatusDescription()
    {
        statusDescription.SetActive(true);
        statusDescriptionText.text = statusIcon.GetStatusDescription();
    }

    public void HideStatusDescription()
    {
        statusDescription.SetActive(false);
        statusDescriptionText.text = "";
    }
}
