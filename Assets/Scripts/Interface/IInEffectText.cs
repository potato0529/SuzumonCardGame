using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 効果ナゲットの最小単位。カードが持つ効果の開始地点として最低限の実装を求める。
/// </summary>
public interface IInEffectText
{
    public Card card { set; get; }
    //テキスト生成用のメソッドや効果無効時の処理など実装予定
}
