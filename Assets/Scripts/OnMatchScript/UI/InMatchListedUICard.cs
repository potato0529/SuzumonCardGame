using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InMatchListedUICard : ListedUICard
{
    public Card sourceCard;

    /// <summary>
    /// ƒNƒŠƒbƒN‚³‚ê‚½Žžref+1
    /// </summary>
    public virtual void OnClick()
    {
        MatchManager.instance.localPlayer.playerCanvas.personalUICanvas.ShowMiniCardInfo(sourceCard);
    }
}
