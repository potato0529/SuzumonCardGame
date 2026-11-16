using UnityEngine;

/// <summary>
/// 試合開始前のシードなどのカスタム設定データのまとまり MatchSettingsだと試合中の設定と重複してしまうのでMatchMenuSettingsにしたが、良い名前なのか
/// </summary>
public class MatchMenuSettings
{
    public MatchMenuSettings(int seed)
    {
        Seed = seed;
    }

    public int Seed
    {
        get; set;
    }
}
