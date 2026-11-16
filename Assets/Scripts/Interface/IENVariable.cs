/// <summary>
/// 一連の効果処理を実行することができるスクリプトが持つ。1つの効果やコストが完了した合図を受け取ったり、効果を使った回数を記録したりするのに必要。
/// </summary>
public interface IENVariable
{
    void PaidCost();
    void CompUsedEffect();
}
