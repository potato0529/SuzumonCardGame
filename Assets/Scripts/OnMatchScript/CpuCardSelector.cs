using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CPUStrategy側からなにかしら情報を送り、それをもとに判断タイプも加味してカードを返す
/// </summary>
public class CpuCardSelector : MonoBehaviour
{
    /// <summary>
    /// 渡されたカードのうちからランダムにカードを1枚選ぶ
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public Card SelectRandomCard(List<Card> cards)
    {
        return cards[MatchManager.instance.cpuRandom.Range(0, cards.Count)];
    }

    /// <summary>
    /// 渡されたカードの中からつよいカードを優先的に選ぶ
    /// </summary>
    /// <returns></returns>
    public Card SelectStrongestCard()
    {
        return new Card();
    }

    /// <summary>
    /// 効果の内容を考慮して、渡されたカードのうちから選ぶカードを決定する
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="effectInvokerCard"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public Card SelectCardConsideringEffect(List<Card> cards,Card effectInvokerCard ,Effect effect)
    {
        return new Card();
    }
}
