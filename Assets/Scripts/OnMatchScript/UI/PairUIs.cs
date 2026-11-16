using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PairUIs : MonoBehaviour
{
    [Header("2つの対応したプレイヤー1のHPバー")] public HPGuage[] player1HPGuage;
    [Header("2つの対応したプレイヤー2のHPバー")] public HPGuage[] player2HPGuage;

    public static PairUIs instance = null;

    private void Awake()
    {
        instance = this;
    }

    public void UpdatePlayerHP(Player player, int lifeDifference)
    {
        if (player == MatchManager.instance.players[0])
        {
            foreach (HPGuage hPGuage in player1HPGuage)
            {
                hPGuage.UpdateGuage(lifeDifference);
            }
        }
        else
        {
            foreach (HPGuage hPGuage in player2HPGuage)
            {
                hPGuage.UpdateGuage(lifeDifference);
            }
        }
    }
}
