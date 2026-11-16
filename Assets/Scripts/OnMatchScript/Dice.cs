using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    private bool hasEndedAnim;
    public IEnumerator RollDice(Action<int> diceValueCallback)
    {
        //af:シード値をリセットする機構をつくっておく。
        //1~6までの乱数(7未満であるため)
        int diceValue = MatchManager.instance.duelRandom.Range(1, 7);
        StartCoroutine(MatchManager.instance.localPlayer.playerCanvas.DiceAnim(diceValue,EndDiceAnim));
        StartCoroutine(MatchManager.instance.localPlayer.opponentPlayer.playerCanvas.DiceAnim(diceValue,null));
        hasEndedAnim = false;
        while (!hasEndedAnim)
        {
            yield return null;
        }
        diceValueCallback(diceValue);
        Debug.Log("サイコロの処理が終了しました");

    }

    /// <summary>
    /// プレイヤーの画面のアニメーションが終わったら通知する（片方のアニメーションが終われば処理を進めてよいということにする）
    /// </summary>
    public void EndDiceAnim()
    {
        hasEndedAnim = true;
    }
}
