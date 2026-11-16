using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchWindowButton : MonoBehaviour
{
    [Header("サーチウィンドウ")] public SearchCard searchCard;
    public Button button;
    public Sprite openImage;
    public Sprite closeImage;
    public Image buttonImage;

    public void Clicked()
    {
        if(searchCard.hidden)
        {
            searchCard.hidden = false;
            searchCard.ReOpenWindow();
            //text.text = "Close";
            buttonImage.sprite = closeImage;
        }
        else
        {
            searchCard.HideWindow();
            //text.text = "Open";
            buttonImage.sprite = openImage;
        }
        
    }

    /// <summary>
    /// サーチ処理が終了してリセットされ、ボタンは消える
    /// </summary>
    public void CloseButton()
    {
        //text.text = "Close";
        buttonImage.sprite = closeImage;
        gameObject.SetActive(false);
    }
}
