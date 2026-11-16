using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSlotsGenerator : MonoBehaviour
{
    [Header("表示するキャンバス")] public Canvas canvas;
    [Header("スロットプレハブ")] public GameObject slotPrefab;
    [Header("横列のカード表示限界数")] public int slotsWidth;
    [Header("生成するスロット数")] public int generateSlotsNum;
    [Header("スロットの間隔（横）")] public float slotColumnDistance;
    [Header("スロットの間隔（縦）")] public float slotRowDistance;
    [Header("生成されたスロット")] public List<GameObject> cardSlots = new List<GameObject>();
    [Header("生成されたスロット要素カード")] public List<ListedUICard> generatedListedUICards = new List<ListedUICard>();

    #region Memo
    //このクラスのすべてのn番目系の値で、現実においての1番目は0なので、こんがらがらないように認識を統一すべし
    #endregion

    private void Awake()
    {
        GenerateSlots();
    }

    /// <summary>
    /// カードリストのスロットを生成する
    /// </summary>
    public void GenerateSlots()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, "スロットを生成します");

        //次にスロットを生成する座標
        Vector2 slotPos = Vector2.zero;

        //何列目？
        int columnsNum = 0;
        //何行目？
        int rowsNum = 0;
        for(int loops = 0; loops < generateSlotsNum; loops++)
        {
            //次のスロットの設定
            columnsNum = loops % slotsWidth;
            rowsNum = loops / slotsWidth;
            
            slotPos = new Vector2(slotColumnDistance * columnsNum, slotRowDistance * -rowsNum);
            //Debug.Log($"スロット生成 座標:{slotPos} 桁/行 {columnsNum}/{rowsNum}");

            //スロットを生成し、設定を反映させる
            GameObject generatedSlot = Instantiate(slotPrefab);
            generatedSlot.transform.SetParent(canvas.transform,false);
            generatedSlot.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition + slotPos;
            generatedSlot.transform.SetParent(this.transform);
            //ジェネレーターの座標に対して加算する
            
            cardSlots.Add(generatedSlot);
        }
    }

    /// <summary>
    /// スロットをすべて削除する
    /// </summary>
    public void RemoveSlots()
    {
        cardSlots.Clear();
    }

    /// <summary>
    /// go:もともとスロットに要素があればすべて上書きしたうえで要素を並べる
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Source"></typeparam>
    /// <param name="inSlotElementPrefab"></param>
    /// <param name="sourceCards"></param>
    /// <returns></returns>
    public List<T> GenerateAndEnumerateCards<T, Source>(T inSlotElementPrefab, List<Source> sourceCards)
        where T : ListedUICard
        where Source : ICardData
    {
        //おそらく完全に上書きするイメージだから既にあるものはいったん破棄するのかな
        foreach(ListedUICard listedUICard in generatedListedUICards)
        {
            Destroy(listedUICard.gameObject);
        }

        generatedListedUICards = new List<ListedUICard>();
        List<T> generatedListCards = new List<T>();
        
        for (int loops = 0; loops < sourceCards.Count; loops++)
        {
            T generatedListedUICard = GenerateListedCard<T,Source>(inSlotElementPrefab,sourceCards[loops]);
            generatedListCards.Add(generatedListedUICard);
        }
        ArrangeCards();
        return generatedListCards;
    }

    /// <summary>
    /// go:与えられたカードデータをもつクラスをリストに並べられるように生成し、初期化する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Source"></typeparam>
    /// <param name="inSlotElementPrefab"></param>
    /// <param name="sourceCard"></param>
    /// <returns></returns>
    public T GenerateListedCard<T, Source>(T inSlotElementPrefab, Source sourceCard)
        where T : ListedUICard
        where Source : ICardData
    {
        T generatedListedCard = Instantiate(inSlotElementPrefab);
        generatedListedUICards.Add(generatedListedCard);
        generatedListedCard.transform.SetParent(canvas.transform, false);
        generatedListedCard.transform.SetParent(transform);
        generatedListedCard.name = sourceCard.GetCardData().cardName;

        generatedListedCard.SetCardData(sourceCard.GetCardData());

        generatedListedCard.GetComponent<Image>().sprite = sourceCard.GetCardData().sprite;
        return generatedListedCard;
    }

    public void RemoveListedCard(int index)
    {
        ListedUICard listedUICard = generatedListedUICards[index];
        generatedListedUICards.RemoveAt(index);
        Destroy(listedUICard.gameObject);
    }

    public void ArrangeCards()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, "リストのカードを並べかえます");

        for (int loops = 0; loops < cardSlots.Count; loops++)
        {
            if (loops < generatedListedUICards.Count)
            {
                ListedUICard listedUICard = generatedListedUICards[loops];
                listedUICard.GetComponent<RectTransform>().position = cardSlots[loops].GetComponent<RectTransform>().position;
            }
        }
    }

    public void SortArrange()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, "リストのカードを並べかえます");

        List<ListedUICard> sortedCards = SuzumonTools.SortCard<ListedUICard>(generatedListedUICards);
        for (int loops = 0; loops < cardSlots.Count; loops++)
        {
            if (loops < sortedCards.Count)
            {
                ListedUICard listedUICard = sortedCards[loops];
                listedUICard.GetComponent<RectTransform>().position = cardSlots[loops].GetComponent<RectTransform>().position;
            }
        }
    }
}
