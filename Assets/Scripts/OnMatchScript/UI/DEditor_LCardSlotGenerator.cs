using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEditor_LCardSlotGenerator : CardSlotsGenerator
{
    [Header("リステッドカードプレハブ")] public ListedCard listedCardPrefab;

    public DeckBoxData deckBoxData;

    public List<ListedCard> generatedListedCards = new List<ListedCard>();

    /// <summary>
    /// go:投入できるカードリストを並べる(既にあるリストを上書きする）
    /// </summary>
    /// <param name="enumerateCardDatas"></param>
    public void EnumerateDeckList(List<ICardData> enumerateCardDatas)
    {
        //List<ListedCard> generatedListedCards = EnumerateCards<ListedCard, PossessionCard>(listedCardPrefab, GameManager.instance.userData.possessionCards);
        generatedListedCards = GenerateAndEnumerateCards<ListedCard, ICardData>(listedCardPrefab, enumerateCardDatas);
        
        //ロードしたデッキ内に所持カードが含まれているか確認し、その分リストに表示されたカードの残量を減らす
        
        for (int loops = 0; loops < cardSlots.Count; loops++)
        {
            if (loops < enumerateCardDatas.Count)
            {
                int thisCardInDeckNum = 0; //並べるカードがデッキに存在する数
                ListedCard generatedListedCard = generatedListedCards[loops];
                foreach (Card_InDeckBox deckCard in deckBoxData.cards)
                {
                    CardData cardData = deckCard.cardData;
                    if (generatedListedCard.CardData == cardData)
                    {
                        thisCardInDeckNum++;
                        deckCard.sourceListedCard = generatedListedCard;
                    }
                }
                //↑のスペシャルデッキver.そもそもこの仕組み繰り返し多すぎてきもいのでエディタ改革で何とかする
                foreach (Card_InDeckBox deckCard in deckBoxData.specialCards)
                {
                    CardData cardData = deckCard.cardData;
                    if (generatedListedCard.CardData == cardData)
                    {
                        thisCardInDeckNum++;
                        deckCard.sourceListedCard = generatedListedCard;
                    }
                }

                if (GameManager.instance.userData.possessionCards.Count > loops)
                {
                    generatedListedCard.RemainingNum = GameManager.instance.userData.possessionCards[generatedListedCard.cardData.cardID].inPossessionNum - thisCardInDeckNum;
                }
                else
                {
                    generatedListedCard.RemainingNum = -thisCardInDeckNum;
                }
                //cardSlots[loops].AssignCardToSlot(generatedListedCard);いらんかも？
            }
        }
    }

    /// <summary>
    /// 同じカードデータのListedCardを取得
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public ListedCard SearchMatchCardData(CardData cardData)
    {
        foreach(ListedCard listedCard in generatedListedCards)
        {
            if(listedCard.cardData == cardData)
            {
                return listedCard;
            }
            else
            {
                Debug.Log(listedCard.cardData.name + "不一致");
            }
        }
        return null;
    }
}
