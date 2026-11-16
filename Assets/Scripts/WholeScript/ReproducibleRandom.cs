using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproducibleRandom
{
    //このクラスが管理する指定シードに基づいた乱数情報
    private Random.State seedState;

    /// <summary>
    /// 指定されたシード値で初期化するコンストラクタ
    /// </summary>
    /// <param name="seed"></param>
    public ReproducibleRandom(int seed)
    {
        //現在の乱数の内部状態を一時退避させる
        var prevState = Random.state;
        
        //新しく指定されたシードの乱数情報をセットする
        Random.InitState(seed);

        //今後Range()が呼ばれるごとに扱う指定シード乱数情報を保存する
        seedState = Random.state;
        //一時退避されたもともとの乱数情報に戻す
        Random.state = prevState;
    }

    /// <summary>
    /// 指定されたint型の範囲の乱数を取得
    /// </summary>
    /// <param name="minInclusive">下限（この値も範囲に含まれる）</param>
    /// <param name="maxExclusive">上限（この値は範囲に含まれない）</param>
    /// <returns></returns>
    public int Range(int minInclusive, int maxExclusive)
    {
        //現在の乱数の内部状態を一時退避させる
        var prevState = Random.state;

        //保存していたシード乱数情報をセットする
        Random.state = seedState;

        //シード乱数を使って乱数を生成する
        var result = Random.Range(minInclusive, maxExclusive);

        //保存情報を乱数が生成されて1つ乱数列が進んだシード乱数に更新する
        seedState = Random.state;

        //一時退避されたもともとの乱数情報に戻す
        Random.state = prevState;

        return result;
    }
}
