using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTransition : MonoBehaviour
{
    [Header("現在のメニュータイプ対応キャンバス")] public ActivityScreen nowScreen;
    [Header("各アクティビティメニュー")] public ActivityScreen[] activityScreens;//[0]メインメニュー,[1]デッキリスト
    [Header("メニュー遷移サウンド")] public AudioSource transitionSound;
    [Header("マッチメニュー遷移あにね")] public GameObject matchmenuanim;
    public OneShotNotice shopnotice;

    public enum MenuType
    {
        none,
        MainMenu,
        DeckList,
        Gacha,
        MatchMenu
    }

    // Start is called before the first frame update
    void Start()
    {
        //GameManagerに設定されたメニュータイプを取得してそれに応じて開くメニューを判断して開く。
        switch (GameManager.instance.transitionMenuType)
        {
            case MenuType.MainMenu:
                Debug.Log("メインメニューを起動します");
                ChangeScreen(activityScreens[0]);
                break;

            case MenuType.DeckList:
                Debug.Log("デッキリストを表示します");
                ChangeScreen(activityScreens[1]);
                break;
        }
        GameManager.instance.transitionMenuType = MenuType.none;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 全てメニュータイプキャンバスを非表示にし、選んだメニューキャンバスを表示させる。
    /// </summary>
    public void ChangeScreen(ActivityScreen thisActivityScreen)
    {
        foreach(ActivityScreen activityScreen in activityScreens)
        {
            activityScreen.HiddenScreen();
        }
        thisActivityScreen.DisplayScreen();
        transitionSound.PlayOneShot(transitionSound.clip);
    }

    /// <summary>
    /// メインメニューに遷移する
    /// </summary>
    public void TransitionMainMenu()
    {
        ChangeScreen(activityScreens[0]);
    }

    /// <summary>
    /// デッキリスト画面に
    /// </summary>
    public void TransitionDeckList()
    {
        ChangeScreen(activityScreens[1]);
    }

    /// <summary>
    /// ガチャ画面に
    /// </summary>
    public void TransitionGacha()
    {
        shopnotice.StartFade();
        //ChangeScreen(activityScreens[2]);
    }

    public void TransitionMatchMeu()
    {
        matchmenuanim.SetActive(true);
        matchmenuanim.GetComponent<VideoPlayerGeneral>().PlayVideo();
        TransMame().Forget();
    }

    public async UniTask TransMame()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.3f));
        matchmenuanim.SetActive(false);
        ChangeScreen(activityScreens[3]);
    }
}
