using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerOverInvisibleUI : MonoBehaviour
{
    [Header("不透明度")] public float opacity;
    [Header("共に適用するオブジェクト")] public Image objectToApplyWith;
    private Color originColor;
    public Image image;

    private void Awake()
    {
        originColor = image.color;
    }

    public void PointerEnter()
    {
        image.color = new Color(0, 0, 0, opacity);
        objectToApplyWith.color = new Color(0, 0, 0, opacity);
    } 

    public void PointerExit()
    {
        image.color = originColor;
        objectToApplyWith.color = originColor;
    }
}
