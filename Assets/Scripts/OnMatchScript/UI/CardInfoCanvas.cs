using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoCanvas : MonoBehaviour
{
    [Header("カードイメージ")] public Image cardImage;
    [Header("カード名")] public Text cardNameText;
    [Header("ランク")] public Text cardRankText;
    [Header("属性")] public Text cardAttributeText;
    [Header("カテゴリ")] public Text cardCategoryText;
    [Header("効果テキスト欄")] public Text effectText;
    [Header("重なったカードリストジェネレーター")] public CardSlotsGenerator piledCardsSlotGenerater;
    [Header("重なったカードリストプレハブ")] public ListedPiledCard listedPiledCard;
    /// <summary>
    /// カードの情報を表示する
    /// </summary>
    /// <param name="card"></param>
    public void ShowCardInfo(Card card)
    {
        cardImage.sprite = card.cardData.sprite;
        cardNameText.text = card.cardData.cardName;
        cardRankText.text = "ランク" + card.cardData.rank;
        cardAttributeText.text = card.cardData.attribute.GetJapaneseNameOfAttribute() + "属性";

        //カテゴリテキスト生成
        string categoryText = "";
        foreach (Card.Category category in card.cardData.categories)
        {
            categoryText += category.GetJapaneseNameOfCategory() + "/";
        }
        cardCategoryText.text = categoryText;
        effectText.text = card.cardData.effectText;

        List<ListedPiledCard> listedPiledCards = piledCardsSlotGenerater.GenerateAndEnumerateCards<ListedPiledCard, Card>(listedPiledCard, card.cardLayer.layer);
        for(int index = 0; index < listedPiledCards.Count; index++)
        {
            listedPiledCards[index].topCard = card;
            listedPiledCards[index].sourceCard = card.cardLayer.layer[index];
            foreach(Weapon equippedWeapon in card.equippedWeapons)
            {
                if(equippedWeapon.card == listedPiledCards[index])
                {
                    listedPiledCards[index].isEquippingWeapon = true;
                }
            }
        }

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
