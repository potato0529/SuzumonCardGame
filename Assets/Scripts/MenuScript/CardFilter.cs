using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardFilter : MonoBehaviour
{
    [Header("カードタイプフィルタ")] public CardTypeFilter cardTypeFilter;
    [Header("属性フィルタ")] public AttributeFilter attributeFilter;
    [Header("ランクフィルタ")] public RankFilter rankFilter;
    [Header("使用感フィルター")] public CardUsabilityFilter cardUsabilityFilter;

    public TMP_InputField inputField;
    [Header("フィルターウィンドウの背景")] public GameObject background;
    [Header("フィルターウィンドウ展開ボタン")] public Button filterWindowButton;

    private Dictionary<FilterSection, List<int>> appliedFilterStates = new Dictionary<FilterSection, List<int>>();

    private bool isFilterApplied = false;//フィルター条件がなにかしら適用されているか（適用ボタンを押されたかではなく、前回適用されたときになにかのトグルがオンになっていたかどうか）

    private void Start()
    {
        appliedFilterStates.Add(cardTypeFilter, cardTypeFilter.GetOnToggleIndexs());
        appliedFilterStates.Add(attributeFilter, attributeFilter.GetOnToggleIndexs());
        appliedFilterStates.Add(rankFilter, rankFilter.GetOnToggleIndexs());
        appliedFilterStates.Add(cardUsabilityFilter, cardUsabilityFilter.GetOnToggleIndexs());

    }

    /// <summary>
    /// テキストフィルターが更新されたときに呼ばれる ref+1
    /// </summary>
    /// <param name="text"></param>
    public void OnEditedTextFilterBox(string text)
    {
        UpdatedFilterConditions();
    }

    /// <summary>
    /// フィルターウィンドウの絞り込むボタンがクリックされ、属性などの条件により絞り込む ref+1
    /// </summary>
    public void DecideFilterConditions()
    {
        appliedFilterStates.Clear();

        appliedFilterStates.Add(cardTypeFilter,cardTypeFilter.GetOnToggleIndexs());
        appliedFilterStates.Add(attributeFilter,attributeFilter.GetOnToggleIndexs());
        appliedFilterStates.Add(rankFilter,rankFilter.GetOnToggleIndexs());
        appliedFilterStates.Add(cardUsabilityFilter,cardUsabilityFilter.GetOnToggleIndexs());

        //少し繰り返しが多くてゴリ押しだからもしかしたらFilterSectionをそれぞれ配列とかで管理したほうがいいのかね
        if(appliedFilterStates[cardTypeFilter].Count == 0 && appliedFilterStates[attributeFilter].Count == 0 && appliedFilterStates[rankFilter].Count == 0 && appliedFilterStates[cardUsabilityFilter].Count == 0)
        {
            //全てのフィルターがオフの場合は、フィルターを適用しない
            isFilterApplied = false;
            filterWindowButton.gameObject.GetComponent<Image>().color = Color.white;
        }
        else
        {
            isFilterApplied = true;
            filterWindowButton.gameObject.GetComponent<Image>().color = Color.yellow;
        }

        UpdatedFilterConditions();
    }

    /// <summary>
    /// フィルターの条件が更新されたとき（フィルターウィンドウで「絞り込む」を押された時かテキストフィルターが更新された時に呼ばれる、フィルターをし直してリストに反映されるまでの一連の処理を呼び出す
    /// </summary>
    private void UpdatedFilterConditions()
    {
        List<CardData> convertedPossessionCards = GameManager.instance.userData.possessionCardDatas;
        convertedPossessionCards = GetAllFilteredCards(convertedPossessionCards);
        EnumerateFilteredCards(convertedPossessionCards);
    }

    /// <summary>
    /// 渡されたカードを全ての種類のフィルターを通して返す
    /// </summary>
    public List<CardData> GetAllFilteredCards(List<CardData> cardDatas)
    {
        List<CardData> filteredCards = cardDatas;
        filteredCards = GetFilteredCards(filteredCards);
        filteredCards = TextSearchFilter(filteredCards);
        return filteredCards;
    }

    /// <summary>
    /// テキストフィルターをもとに絞り込む
    /// </summary>
    /// <param name="cardDatas"></param>
    /// <returns></returns>
    public List<CardData> TextSearchFilter(List<CardData> cardDatas)//この引数は使えないからinputFieldを使おう
    {
        List<CardData> filteredCards = new List<CardData>(cardDatas);

        filteredCards = filteredCards.FindAll(cardData => cardData.cardName.Contains(inputField.text) || cardData.effectText.Contains(inputField.text));

        return filteredCards;
    }

    /// <summary>
    /// トグルフィルターを元にカードを絞り込む
    /// </summary>
    /// <param name="cardDatas"></param>
    /// <returns></returns>
    public List<CardData> GetFilteredCards(List<CardData> cardDatas)
    {
        List<CardData> filteredCards = cardDatas;

        filteredCards = cardTypeFilter.FilterCards(filteredCards,appliedFilterStates[cardTypeFilter]);

        filteredCards = attributeFilter.FilterCards(filteredCards,appliedFilterStates[attributeFilter]);

        filteredCards = rankFilter.FilterCards(filteredCards,appliedFilterStates[rankFilter]);

        filteredCards = cardUsabilityFilter.FilterCards(filteredCards, appliedFilterStates[cardUsabilityFilter]);

        return filteredCards;
    }

    /// <summary>
    /// 絞り込んだカードをデッキリストに表示するための一連の処理
    /// </summary>
    /// <param name="cardDatas"></param>
    private void EnumerateFilteredCards(List<CardData> filteredCards)
    {
        DeckEditorManager.instance.slotsGenerator.EnumerateDeckList(filteredCards.ConvertAll(x => (ICardData)x));
        DeckEditorManager.instance.cardListArea.SetOriginalPos();

    }

    /// <summary>
    /// フィルターウィンドウを開くボタンがクリックされる ref+1
    /// </summary>
    public void OnClickedFilterWindowButton()
    {
        background.SetActive(true);        
    }

    /// <summary>
    /// フィルターウィンドウの閉じるボタンがクリックされる ref+1
    /// </summary>
    public void OnClickedCloseButton()
    {
        //フィルターウィンドウを閉じるときに、前回適用した条件のトグル状態にする(適用していないけどとグルだけ変更されていた場合紛らわしいので戻す)
        //if (appliedFilterStates.ContainsKey(cardTypeFilter))
            cardTypeFilter.SetTogglesFromIndexs(appliedFilterStates[cardTypeFilter]);

        //if(appliedFilterStates.ContainsKey(attributeFilter))
            attributeFilter.SetTogglesFromIndexs(appliedFilterStates[attributeFilter]);

        //if(appliedFilterStates.ContainsKey(rankFilter))
            rankFilter.SetTogglesFromIndexs(appliedFilterStates[rankFilter]);

        //if(appliedFilterStates.ContainsKey(cardUsabilityFilter))
            cardUsabilityFilter.SetTogglesFromIndexs(appliedFilterStates[cardUsabilityFilter]);

        background.SetActive(false);
    }

    /// <summary>
    /// フィルターウィンドウのリセットボタンがクリックされる ref+1
    /// </summary>
    public void OnClickedResetButton()
    {
        cardTypeFilter.SetTogglesFromIndexs(new List<int>());
        attributeFilter.SetTogglesFromIndexs(new List<int>());
        rankFilter.SetTogglesFromIndexs(new List<int>());
        cardUsabilityFilter.SetTogglesFromIndexs(new List<int>());
    }
}
