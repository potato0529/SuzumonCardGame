using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGuideSeries : MonoBehaviour
{
    [Header("ガイドスライド")] public List<GameObject> guideSlides = new List<GameObject>();

    private int currentSlideIndex = 0;

    /// <summary>
    /// ガイドを開く
    /// </summary>
    public void ShowGuide()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ガイドを閉じる
    /// </summary>
    public void HideGuide()
    {
        gameObject.SetActive(false);
    }

    public void ShowSlide(int index)
    {
        guideSlides[currentSlideIndex].gameObject.SetActive(false);

        guideSlides[index].gameObject.SetActive(true);
        currentSlideIndex = index;
    }

    public void NextSlide()
    {
        if (currentSlideIndex < guideSlides.Count - 1)
        {
            ShowSlide(currentSlideIndex + 1);
        }
    }

    public void PreviousSlide()
    {
        if (currentSlideIndex > 0)
        {
            ShowSlide(currentSlideIndex - 1);
        }
    }
}
