using UnityEngine;

public class LocalPlayerController
{
    public void OnCardSelectionStarted()
    {
        //2026/06/28時点では、カード選択処理開始時にPlayerの操作者がローカルプレイヤーだった時にすべき処理はなく、CardPointerで選択カードが受信されるのを待つだけなので記述なし
    }

    public void OnMyTurnStarted()
    {
        //2026/06/28時点では、自分ターン開始時にPlayerの操作者がローカルプレイヤーだった時に自動ですべき操作はなく、自分の操作を待つだけなので記述なし
    }
}
