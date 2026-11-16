using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectButton : CardButton
{
    /// <summary>
    /// クリックされたとき
    /// </summary>
    public override void Clicked()
    {
        UtilityLog.CardLog("EffectButtonがクリックされました", card, UtilityLog.LogCategory.none);
        card.EffectButton();
    }

    public override bool EnableButton()
    {
        EffectNugget myTurnEffect = card.effectNuggets[((byte)EffectNugget.InvokeTiming.MyTurn)];
        //自分ターン起動効果を持っている/ターン内使用可能回数を超えていない/効果の発動条件を満たしている/保存されていない/自分ターンである
        return card.CanUseMyTurnEffect() && card.CanMyTurnCommand();
    }
}
