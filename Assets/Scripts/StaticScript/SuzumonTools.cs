using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SuzumonTools
{
    /// <summary>
    /// 決められた選択肢の中からランダムに対象を1体返す
    /// </summary>
    /// <param name="targets">選択肢</param>
    /// <returns>ランダムで選ばれたカード</returns>
    public static Card RandomTargets(List<Card> targets)
    {
        int choiceNum = targets.Count;
        int random = MatchManager.instance.duelRandom.Range(0, choiceNum);
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Whole, "ランダムな値'" + random + "'を設定しました");
        return targets[random];
    }

    /// <summary>
    /// 1番の親にたどり着くまで親をたどってカードスクリプトを取得します。1番の親までたどっても取得できなかった場合、nullを返します。
    /// </summary>
    /// <param name="child">親を探している子</param>
    /// <returns></returns>
    public static Card GetParentCard(Transform child)
    {
        Transform parent = child;
        while (parent)
        {
            parent = parent.transform.parent;
            Card parentCard = null;
            if (parent)
            {
                parentCard = parent.gameObject.GetComponent<Card>();
            }
            else
            {
                return null;
            }

            if (parentCard)
            {
                return parentCard;
            }
        }
        return null;
    }

    /// <summary>
    /// 変数の値をAddまたはSetどちらの方式で変更する場合でもこのメソッドに値を入れて処理することができる
    /// </summary>
    /// <param name="variable">変更したい変数</param>
    /// <param name="value">変更する値</param>
    /// <param name="isSet">true = Set,false = Add</param>
    public static void SeparateSetOrAdd(ref int variable, int value, bool isSet)
    {
        if (isSet)
        {
            variable = value;
        }
        else
        {
            variable += value;
        }
    }
    
    public static List<Card> ToCards(this IEnumerable<Monster> monsters)
    {
        return monsters.Select(monster => monster.card).ToList();
    }

    /// <summary>
    /// IEnumerableの中から要素をランダムに1つ取り出す
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T RandomElement<T>(this IEnumerable<T> source)
    {
        var list = source as IList<T> ?? source.ToList();

        if (list.Count == 0)
            throw new InvalidOperationException();

        //今のところここはduelRandom使わなくていいけど今後どうなんだろう、通信でこの結果ごと同期されるから異なる値になることはないし、リプレイとしてもランダム効果ではなく単にこのランダム出力が「選択された」として保存されるからランダムシードが介入することもない。むしろ今のところ、ここでduelRandomを使ってしまうと受信側はduelRandomのシード内の順番が1回分進んでないのにこっちは進んじゃうからむしろだめだ
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static List<T> SortCard<T>(List<T> cards)
        where T : ICardData
    {
        List<T> sortedCards = new List<T>();

        List<T> remainCards = cards.ToList();

        //ノーマルモンスターをソート
        List<T> normalMonsters = new List<T>();
        for (int i = remainCards.Count - 1; i >= 0; i--)
        {
            T card = remainCards[i];
            if (card.GetCardData().GetDetailCardType() == TargetCardConditions.DetailCardType.NormalMonster)
            {
                normalMonsters.Add(card);
                remainCards.Remove(card);
            }
        }
        normalMonsters = normalMonsters.OrderBy(n => n.GetCardData().cardName).ToList();

        List<T> evoMonsters = new List<T>();
        for (int i = remainCards.Count - 1; i >= 0; i--)
        {
            T card = remainCards[i];
            if (card.GetCardData().GetDetailCardType() == TargetCardConditions.DetailCardType.EvoMonster)
            {
                evoMonsters.Add(card);
                remainCards.Remove(card);
            }
        }
        evoMonsters = evoMonsters.OrderBy(n => n.GetCardData().cardName).ToList();

        List<T> adventMonsters = new List<T>();
        for (int i = remainCards.Count - 1; i >= 0; i--)
        {
            T card = remainCards[i];
            if (card.GetCardData().GetDetailCardType() == TargetCardConditions.DetailCardType.AdventMonster)
            {
                adventMonsters.Add(card);
                remainCards.Remove(card);
            }
        }
        adventMonsters = adventMonsters.OrderBy(n => n.GetCardData().cardName).ToList();

        List<T> normalItems = new List<T>();
        for (int i = remainCards.Count - 1; i >= 0; i--)
        {
            T card = remainCards[i];
            if (card.GetCardData().GetDetailCardType() == TargetCardConditions.DetailCardType.NormalItem)
            {
                normalItems.Add(card);
                remainCards.Remove(card);
            }
        }
        normalItems = normalItems.OrderBy(n => n.GetCardData().cardName).ToList();

        List<T> weaponItems = new List<T>();
        for (int i = remainCards.Count - 1; i >= 0; i--)
        {
            T card = remainCards[i];
            if (card.GetCardData().GetDetailCardType() == TargetCardConditions.DetailCardType.Weapon)
            {
                weaponItems.Add(card);
                remainCards.Remove(card);
            }
        }
        weaponItems = weaponItems.OrderBy(n => n.GetCardData().cardName).ToList();

        List<T> eternalItems = new List<T>();
        for (int i = remainCards.Count - 1; i >= 0; i--)
        {
            T card = remainCards[i];
            if (card.GetCardData().GetDetailCardType() == TargetCardConditions.DetailCardType.Eternal)
            {
                eternalItems.Add(card);
                remainCards.Remove(card);
            }
        }
        eternalItems = eternalItems.OrderBy(n => n.GetCardData().cardName).ToList();

        sortedCards.AddRange(normalMonsters);
        sortedCards.AddRange(evoMonsters);
        sortedCards.AddRange(adventMonsters);
        sortedCards.AddRange(normalItems);
        sortedCards.AddRange(weaponItems);
        sortedCards.AddRange(eternalItems);

        return sortedCards;
    }
}
