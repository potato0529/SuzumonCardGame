using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GuideOpacity : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [Header("不透明度")] public float opacity;

    private Color originColor;
    public Image image;
    public TMP_Text guideText;
    public Image cardImage;

    private void Awake()
    {
        originColor = image.color;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        image.color = new Color(0, 0, 0, opacity);
        guideText.color = new Color(1, 1, 1, opacity);
        //if (cardImage) cardImage.color = new Color(1, 1, 1, opacity); ※cardImageは不透明度変えなくていいかな　前までは変えてたからこの処理あるけど
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        image.color = originColor;
        guideText.color = new Color(1, 1, 1, 1);
        //if (cardImage) cardImage.color = new Color(1, 1, 1, 1); ※cardImageは不透明度変えなくていいかな　前までは変えてたからこの処理あるけど
    }

    
}
