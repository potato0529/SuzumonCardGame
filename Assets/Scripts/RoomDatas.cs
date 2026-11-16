using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// このクラスはサーバーで管理している想定です
/// </summary>
public class RoomDatas : MonoBehaviour
{
    [Header("マッチングキュー"),SerializeField] private List<MatchPlayerData> matchingQueue = new List<MatchPlayerData>();

    [Header("マッチプレイヤーデータ")] public MatchPlayerData[] matchPlayerDatas = new MatchPlayerData[2];

    [Header("相手のカスタム先攻設定")] public bool opponentCustomFirst;
    
    //本来は、まずサーバーの言語とこっちの言語が異なると思うけどとりあえずうまい具合にc#からこっちのサーバーのマッチメーカーの下のメソッドを呼び出してマッチングを行うという感じ
    /// <summary>
    /// マッチングキューにプレイヤーを追加し、そのままマッチングできればマッチングする
    /// </summary>
    /// <param name="waitingUserData"></param>
    //public void AddPlayerToQueue(MatchPlayerData waitingUserData)
    //{
    //    Debug.Log(waitingUserData + "をマッチングキューに追加します");
    //    matchingQueue.Add(waitingUserData);
    //    if(matchingQueue.Count >= 2)
    //    {
    //        //使用デッキを指定するシステムを仮で作るのめんどいからとりあえず最新の保存したデッキを仮で使用する仕様にしとく
    //        //今はとりまデバッグ楽だし先にcarddataとして抜き取るようにする
    //        UserData userData = GameManager.instance.userData;
    //        //1コメのデッキ
    //        for (int loops = 0; loops < userData.GetDeckBoxDatas()[0].cards.Count; loops++)
    //        {
    //            matchingQueue[0].deckCards.Add(userData.GetDeckBoxDatas()[0].cards[loops].cardData);
    //        }
    //        for (int loops = 0; loops < userData.GetDeckBoxDatas()[0].specialCards.Count; loops++)
    //        {
    //            matchingQueue[0].specialDeckCards.Add(userData.GetDeckBoxDatas()[0].specialCards[loops].cardData);
    //        }

    //        //2個目のデッキ
    //        for (int loops = 0; loops < userData.GetDeckBoxDatas()[1].cards.Count; loops++)
    //        {
    //            matchingQueue[1].deckCards.Add(userData.GetDeckBoxDatas()[1].cards[loops].cardData);
    //        }
    //        for (int loops = 0; loops < userData.GetDeckBoxDatas()[1].specialCards.Count; loops++)
    //        {
    //            matchingQueue[1].specialDeckCards.Add(userData.GetDeckBoxDatas()[1].specialCards[loops].cardData);
    //        }

    //        Debug.Log("マッチングしました！");
    //        SceneManager.LoadScene("DuelField"); //本来はマッチユーザー2人のそれぞれのシーンをDuelFieldに切り替える。
    //    }
    //}

    /// <summary>
    /// マッチングキューを取得する
    /// </summary>
    /// <returns></returns>
    public MatchPlayerData[] GetMatchedPlayers()
    {
        return matchPlayerDatas;
    }
}
