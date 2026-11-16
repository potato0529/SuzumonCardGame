using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// トグルのインデックス番号とenumが対応しているフィルターセクションである場合にIsMatch(Card,int)をオーバーライドして参照ステータスの条件を設定すれば使える、カードのフィルターの種類のまとまり。
/// </summary>
public class FilterSection : MonoBehaviour
{
    [Header("トグルリスト(対応enumの順番準拠）")] public FilterToggle[] filterToggles = new FilterToggle[0];

    private void Awake()
    {
        //トグルにインデックス番号を記憶させる
        for (int filterIndex = 1; filterIndex < filterToggles.Length; filterIndex++)
        {
            FilterToggle filterToggle = filterToggles[filterIndex];

            if(filterToggle)
                filterToggle.indexNum = filterIndex;
        }
    }

    /// <summary>
    /// 与えられたカードを属性トグルに合わせてフィルターする
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public List<CardData> FilterCards(List<CardData> cardDatas,List<int> onToggleIndexs)
    {
        //af:カテゴリのような複数項目を持ちうるものになった場合重複してしまうので先にすべてを代入してけずっていく形式にする必要がある

        List<CardData> remainCards = new List<CardData>();

        if (onToggleIndexs.Count == 0)
            return cardDatas;

        foreach (CardData cardData in cardDatas)
        {
            foreach(int onToggleIndex in onToggleIndexs)
            {
                if (IsMatch(cardData,onToggleIndex))
                {
                    remainCards.Add(cardData);
                }
            }
        }
        return remainCards;
    }

    /// <summary>
    /// 現在のトグル状態に基づいてカードをフィルターする
    /// </summary>
    /// <param name="cardDatas"></param>
    /// <returns></returns>
    public List<CardData> FilterCardsCurrentState(List<CardData> cardDatas)
    {
        return FilterCards(cardDatas,GetOnToggleIndexs());
    }

    /// <summary>
    /// 引数とenumが対応しているとき、渡したカードの特定のステータスがそのenumの番号の項目と一致しているか確かめる。filterTogglesのindexとenumが対応するようになっていれば、これをオーバーライドしてcardの特定のステータスに一致するかどうかをreturnすればよい。
    /// </summary>
    /// <param name="cardData"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual bool IsMatch(CardData cardData,int index)
    {
        return true;
    }

    /// <summary>
    /// オンになっているトグルのインデックスの番号をすべて取得する
    /// </summary>
    /// <returns></returns>
    public List<int> GetOnToggleIndexs()
    {
        /*オンになっているトグルのクラスそのものを返してた時のやつ
        List<FilterToggle> onToggles = new List<FilterToggle>();

        for (int toggleIndex = 1; toggleIndex < filterToggles.Length; toggleIndex++)
        {
            FilterToggle filterToggle = filterToggles[toggleIndex];
            if (filterToggle && filterToggle.toggle.isOn)
            {
                onToggles.Add(filterToggle);
            }
        }

        return onToggles;
        */

        List<int> onToggleIndexs = new List<int>();

        for (int toggleIndex = 1; toggleIndex < filterToggles.Length; toggleIndex++)
        {
            FilterToggle filterToggle = filterToggles[toggleIndex];
            if (filterToggle && filterToggle.toggle.isOn)
            {
                onToggleIndexs.Add(toggleIndex);
            }
        }

        return onToggleIndexs;
    }

    /// <summary>
    /// 与えられたインデックスの配列のトグルのみをオンにする（それ以外はオフにする）
    /// </summary>
    /// <param name="indexs"></param>
    public void SetTogglesFromIndexs(List<int> indexs)
    {
        for (int toggleIndex = 1; toggleIndex < filterToggles.Length; toggleIndex++)
        {
            FilterToggle filterToggle = filterToggles[toggleIndex];
            if (filterToggle)
            {
                filterToggle.SetToggle(indexs.Contains(toggleIndex));
            }
        }
    }
}
