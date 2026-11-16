using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EitherAntiqueBurstList : MonoBehaviour
{
    public CardSlotsGenerator cardSlotsGenerator;
    public InMatchListedUICard listedUICardPrefab;
    private List<AntiqueListedUICard> antiqueListedUICards = new List<AntiqueListedUICard>();
    [Header("アンティークバースト確認ボタン")] public Image antiqueButton;

    /// <summary>
    /// アンティークバーストが開始し、リストに追加される
    /// </summary>
    /// <param name="antiqueBurst"></param>
    public void StartAntiqueBurst(AntiqueBurst antiqueBurst)
    {
        //進行中のアンティークバーストがない状態でアンティークバーストが開始したとき、ボタンを明るくする
        if (antiqueListedUICards.Count == 0)
            antiqueButton.color = new Color(1, 1, 1, 1);

        Card sourceCard = antiqueBurst.invokeEffectNugget.card;
        InMatchListedUICard inMatchListedUICardCanvas = cardSlotsGenerator.GenerateListedCard(listedUICardPrefab, sourceCard);

        inMatchListedUICardCanvas.sourceCard = sourceCard;

        cardSlotsGenerator.ArrangeCards();

        AntiqueListedUICard generatedAntiqueListedUICard = inMatchListedUICardCanvas.GetComponent<AntiqueListedUICard>();
        antiqueListedUICards.Add(generatedAntiqueListedUICard);
        generatedAntiqueListedUICard.antiqueCountText.text = antiqueBurst.antiqueCount.ToString();
    }

    /// <summary>
    /// アンティークバーストが発動してアンティークリストから取り除かれる
    /// </summary>
    /// <param name="antiqueBurst"></param>
    public void InvokedAntiqueBurst(AntiqueBurst antiqueBurst,int indexNum)
    {
        RemoveAntiqueList(antiqueBurst,indexNum);
    }

    /// <summary>
    /// アンティークバーストが中断してアンティークリストから取り除かれる
    /// </summary>
    public void CanceledAntiqueBurst(AntiqueBurst antiqueBurst, int indexNum)
    {
        RemoveAntiqueList(antiqueBurst,indexNum);
    }

    public void RemoveAntiqueList(AntiqueBurst antiqueBurst,int indexNum)
    {
        cardSlotsGenerator.RemoveListedCard(indexNum);
        cardSlotsGenerator.ArrangeCards();
        antiqueListedUICards.RemoveAt(indexNum);

        //進行中のアンティークバーストがなくなったとき、ボタンを暗くする
        if (antiqueListedUICards.Count == 0)
            antiqueButton.color = new Color(1, 0.7f, 1, 0.4f);
    }

    public void GoOnAntique(AntiqueBurst antiqueBurst,int indexNum)
    {
        antiqueListedUICards[indexNum].antiqueCountText.text = antiqueBurst.antiqueCount.ToString();
    }
}
