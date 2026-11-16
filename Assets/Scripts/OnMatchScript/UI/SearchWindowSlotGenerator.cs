using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchWindowSlotGenerator : CardSlotsGenerator
{
    [Header("サーチ")] public SearchCard searchCard;
    [Header("サーチウィンドウカードプレハブ")] public SearchWindowCard searchWindowCardPrefab;
    private List<Card> sourceCards = new List<Card>();
    public List<SearchWindowCard> searchWindowCards = new List<SearchWindowCard>();

    /// <summary>
    /// 与えられたカードを元にサーチウィンドウカードを生成して並べる
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public List<SearchWindowCard> EnumerateCards(List<Card> cards)
    {
        sourceCards = cards;
        searchWindowCards = GenerateAndEnumerateCards<SearchWindowCard, Card>(searchWindowCardPrefab, sourceCards);
        //カードを種類順に並べる
        for(int index = 0;index < searchWindowCards.Count;index++)
        {
            SearchWindowCard searchWindowCard = searchWindowCards[index];
            searchWindowCard.searchCard = searchCard;
            searchWindowCard.sourceCard = sourceCards[index];
        }
        return searchWindowCards;
    }

    public void RemoveSearchWindowCard(Card sourceCard)
    {
        for (int index = 0; index < searchWindowCards.Count; index++)
        {
            SearchWindowCard searchWindowCard = searchWindowCards[index];
            if(searchWindowCard.sourceCard == sourceCard)
            {
                searchWindowCard.DisplaySelectedMark();
            }
        }
    }
}
