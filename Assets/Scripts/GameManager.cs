using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Debug.Logの無効化")] public bool disableDebugLog;
    [Header("ユーザーデータ")] public UserData userData;
    [Header("カードデータベース")] public CardDataBase cardDataBase;
    [Header("マッチメーカー")] public RoomDatas matchMaker; //今は仮で端末ごと管理のGMのフィールドにありますが今後サーバーサイドに移行します
    [Header("空のデッキカードデータ")] public Card_InDeckBox deckCardPrefab;
    
    [Header("アセットデータ")] public AssetsDatas assetsDatas;
    [Header("ログコントローラー")] public LogController logController;
    [Header("ユニークテキストマネージャー")] public UniqueTextManager uniqueTextManager;

    [Header("CPUプレイヤーデータ")] public MatchPlayerData cpuMatchPlayerData;//仮置き　どこにこのデータを置けばいいのかわかっていない
    [Header("デバッグコンソールポップアップ")] public GameObject inGameDebugConsoleObj;
    private bool debugToggle = false;


    public static GameManager instance = null;
    public MenuTransition.MenuType transitionMenuType;
    public DeckBoxData transitionDeckEditorData;
    public DeckBoxData useMatchDeckData;
    public bool isAfterMatchTransition;//試合後の遷移である（試合後の初期化などの処理を行うため
    public MouseRaycast mouseRaycast;

    //次の2つはシーン遷移後も引き継ぎたいのでここに置いているが、好ましくないのでいずれは受け渡し機構でこの情報を渡すようにしたいので、消す予定
    [HideInInspector] public bool isSoloMode = false;

    public MatchMenuSettings matchMenuSettings = null;

    #region memo
    //GameManagerはそれぞれの端末での情報を管理するためサーバー用ではない
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (disableDebugLog)
            Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        inGameDebugConsoleObj.SetActive(false);
    }

    /// <summary>
    /// デバッグ用
    /// </summary>
    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard.semicolonKey.wasPressedThisFrame)
        {
            debugToggle = !debugToggle;
            inGameDebugConsoleObj.SetActive(debugToggle);
        }
    }
    #region シーン変更関連 ※基本他のスクリプトのボタン反応メソッドから引数を場合によって入れてそこからこのメソッドを呼ぶ

    /// <summary>
    /// メニューシーンに遷移する(メニューのうち、どの画面を開きたいかを設定)
    /// </summary>
    public void TransitionMenu(MenuTransition.MenuType menuType)
    {
        transitionMenuType = menuType;
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// デッキエディターシーンに遷移する
    /// </summary>
    /// <param name="thisTransitionDeckEditorData">開きたいデッキエディターの内容を設定</param>
    public void TransitionDeckEditor(DeckBoxData thisTransitionDeckEditorData)
    {
        transitionDeckEditorData = thisTransitionDeckEditorData;
        Debug.Log($"デッキ {thisTransitionDeckEditorData} を開きます");
        
        SceneManager.LoadScene("DeckEditor");
    }
    #endregion
}
