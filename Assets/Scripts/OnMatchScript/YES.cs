using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class YES :YNPanel
{
    public override void OnClicked()
    {        
        MatchManager.instance.matchActionService.Submit(new YesNoCommand(yesNo.pScr, true));
    }

    /// <summary>
    /// YESがクリックされたとき
    /// </summary>
    protected override void DecideChoice()
    {
        Debug.Log("YESクリック");
        callback.Invoke();
        callback = null;
        //imp:旧通信処理
        //if (yesNo.pScr == MatchManager.instance.localPlayer)
        //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentDecideYesNo), Photon.Pun.RpcTarget.Others, true);
    }
}
