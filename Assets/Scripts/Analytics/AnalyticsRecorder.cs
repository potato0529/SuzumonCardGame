using UnityEngine;
using Unity.Services.Analytics;

/// <summary>
/// ゲーム内で発生したイベントをUnity Analyticsに記録する。基本的に、ゲームのメインシステム内で直接記録処理は行わずにここに記録処理を集約させる。
/// </summary>
public static class AnalyticsRecorder
{
    /// <summary>
    /// カードが使用されたことを記録する。使用されたカードのコントローラーがCPUだった場合無視する。
    /// </summary>
    /// <param name="cardId"></param>
    public static void RecordCardPlayed(CardData cardData,Card card)
    {
        //元々のコントローラーが相手のカードを使用した場合の処理はどうするか。使用された時点でコントローラーは変わっている扱いになるならよいが、そうでないならカードが使用されたときにその操作の実行者も情報として運ぶ必要が出てくる
        if (card.controlerPlayer.isCpu)
            return;

        //UtilityLog.DebugLog($"{cardData.cardName} カード使用ログ");
        int cardId = GameManager.instance.cardDataBase.GetCardId(cardData);
        //UtilityLog.DebugLog($"{cardId}");

        var cardPlayedEvent = new CardPlayedEvent
        {
            CardId = cardId
        };

        AnalyticsService.Instance.RecordEvent(cardPlayedEvent);
    }
}
