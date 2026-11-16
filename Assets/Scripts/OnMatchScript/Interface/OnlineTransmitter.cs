using UnityEngine;
using Newtonsoft.Json;

public class OnlineTransmitter : IMatchTransmitter
{
    /// <summary>
    /// commandのデータを対戦相手に送信する
    /// </summary>
    /// <param name="command"></param>
    public void Send(PlayerCommand command)
    {
        Debug.Log($"コマンド {command} を送信します");
        MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.ReceiveOpponentCommand),Photon.Pun.RpcTarget.Others,CommandSerializer.Serialize(command));
    }
}
