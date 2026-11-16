using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniCardInfoCanvas : MonoBehaviour,IHyperLink
{
    public Canvas canvas;
    [Header("カード情報キャンバス")] public CardInfoCanvas cardInfoCanvas;
    [Header("表示中のカード")] public Card card;
    [Header("表示中のカードデータ")] public CardData cardData;
    [Header("カードイメージ")] public Image cardImage;
    [Header("カードタイプ")] public TMP_Text cardTypeText;
    [Header("カード名")] public TMP_Text cardNameText;
    [Header("ランク")] public TMP_Text cardRankText;
    [Header("属性")] public TMP_Text cardAttributeText;
    [Header("カテゴリ")] public TMP_Text cardCategoryText;
    [Header("効果テキスト欄")] public TMP_Text effectText;
    [Header("ガイドUI")] public TMP_Text uniqueTextGuideText;

    /// <summary>
    /// カードの情報を表示する
    /// </summary>
    /// <param name="card"></param>
    public void ShowCardInfo(CardData showCardData)
    {
        cardData = showCardData;
        cardImage.sprite = cardData.sprite;
        cardTypeText.text = cardData.GetDetailCardType().GetJapaneseNameOfDetailCategory();
        cardNameText.text = cardData.cardName;

        if (cardData.cardType == Card.CardType.Monster)
        {
            cardRankText.text = "ランク" + cardData.rank;
            cardAttributeText.text = cardData.attribute.GetJapaneseNameOfAttribute() + "属性";
        }
        else
        {
            cardRankText.text = "";
            cardAttributeText.text = "";
        }

        //カテゴリテキスト生成
        string categoryText = "カテゴリ:";
        foreach(Card.Category category in cardData.categories)
        {
            categoryText += category.GetJapaneseNameOfCategory() + "/";
        }

        if (cardData.categories.Count == 0) categoryText = "";
        cardCategoryText.text = categoryText;

        effectText.text = cardData.effectText;

        //テキスト生成
        effectText.text = GameManager.instance.uniqueTextManager.ConvertDescriptedText(cardData.effectText);
    }

    /// <summary>
    /// カードの情報を表示する cardを引数に指定したいとき用のオーバーロード。とはいえ呼び出す側がcardDataにして渡してもいいしこの機能が要るかはわからん
    /// </summary>
    /// <param name="showCard"></param>
    public void ShowCardInfo(Card showCard)
    {
        card = showCard;
        ShowCardInfo(showCard.cardData);
    }

    public void OpenCardInfoWindow()
    {
        cardInfoCanvas.gameObject.SetActive(true);
        cardInfoCanvas.ShowCardInfo(card);
    }

    public void EnterLink(string linkId)
    {
        uniqueTextGuideText.transform.parent.gameObject.SetActive(true);
        Vector2 MousePos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uniqueTextGuideText.transform.parent.parent.gameObject.GetComponent<RectTransform>(),
            Input.mousePosition,
            canvas.worldCamera,
            out MousePos);
        uniqueTextGuideText.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            MousePos.x + 700,
            MousePos.y);
        uniqueTextGuideText.text = GameManager.instance.uniqueTextManager.GetDescriptionCorrespondingToLink(linkId);
    }

    public void ExitLink(string linkId)
    {
        uniqueTextGuideText.text = "";
        uniqueTextGuideText.transform.parent.gameObject.SetActive(false);
    }

    public void ClickLink(string linkId)
    {
        EnterLink(linkId);
    }
}
