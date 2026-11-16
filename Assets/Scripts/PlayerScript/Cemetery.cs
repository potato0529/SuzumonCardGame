using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cemetery : MonoBehaviour
{
    public Player player;
    private List<Card> cards = new List<Card>();
    [Header("自分の墓地スロットのジェネレーター")] public CardSlotsGenerator myCardSlotsGenerator;
    [Header("相手の墓地スロットのジェネレーター")] public CardSlotsGenerator opponentCardSlotGenerator;
    [Header("自分の墓地を表示するキャンバス")] public Canvas myCemeteryCanvas;
    [Header("相手の墓地を表示するキャンバス")] public Canvas opponentCemeteryCanvas;
    [Header("墓地リストに表示するカードプレハブ"),SerializeReference] public InMatchListedUICard inMatchListedUICardPrefab;
    [HideInInspector] public int cardsNum;

    private void Start()
    {
        //Awakeを起こすため
        myCemeteryCanvas.gameObject.SetActive(true);
        myCemeteryCanvas.gameObject.SetActive(false);
        opponentCemeteryCanvas.gameObject.SetActive(true);
        opponentCemeteryCanvas.gameObject.SetActive(false);
    }

    public void LeaveCemetery(Card card)
    {
        int thisIndex = cards.IndexOf(card);
        cards.Remove(card);
        myCardSlotsGenerator.RemoveListedCard(thisIndex);
        myCardSlotsGenerator.ArrangeCards();
        opponentCardSlotGenerator.RemoveListedCard(thisIndex);
        opponentCardSlotGenerator.ArrangeCards();
    }

    public List<Card> GetCemeteryCards()
    {
        return cards;
    }

    public void SendCemetery(Card card)
    {
        card.moveToPos.Move(transform.position, 15,ArrangementCemetery); //今までずっと音だったけどコレのせいで墓地の位置若干ずれてる
        cards.Add(card);
        UtilityLog.CardLog(card.gameObject.name + "が墓地に送られました", card, UtilityLog.LogCategory.ChangeCardState);
        //ArrangementCemetery();

        InMatchListedUICard inMatchListedUICardInMyCanvas = myCardSlotsGenerator.GenerateListedCard(inMatchListedUICardPrefab, card);
        inMatchListedUICardInMyCanvas.sourceCard = card;

        InMatchListedUICard inMatchListedUICardInOpponentCanvas = opponentCardSlotGenerator.GenerateListedCard(inMatchListedUICardPrefab, card);
        inMatchListedUICardInOpponentCanvas.sourceCard = card;

        myCardSlotsGenerator.ArrangeCards();
        opponentCardSlotGenerator.ArrangeCards();
    }

    public void ArrangementCemetery()
    {
        float y = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            y += cards[i].body.transform.lossyScale.y;
            cards[i].transform.position = transform.position + new Vector3(0, y, 0);
        }
    }

    public void OpenCemeteryWindow()
    {
        myCemeteryCanvas.gameObject.SetActive(true);
    }

    public void OpenOpponentCemeteryWindow()
    {
        opponentCemeteryCanvas.gameObject.SetActive(true);
    }
}
