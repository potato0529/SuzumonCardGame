using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Security.Cryptography;
using UnityEngine.UI;

public class DeckGeneratorFromDatas : MonoBehaviour
{
    [Header("Card_InDeckBoxのプレハブ")] public Card_InDeckBox cardIdbPrefab;
    [Header("DeckBoxDataのプレハブ")] public DeckBoxData deckBoxDataPrefab;
    [Header("カードデータベース")] public CardDataBase cardDataBase; //本来GameManager.instance.cardDataBaseでよいが、非再生時なのでGameManager.instance=nullになってしまっているのでデータベースが直接必要

    private void Start()
    {
        cardDataBase = GameManager.instance.cardDataBase;
    }

    public DeckBoxData ConvertCardDatasToDeckBoxData(List<CardData> _cardDatas)
    {
        DeckBoxData generatedDeckBoxData = Instantiate(deckBoxDataPrefab, transform);

        foreach (CardData cardData in _cardDatas)
        {
            Card_InDeckBox generatedCardIdb = Object.Instantiate(cardIdbPrefab, generatedDeckBoxData.transform);
            generatedCardIdb.cardData = cardData;
            generatedDeckBoxData.AddCard(generatedCardIdb);
        }

        return generatedDeckBoxData;
    }


    /// <summary>
    /// デッキIDをもとにdeckBoxDataを生成して返す
    /// </summary>
    /// <param name="_deckId"></param>
    /// <returns></returns>
    public DeckBoxData CopyTextValueDeck(string _deckId)
    {
        DeckBoxData generatedDeckBoxData = Object.Instantiate(deckBoxDataPrefab);
        string[] cardIds = _deckId.Split('/');

        generatedDeckBoxData.deckName = $"コピーデッキ({_deckId})";
        for (int i = 0; i < cardIds.Length; i++)
        {
            string cardIdText = cardIds[i];
            int cardId = int.Parse(cardIdText);
            Card_InDeckBox generatedDeckCard = Object.Instantiate(cardIdbPrefab);
            generatedDeckCard.cardData = cardDataBase.cardDatas[cardId];
            generatedDeckCard.GetComponent<Image>().sprite = generatedDeckCard.cardData.sprite;

            //デッキエディター内の場所を設定
            int columns = 10;
            int rowsNum = (i / columns);
            int width = 25;
            generatedDeckCard.positionInDeckBox = new Vector2(-210 + i * width - rowsNum * width * columns, 240 - rowsNum * 50);
            if (generatedDeckCard.cardData.belongDeckType == CardData.BelongDeckType.MainDeck)
            {
                generatedDeckBoxData.cards.Add(generatedDeckCard);
            }
            else
            {
                generatedDeckBoxData.specialCards.Add(generatedDeckCard);
            }

            generatedDeckCard.transform.SetParent(generatedDeckBoxData.transform);
        }
        //DeckBoxData deckAddedToUserData = GameManager.instance.userData.AddNewDeck(generatedDeckBoxData);AA
        //slotGenerator.EnumerateDecks();AA

        //なんか謎にうまくいかない
        //SelectedDeck(slotGenerator.GetDeckBoxImages()[GameManager.instance.userData.deckBoxDatas.IndexOf(deckAddedToUserData)]);AA
        return generatedDeckBoxData;
    }
}
