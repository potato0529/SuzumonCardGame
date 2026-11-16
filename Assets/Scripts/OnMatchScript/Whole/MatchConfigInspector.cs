using UnityEngine;

/// <summary>
/// マッチに関連する設定を管理するためのインスペクターを持つ。このクラスは実際にゲーム内で参照される内部設定を保持しているわけではなく、ゲーム開始時に実際の内部設定クラスに設定を反映させる
/// </summary>
public class MatchConfigInspector : MonoBehaviour
{
    [Header("カードの移動アニメーションFPS")] public float cardMoveAnimFps;
    [Header("効果発動アニメーションFPS")] public float effectAnimFps;
    [Header("アタックアニメーションFPS")] public float attackAnimFps;
    [Header("カードハイライトのアニメーション周期")] public float cardHighlightAnimCycleTime;
    [Header("カードハイライトのアニメーション最大拡大率")] public float cardHighlightAnimSinScale;
    [Header("カードハイライトのアニメーション最小拡大率")] public float cardHighlightAnimMinScale;

}
