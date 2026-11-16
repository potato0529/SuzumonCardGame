using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialDeck : MonoBehaviour
{
    [Header("スペシャルデッキ内のカード")] private List<Card> cards = new List<Card>();
    [Header("スペシャルデッキスロットジェネレーター")] public CardSlotsGenerator cardSlotsGenerator;
    [Header("スペシャルデッキキャンバス")] public Canvas specialDeckCanvas;
    [Header("スペシャルデッキリストに表示するカードプレハブ")] public InMatchListedUICard inMatchListedUICardPrefab;
    public SearchCard searchCard;

    public void SpecialDeckInit()
    {
        ArrangementDeck();
        specialDeckCanvas.gameObject.SetActive(true);
        specialDeckCanvas.gameObject.SetActive(false);
    }

    public List<Card> GetSpecialDeckCards()
    {
        return cards;
    }

    public void AddSpecialDeck(Card card)
    {
        cards.Add(card);
        ArrangementDeck();
        InMatchListedUICard inMatchListedUICard = cardSlotsGenerator.GenerateListedCard(inMatchListedUICardPrefab, card);
        inMatchListedUICard.sourceCard = card;        
        cardSlotsGenerator.ArrangeCards();
    }

    public void LeaveSpecialDeck(Card card)
    {
        UtilityLog.CardLog("スペシャルデッキを離れます", card, UtilityLog.LogCategory.ChangeCardState);
        int thisIndex = cards.IndexOf(card);
        cards.Remove(card);
        cardSlotsGenerator.RemoveListedCard(thisIndex);
        cardSlotsGenerator.ArrangeCards();
    }

    /// <summary>
    /// デッキのカードの座標をカードリストの順番に従って変更して並べる 
    /// </summary>
    public void ArrangementDeck()
    {
        float y = 0.1f;
        for (int deckNum = cards.Count - 1; deckNum >= 0; deckNum--)
        {
            y += cards[0].body.transform.lossyScale.y;
            cards[deckNum].transform.position = transform.position + new Vector3(0, y, 0);
            cards[deckNum].SetCardRotation(new Vector3(90, 0, 0));
            cards[deckNum].cardState = Card.CardState.SpecialDeck;
        }
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.OtherSystem, "スペシャルデッキをリスト順にソートしました");
    }

    public void OpenSpecialWindow()
    {
        specialDeckCanvas.gameObject.SetActive(true);
        if (!searchCard.isActiveAndEnabled)
        {
            searchCard.gameObject.SetActive(true);

            List<Card> availableSpCards = new List<Card>();

            foreach (Card card in cards)
            {
                if (card.cardData.specialCardData.canSelfSummon && card.controlerPlayer.IsRemainUseRights(card) && card.CanUseCheck())
                {
                    availableSpCards.Add(card);
                }
            }

            searchCard.SpecialDeckSummon(availableSpCards, SpecialUsed);
        }
    }

    /// <summary>
    /// スペシャルウィンドウが閉じられたときに呼び出される(ref+1)
    /// </summary>
    public void CloseSpecialWindow()
    {
        //スペシャルデッキから直接展開されているときのみ閉じる処理を行う　※選択処理の途中で展開されているときにその処理が強制終了しないようにするため
        if (searchCard.searchMode == SearchCard.SearchMode.DirectSpecialDeck)
        {
            searchCard.EndSearch();
        }
    }

    /// <summary>
    /// スペシャルデッキからカードを使用された
    /// </summary>
    /// <param name="usedCards"></param>
    public void SpecialUsed(List<Card> usedCards)
    {

    }
}
