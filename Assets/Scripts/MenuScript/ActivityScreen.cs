using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityScreen : MonoBehaviour
{
    [Header("アクティビティ対応キャンバス")] public Canvas activityCanvas;
    
    /// <summary>
    /// このアクティビティスクリーンを表示する
    /// </summary>
    public virtual void DisplayScreen()
    {
        Debug.Log($"{activityCanvas} を表示します");
        activityCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// このアクティビティスクリーンを非表示にする
    /// </summary>
    public virtual void HiddenScreen()
    {
        Debug.Log($"{activityCanvas} を非表示にします");
        activityCanvas.gameObject.SetActive(false);
    }
}
