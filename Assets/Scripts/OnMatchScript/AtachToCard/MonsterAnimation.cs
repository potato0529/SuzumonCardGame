using System.ComponentModel.Design;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterAnimation : MonoBehaviour
{
    [Header("ディフェンスアニメーションプレハブ")] public DefenceAnim defenceAnimPrefab;
    [Header("耐えるアニメーションプレハブ")] public GameObject withstandAnimPrefab;
    [Header("耐える破壊アニメーションプレハブ")] public GameObject withstandBrokenAnimPrefab;

    /// <summary>
    /// ディフェンスアニメーションを再生する
    /// </summary>
    public void PlayDefenceAnimation()
    {
        //両プレイヤーに対して表示 ※両プレイヤーに対し共通UIを表示するメソッドを作りたかったが、各canvasの特定要素を同時に親として指定できないので断念
        for (int i = 0; i < MatchManager.instance.players.Length; i++)
        {
            Player player = MatchManager.instance.players[i];
            MatchManager.instance.ShowUIAtWorldPosPosition(defenceAnimPrefab.gameObject, transform.position, player.playerCanvas.lowLayerAnim, player.cameraScr);
        }
    }

    /// <summary>
    /// 耐えるアニメーションを再生する
    /// </summary>
    public void PlayWithstandAnimation()
    {
        //両プレイヤーに対して表示
        for (int i = 0; i < MatchManager.instance.players.Length; i++)
        {
            Player player = MatchManager.instance.players[i];
            MatchManager.instance.ShowUIAtWorldPosPosition(withstandAnimPrefab.gameObject, transform.position, player.playerCanvas.lowLayerAnim, player.cameraScr);
        }

    }

    public void PlayWithstandBrokenAnimation()
    {
        //両プレイヤーに対して表示
        for (int i = 0; i < MatchManager.instance.players.Length; i++)
        {
            Player player = MatchManager.instance.players[i];
            MatchManager.instance.ShowUIAtWorldPosPosition(withstandBrokenAnimPrefab.gameObject, transform.position, player.playerCanvas.lowLayerAnim, player.cameraScr);
        }
    }
}
