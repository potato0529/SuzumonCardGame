using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("バリアレンダラー")] public MeshRenderer meshRenderer;
    [Header("バリアが有効")] public bool enableBarrier;
    [Header("バリアがある"),SerializeField] private bool remainBarrier;

    /// <summary>
    /// フィールドremainBarrierの値をセットする
    /// </summary>
    /// <param name="isRemain"></param>
    public void SetRemainBarrier(bool isRemain)
    {
        remainBarrier = isRemain;
        meshRenderer.enabled = isRemain;
    }

    /// <summary>
    /// フィールドremainBarrierの値を取得する
    /// </summary>
    /// <returns></returns>
    public bool GetRemainBarrier()
    {
        return remainBarrier;
    }

    /// <summary>
    /// アタックすることによりバリアが消滅する
    /// </summary>
    public void AttackAndDisableBarrier()
    {
        DisableBarrier();
    }

    /// <summary>
    /// 効果などによってバリアが破壊される
    /// </summary>
    public void BreakBarrier()
    {
        DisableBarrier();
    }

    /// <summary>
    /// 何らかの要因によってバリアが無くなる
    /// </summary>
    public void DisableBarrier()
    {
        SetRemainBarrier(false);
    }

    /// <summary>
    /// バリアを展開する
    /// </summary>
    public void ExpandBarrier()
    {
        SetRemainBarrier(true);
    }

    /// <summary>
    /// 効果などによってバリアが復活する
    /// </summary>
    public void ReviveBarrier()
    {
        ExpandBarrier();
    }

}
