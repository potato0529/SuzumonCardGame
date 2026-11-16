using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class IndexSelector
{
    [Header("インデックス範囲指定")] public IndexRange[] indexRanges;

    /// <summary>
    /// 設定されているIndexRangesのデータをもとに渡されたリストの当てはまるインデックス要素を返す。※indexRangesの要素が0の場合、渡されたリストをそのまま返す。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="card">ValueGeneratorを判断する基点のカード</param>
    /// <returns></returns>
    public List<T> GetIndexObjects<T>(List<T> list,Card card)
    {
        List<T> results = new List<T>();

        //設定（要素）がなければ渡されたリストをそのまま返す。
        if (indexRanges.Length == 0)
        {
            return list;
        }

        foreach (IndexRange indexRange in indexRanges)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (i >= indexRange.min.GetValue(card) && i<= indexRange.max.GetValue(card))
                {
                    results.Add(list[i]);
                }
            }
        }
        return results;
    }
}

[Serializable]
public class IndexRange
{
    public VariableInt min;
    public VariableInt max;
}
