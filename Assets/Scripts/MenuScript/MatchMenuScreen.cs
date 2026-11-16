using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メニューシーンの中のマッチメニューの主にUI操作を受け取る
/// </summary>
public class MatchMenuScreen : ActivityScreen
{
    [Header("マッチメーカー")] public MatchMaker matchMaker;
    [Header("マッチング開始ボタン")] public Button stratMatchMakeButton;
    [Header("マッチングキャンセルボタン")] public Button MatchingCancelButton;
    [Header("CPUマッチボタン")] public Button cpuMatchButton;
    [Header("ライフセッター")] public TMP_InputField lifeSettorInputField;
    [Header("シード値セッター")] public TMP_InputField seedSettorInputField;
    [Header("先攻後攻セッター")] public Toggle firstSettor;

    [Header("バトルデッキ未登録警告")] public OneShotNotice noBattleDeckRegisteredNotice;
    [Header("マッチングルームID入力")] public TMP_InputField roomIdInputField;

    private int customSeed = -1;//-1だと未設定というしるし

    /// <summary>
    /// マッチング中の状態にする(B)
    /// </summary>
    public void StartMatchMake()
    {
        if (GameManager.instance.userData.matchPlayerData && GameManager.instance.userData.battleDeck)
        {
            matchMaker.MatchingSetUp(GetMatchMenuSettings());
            HideRootMenu();
            MatchingCancelButton.gameObject.SetActive(true);
            HideRootMenu();
            //GameManager.instance.matchMaker.AddPlayerToQueue(GameManager.instance.userData.matchPlayerData);

            if (roomIdInputField.text == "")
            {
                PhotonNetwork.JoinRandomRoom();
                UtilityLog.DebugLog("ランダムマッチングを開始");
            }
            else
            {
                var roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 2;
                roomOptions.IsVisible = false;

                PhotonNetwork.JoinOrCreateRoom(roomIdInputField.text, roomOptions, TypedLobby.Default);

                UtilityLog.DebugLog("ルームID指定マッチングを開始" + roomIdInputField.text);
            }
        }
        else
        {
            Debug.Log("デッキが選択されてないよ！");
            noBattleDeckRegisteredNotice.StartFade();
        }   
    }

    public void CancelMatching()
    {
        matchMaker.MatchingCancel();
        stratMatchMakeButton.gameObject.SetActive(true);
        MatchingCancelButton.gameObject.SetActive(false);
        cpuMatchButton.gameObject.SetActive(true);
        roomIdInputField.interactable = true;
    }

    /// <summary>
    /// ライフセッターの決定ボタンが押される(ref+1)
    /// </summary>
    public void LifeSettorDecide()
    {
        GameManager.instance.userData.customLife = int.Parse(lifeSettorInputField.text);
        GameManager.instance.userData.matchPlayerData.customLife = int.Parse(lifeSettorInputField.text);
    }

    //ref+1:"CustomFirst"toggle
    public void FirstDecide(bool toggle)
    {
        //af:ここでわざわざ同じ内容を複数クラスに反映しなければならないのはよくないかも
        GameManager.instance.userData.customFirst = firstSettor.isOn;
        GameManager.instance.userData.matchPlayerData.customFirst = firstSettor.isOn;
    }

    /// <summary>
    /// シード値セッターの決定ボタンが押されるref+1
    /// </summary>
    public void OnClickedDecideSeed()
    {
        customSeed = int.Parse(seedSettorInputField.text);
    }

    /// <summary>
    /// CPU戦開始の初期処理を行う
    /// </summary>
    public void StartCPUMatch()
    {
        if (GameManager.instance.userData.matchPlayerData && GameManager.instance.userData.battleDeck)
        {
            matchMaker.MatchingSetUp(GetMatchMenuSettings());
            HideRootMenu();
            //GameManager.instance.matchMaker.AddPlayerToQueue(GameManager.instance.userData.matchPlayerData);←おそらく全体用サーバーが管理すると思って設計してたものの跡かな...?念のため残す
            GameManager.instance.matchMaker.matchPlayerDatas[1] = GameManager.instance.cpuMatchPlayerData;
            GameManager.instance.cpuMatchPlayerData.RegisterDeckData(GameManager.instance.cpuMatchPlayerData.deckData);

            GameManager.instance.isSoloMode = true;
            PhotonNetwork.LoadLevel("DuelField");
        }
        else
        {
            Debug.Log("デッキが選択されてないよ！");
            noBattleDeckRegisteredNotice.StartFade();
        }
    }

    /// <summary>
    /// MatchMenuScreenにおける最上層のメニューを隠す
    /// </summary>
    public void HideRootMenu()
    {
        stratMatchMakeButton.gameObject.SetActive(false);
        cpuMatchButton.gameObject.SetActive(false);
        roomIdInputField.interactable = false;
    }

    /// <summary>
    /// MatchMenu内の各設定をもとにMatchMenuSettingsを生成して得る
    /// </summary>
    /// <returns></returns>
    public MatchMenuSettings GetMatchMenuSettings()
    {
        return new MatchMenuSettings(customSeed);
    }
}
