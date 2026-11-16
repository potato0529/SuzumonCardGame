using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchMaker : MonoBehaviourPunCallbacks
{
    [Header("マスターが先行確定")] public bool confirmMasterFirst;
    private bool localReady;
    private bool opponentReady;
    [Header("CPUプレイヤーデータ")] public MatchPlayerData cpuMatchPlayerData;

    private MatchMenuSettings matchMenuSettings;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// マッチング開始時に自分のデッキデータなどを登録する
    /// </summary>
    public void MatchingSetUp(MatchMenuSettings _matchMenuSettings)
    {
        //プレイヤーデータにバトルデッキに登録されているデッキを代入(この後GM.matchMaker.matchPlayerDatasにも同じデータを代入するが、userDataの方は単なるローカルの基本データで、GM.matchMaker.matchPlayerDatasの方はマッチ用のデータというイメージ
        GameManager.instance.userData.matchPlayerData.deckData = GameManager.instance.userData.battleDeck;

        UserData userData = GameManager.instance.userData; //複数回3段階参照するのが冗長なのでローカル変数に置き換えただけ

        //ここで自分のデッキデータを試合用に登録。対戦相手用のこれに相当するものはMatchPlayerDatas.GetMatchPlayerDatas()で登録される

        GameManager.instance.matchMaker.matchPlayerDatas[0].RegisterDeckData(userData.battleDeck);
        //imp:下の8行全てがこの上の1行に変わった
        //for (int loops = 0; loops < userData.battleDeck.cards.Count; loops++)
        //{
        //    GameManager.instance.matchMaker.matchPlayerDatas[0].deckCards.Add(userData.battleDeck.cards[loops].cardData);
        //}
        //for (int loops = 0; loops < userData.battleDeck.specialCards.Count; loops++)
        //{
        //    GameManager.instance.matchMaker.matchPlayerDatas[0].specialDeckCards.Add(userData.battleDeck.specialCards[loops].cardData);
        //}

        matchMenuSettings = _matchMenuSettings;

        //af:シーン遷移時に情報を引き継ぐため仮にGameManagerにこのフィールドを置いているが、応急措置なので別の管理方法に変更される場合はここも対応すること。
        GameManager.instance.matchMenuSettings = _matchMenuSettings;

    }

    public void MatchingCancel()
    {
        PhotonNetwork.LeaveRoom();
        GameManager.instance.userData.matchPlayerData.deckData = null;
        
        GameManager.instance.matchMaker.matchPlayerDatas[0].deckCards = new List<CardData>();
            
        GameManager.instance.matchMaker.matchPlayerDatas[0].specialDeckCards = new List<CardData>();
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        //あとでルーム名は固有の名前にする
        PhotonNetwork.CreateRoom("wiudediuaidaa",roomOptions);

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("ルームへ参加しました！デュエル開始の宣言をしろ！");
        SetMatchData();
        //もともとルームにプレイヤーがいた場合、そのプレイヤーのデータを取得する
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("テスト");
            GameManager.instance.matchMaker.matchPlayerDatas[1].GetMatchPlayerDatas(PhotonNetwork.MasterClient, PhotonNetwork.MasterClient.CustomProperties);
        }
        
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if(playerCount == 2)
        {
            Debug.Log("相手のデュエリストが見つかったぜ！");
        }
        else
        {
            Debug.Log("相手のデュエリストを探しているぜ！");
        }
    }

    /// <summary>
    /// ルームに自分以外のプレイヤーが参加したとき
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {        

        //MasterClientが代表して部屋を操作する。MasterClientは最初から部屋にいるので人がそろったときのOnPlayerEnteredRoom()を見逃すことはない MasterClientでしかLoadlevel()を呼べないらしい
        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                
                Debug.Log("相手のデュエリストが見つかったぜ！");

                //コイントス af:コルーチンにする予定
                var hashtable = new ExitGames.Client.Photon.Hashtable();

                int isMasterFirst = Random.Range(0, 2);
                if (confirmMasterFirst)
                {
                    isMasterFirst = 1;
                }

                if (isMasterFirst == 1)
                {
                    hashtable["IsMasterFirst"] = true;
                    Debug.Log("マスターが先攻！");
                }
                else
                {
                    hashtable["IsMasterFirst"] = false;
                    Debug.Log("マスターが後攻！");
                }

                //部屋の乱数シードを抽選する
                int seed = 0;
                //シードが未設定(-1)なら抽選、設定済みならそれに従う
                if(matchMenuSettings.Seed == -1)
                {
                    seed = Random.Range(0, 1000000);
                }
                else
                {
                    seed = matchMenuSettings.Seed;
                }

                hashtable["RoomRandomSeed"] = seed;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }
        }
    }

    /// <summary>
    /// 部屋に入ったとき、自分のMatchPlayerDataを登録する
    /// </summary>
    public void SetMatchData()
    {
        var hashtable = new ExitGames.Client.Photon.Hashtable();

        UserData userData = GameManager.instance.userData;
        int[] mainDeckCardIds = new int[userData.battleDeck.cards.Count];
        for (int loops = 0; loops < userData.battleDeck.cards.Count; loops++)
        {
            //登録されたバトルデッキからメインデッキのそれぞれのカードのデータベース上の番号を取得する
            mainDeckCardIds[loops] = GameManager.instance.cardDataBase.cardDatas.IndexOf(userData.battleDeck.cards[loops].cardData);
        }

        int[] spDeckCardIds = new int[userData.battleDeck.specialCards.Count];
        for (int loops = 0; loops < userData.battleDeck.specialCards.Count; loops++)
        {
            //登録されたバトルデッキからスペシャルデッキのそれぞれのカードのデータベース上の番号を取得する
            spDeckCardIds[loops] = GameManager.instance.cardDataBase.cardDatas.IndexOf(userData.battleDeck.specialCards[loops].cardData);
        }

        hashtable.Add("mainDeckCardIds", mainDeckCardIds);
        hashtable.Add("spDeckCardIds", spDeckCardIds);

        hashtable.Add("customFirst", GameManager.instance.userData.customFirst);
        hashtable.Add("customLife", GameManager.instance.userData.customLife);
        hashtable.Add("playerName", "GangNum");

        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //ローカルではない（つまり対戦相手）のデッキデータが送られてきたとき、ローカル側の、相手プレイヤーデータを格納する変数に保存する
        if (!targetPlayer.IsLocal)
        {
            //mainDeckCardIdsと同時に送られるセットをすべて処理する
            if (changedProps.ContainsKey("mainDeckCardIds"))
            {
                Debug.Log("テスト");
                GameManager.instance.matchMaker.matchPlayerDatas[1].GetMatchPlayerDatas(targetPlayer, changedProps);
            }
        }

        if (changedProps.ContainsKey("PlayerReady"))
        {
            if(targetPlayer == PhotonNetwork.LocalPlayer)
            {
                Debug.Log("自分の準備ができました");
                localReady = true;
                GetReadyStartMatch();
            }
            else
            {
                Debug.Log("相手の準備ができました");
                opponentReady = true;
                GetReadyStartMatch();
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //af:もしかしてこれIsMasterFirst使われてない？シードさえ統一すればマスターが先攻かどうか伝える手間省けるからやめたのに処理消し忘れたとか？ちょっとパット見分からないから要確認
        if (propertiesThatChanged.ContainsKey("IsMasterFirst"))
        {
            Debug.Log("先攻後攻、乱数シードの通知が来ました");
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable["PlayerReady"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }

    }

    /// <summary>
    /// マッチを開始できるか確認し、できれば開始処理をする
    /// </summary>
    public void GetReadyStartMatch()
    {
        //af:ここに置くのは適切なのかわからん
        GameManager.instance.isSoloMode = false;

        if (PhotonNetwork.IsMasterClient && localReady && opponentReady)
        {
            PhotonNetwork.LoadLevel("DuelField");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"ルームへの参加に失敗しました: {message}");
    }
}
