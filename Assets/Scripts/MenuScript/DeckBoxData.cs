using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class DeckBoxData : MonoBehaviour
{
    [Header("投入カード(メインデッキ)")] public List<Card_InDeckBox> cards = new List<Card_InDeckBox>();
    [Header("投入カード(スペシャル)")] public List<Card_InDeckBox> specialCards = new List<Card_InDeckBox>();
    [Header("デッキ名")] public string deckName;
    [Header("デッキイメージ")] public DeckBoxImage boxImage;

    public List<Card_InDeckBox> AllDeckCards
    {
        get 
        {
            List<Card_InDeckBox> allCards = cards.ToList();
            allCards.AddRange(specialCards);
            return allCards;
        }
    }

    /// <summary>
    /// デッキカードのデータをもとにデータを生成し、デッキの子オブジェクトにする
    /// </summary>
    /// <param name="card_InDeck"></param>
    public void AddCardDataToDeck(Card_InDeckBox baseData)
    {
        Card_InDeckBox deckCardData = Instantiate(GameManager.instance.deckCardPrefab.gameObject).GetComponent<Card_InDeckBox>();
        //生成データの設定
        deckCardData.cardData = baseData.cardData;
        deckCardData.transform.SetParent(DeckEditorManager.instance.canvas.transform);
        deckCardData.GetComponent<Image>().sprite = baseData.cardData.sprite;
        deckCardData.belongDeck = DeckEditorManager.instance.editingBox;
        Debug.Log($"{deckCardData.belongDeck} に所属します");
        deckCardData.belongDeck.cards.Add(deckCardData);
        
        deckCardData.transform.SetParent(deckCardData.belongDeck.transform);
        Debug.Log(DeckEditorManager.instance.editingBox);
        Debug.Log(deckCardData + "が複製されました");
        cards.Add(deckCardData);
    }

    /// <summary>
    /// 指定カードがデッキ内にいくつあるか返す
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public int GetInDeckCardNum(CardData cardData)
    {
        int cardNum = 0;
        foreach(Card_InDeckBox card_InDeckBox in cards)
        {
            if(card_InDeckBox.cardData == cardData)
            {
                cardNum++;
            }
        }
        return cardNum;
    }

    /// <summary>
    /// Card_InDeckBoxのインスタンスをデッキに追加する。メインデッキに属するカードならメインデッキのリスト、スペシャルデッキならスペシャルデッキのリストに追加する
    /// </summary>
    /// <param name="cardData"></param>
    public void AddCard(Card_InDeckBox card_Idb) 
    {
        if (card_Idb.cardData.belongDeckType == CardData.BelongDeckType.MainDeck)
        {
            cards.Add(card_Idb);
        }
        else if(card_Idb.cardData.belongDeckType == CardData.BelongDeckType.SpecialDeck)
        {
            specialCards.Add(card_Idb);
        }
    }
}
