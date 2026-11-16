using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkedUGUI : MonoBehaviour
{
    [HideInInspector] public ObjToUGUI LinkSource;

    private RectTransform rectTransform;
    [HideInInspector] public Image image;
    [HideInInspector] public Player viewThisPlayer;
    public GameObject hadValueChild;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    /// <summary>
    /// リンク元のオブジェクトの座標が更新されたときこのUGUIのスクリーン上の座標をそれに基づいて更新する
    /// </summary>
    public void UpdatePosition()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, "UIの座標を更新します");
        rectTransform.position =viewThisPlayer.cameraScr.WorldToScreenPoint(LinkSource.transform.position);
    }
}
