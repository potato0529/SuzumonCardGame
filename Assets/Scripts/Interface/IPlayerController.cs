using UnityEngine;

/// <summary>
/// プレイヤーに関する処理の中でも、Playerクラスから分離し、Playerの操作主体が何かによって処理が分岐するような、状態としてのPlayerというより操作主体のPlayerとして行動が求められる処理を持つクラス、簡潔に言うとプレイヤーの操作主体クラス
/// </summary>
public interface IPlayerController
{
    //memo:一度このPlayerController系列のクラスを追加してしまったが、結構処理の構造を変えるのに時間がかかりそうなので一旦これらはまだ実際の処理の導入せず、置いておく。

    public void OnCardSelectionStarted();

    public void OnMyTurnStarted();
}
