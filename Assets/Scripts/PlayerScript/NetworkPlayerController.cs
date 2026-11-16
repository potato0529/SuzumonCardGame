using UnityEngine;

public class NetworkPlayerController : IPlayerController
{
    public void OnCardSelectionStarted()
    {
        //2026/06/28時点では、カード選択処理開始時にPlayerの操作者が通信相手だった時にすべき処理はなく、相手の選択コマンド受信を待つだけなので記述なし
    }

    public void OnMyTurnStarted()
    {
        //2026/06/28時点では、自分ターン開始時にPlayerの操作者が通信相手だった時にすべき処理はなく、相手の選択コマンド受信を待つだけなので記述なし
    }
}
