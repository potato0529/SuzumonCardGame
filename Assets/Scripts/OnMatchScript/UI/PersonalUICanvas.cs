using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalUICanvas : MonoBehaviour
{
    [Header("サイドカード情報キャンバス")] public MiniCardInfoCanvas miniCardInfoCanvas;
    [Header("カード情報キャンバス")] public CardInfoCanvas cardInfoCanvas;
    [Header("ドローカードを表示するイメージ")] public Image image;

    /// <summary>
    /// ドローフェイズにドローしたカードを表示する
    /// </summary>
    /// <param name="card"></param>
    public void ShowDrawCard(Card card)
    {
        image.gameObject.SetActive(true);
        image.sprite = card.cardData.sprite;
    }

    /// <summary>
    /// 表示したカードを非表示にする。
    /// </summary>
    public void HideDrawCard()
    {
        image.sprite = null;
        image.gameObject.SetActive(false);
    }

    /// <summary>
    /// サイドのカードインフォキャンバスを展開し、カード情報を表示する。
    /// </summary>
    public void ShowMiniCardInfo(Card card)
    {
        miniCardInfoCanvas.gameObject.SetActive(true);
        miniCardInfoCanvas.ShowCardInfo(card);
    }

    /// <summary>
    /// サイドのカードインフォキャンバスを閉じる。
    /// </summary>
    public void HideMiniCardInfo()
    {
        miniCardInfoCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// カードインフォキャンバスを閉じる。
    /// </summary>
    public void HideCardInfo()
    {
        cardInfoCanvas.Hide();
    }
}
