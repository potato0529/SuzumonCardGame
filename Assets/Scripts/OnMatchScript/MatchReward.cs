using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchReward : MonoBehaviour
{
    [Header("経験値表示")] public Image expView;
    [Header("ランク表示")] public TMP_Text rankView;
    void Start()
    {
        UserData userData = GameManager.instance.userData;
        rankView.text = "Rank" + userData.userRank;
        expView.GetComponent<RectTransform>().localScale = new Vector3(userData.GetNeedExpPercentage(), 1, 1);
    }

    /// <summary>
    /// 報酬画面を最前面に表示する
    /// </summary>
    public void ViewReward()
    {
        transform.SetSiblingIndex(1);
    }
}
