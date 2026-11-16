using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OneShotNotice : MonoBehaviour
{
    [Header("フェードアウト開始時間")] public float startFadeTime;
    [Header("1フレームのフェード率")] public float fadeScale;
    [Header("フェードするイメージ")] public Image image;
    [Header("フェードするテキスト")] public TMP_Text text;

    private float fadingTime = -1;

    public void StartFade()
    {
        gameObject.SetActive(true);
        fadingTime = 0;
        image.color = new Color(0, 0, 0, 0.8f);
        text.color = new Color(1, 1, 1, 0.8f);
    }

    private void Update()
    {
        if (fadingTime < 0) return;
        
        fadingTime += Time.deltaTime;
        if(fadingTime >= startFadeTime)
        {
            image.color -= new Color(0, 0, 0, fadeScale);
            text.color -= new Color(0, 0, 0, fadeScale);

            if (image.color.a <= 0)
            {
                fadingTime = -1;
                gameObject.SetActive(false);

            }
        }
    }
}
