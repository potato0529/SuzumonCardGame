using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NO : YNPanel
{
    public override void OnClicked()
    {
        //この分岐は仮、ifとelseとelseの中身を消し、Submitする部分だけ残す。詳しくはyesNo.doSendCommandModeの隣に書いてあるコメントを参照
        if (yesNo.doSendCommandMode)
        {
            MatchManager.instance.matchActionService.Submit(new YesNoCommand(yesNo.pScr, false));
        }
        else
        {
            DecidedCP();
        }
    }

    /// <summary>
    /// NOがクリックされたとき
    /// </summary>
    protected override void DecideChoice()
    {
        Debug.Log("NOクリック");
        callback.Invoke();
        callback = null;
        //imp:旧通信処理
        //if (yesNo.pScr == MatchManager.instance.localPlayer && !yesNo.ifNoIsChosenDontRPC)
        //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentDecideYesNo), Photon.Pun.RpcTarget.Others, false);
    }
}
