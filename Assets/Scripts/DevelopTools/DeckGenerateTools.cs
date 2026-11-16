using System.Collections.Generic;
using UnityEngine;

public class DeckGenerateTools : MonoBehaviour
{
    [Header("ここに変換したいCardDataをアタッチ")] public List<CardData> cardDatas = new List<CardData>();
    [Header("ここに変換したいデッキIDをアタッチ")] public string deckId;
    [Header("デッキ生成機")] public DeckGeneratorFromDatas deckGenerator;


    [ContextMenu("GenerateDeckBoxDataFromCardDatas")]
    public void OnClickedGenerateDeckBoxDataFromCardDatas()
    {
        DeckBoxData generatedDeckBoxData = deckGenerator.ConvertCardDatasToDeckBoxData(cardDatas);
        generatedDeckBoxData.transform.SetParent(transform);
    }

    [ContextMenu("GenerateDeckBoxDataFromDeckId")]
    public void OnClickedConvertDeckIdToDeckBoxData()
    {
        DeckBoxData generatedDeckBoxData = deckGenerator.CopyTextValueDeck(deckId);
        generatedDeckBoxData.transform.SetParent(transform);
    }

}
