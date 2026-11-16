using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfoCanvas_DeckEditor : MonoBehaviour
{
    [Header("カードイメージ")] public Image cardImage;
    [Header("カードタイプ")] public TMP_Text cardTypeText;
    [Header("カード名")] public TMP_Text cardNameText;
    [Header("ランク")] public TMP_Text cardRankText;
    [Header("属性")] public TMP_Text cardAttributeText;
    [Header("カテゴリ")] public TMP_Text cardCategoryText;
    [Header("効果テキスト欄")] public TMP_Text effectText;
    /// <summary>
    /// カードの情報を表示する
    /// </summary>
    /// <param name="card"></param>
    public void ShowCardInfo(CardData cardData)
    {
        cardImage.sprite = cardData.sprite;
        cardTypeText.text = cardData.GetDetailCardType().GetJapaneseNameOfDetailCategory();
        cardNameText.text = cardData.cardName;
        cardRankText.text = "ランク" + cardData.rank;
        cardAttributeText.text = cardData.attribute.GetJapaneseNameOfAttribute() + "属性";

        //カテゴリテキスト生成
        string categoryText = "";
        foreach (Card.Category category in cardData.categories)
        {
            categoryText += category.GetJapaneseNameOfCategory() + "/";
        }
        cardCategoryText.text = categoryText;

        effectText.text = cardData.effectText;
    }

    public void Disassemble()
    {
        //PossessionCard possessionCard = GameManager.instance.userData.possessionCards[]
    }
}
