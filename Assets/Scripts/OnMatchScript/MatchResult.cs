using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchResult : MonoBehaviour
{
    [Header("プレイヤー")] public Player player;
    [Header("召喚回数")] public TMP_Text summonedCount;
    [Header("アイテム使用回数")] public TMP_Text usedItemCount;
    [Header("効果発動回数")] public TMP_Text effectInvokeCount;
    [Header("アタック回数")] public TMP_Text attackedCount;
    [Header("ディフェンス回数")] public TMP_Text defencedCount;
    [Header("破壊された回数")] public TMP_Text destroyedCount;
    [Header("墓地に送られた回数")] public TMP_Text sentCemeteryCount;

    private void Start()
    {
        PlayerMatchLog matchLog = player.playerMatchLog;
        summonedCount.text = "召喚回数:" + matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.SummonedCount);
        usedItemCount.text = "アイテム使用回数:" + matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.UsedItemCount);
        effectInvokeCount.text = "効果発動回数:" + matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.EffectInvoked);
        attackedCount.text = "アタック回数:" + matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.Attacked);
        defencedCount.text = "ディフェンス回数:" + matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.Defenced);
        destroyedCount.text = "破壊された回数:" + matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.Destroyed);
        sentCemeteryCount.text = "墓地に送られた回数:" + matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.SentCemetery);

        UserData userData = GameManager.instance.userData;
        int getExp = 100;
        getExp += matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.SummonedCount) * 10;
        getExp += matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.UsedItemCount) * 10;
        getExp += ((matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.EffectInvoked) - 10) / 10) * 5;
        getExp += matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.Attacked) * 3;
        getExp += matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.Defenced) * 3;
        getExp += ((matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.Destroyed) + matchLog.GetIntMatchData(PlayerMatchLog.IntMatchData.SentCemetery) - 15) / 10) * 5;
        userData.GetExp(getExp);
    }

    /// <summary>
    /// マッチリザルトを前面に出す
    /// </summary>
    public void ViewMatchResult()
    {
        transform.SetSiblingIndex(1);
    }
}
