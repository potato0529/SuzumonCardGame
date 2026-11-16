using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using TMPro;

public class MatchPlayerScreen : MonoBehaviourPunCallbacks
{ 
    [Header("パーソナルUIキャンバス")] public PersonalUICanvas personalUICanvas;
    [Header("キャンバスポインター")] public CanvasPointer canvasPointer;
    [Header("カードインフォメーションメニュー")] public CardInfoMenu cardInfoMenu;
    [Header("直接攻撃アイコン")] public Image directAttackIcon;
    [Header("ターン終了ボタン")] public Image turnEndButton;
    [Header("スタートアニメーション")] public GameObject startAnim;
    [Header("勝利アニメーション")] public GameObject winAnim;
    [Header("敗北アニメーション")] public GameObject loseAnim;
    [Header("サブガイド(ライフブロッカー関連など補助的)")] public TMP_Text guideText;
    [Header("試合終了時キャンバス")] public EndGameCanvas gameEndCanvas;
    [Header("ターゲット決定ボタン")] public Button targetsDecideButton;
    [Header("ライフ生贄ボタン")] public Button lifeSacrificeButton;
    [Header("【サイコロ】1～6の出目イメージ")] public Image[] pipImages = new Image[6];
    [Header("ターン数")] public TMP_Text turnNum;
    [Header("ターンフレーム")] public Image turnFrame;
    [Header("自分ターンフレームスプライト")] public Sprite myTurnFrameSprite;
    [Header("相手ターンフレームスプライト")] public Sprite opponentTurnFrameSprite;
    [Header("ゲームガイド")] public GameObject gameGuide;
    [Header("ゲームガイドボタンテキスト")] public TMP_Text gameGuideButtonText;
    [Header("LinkedGUIレイヤー")] public Transform linkedGUILayer;
    [Header("アクションフェイズガイドテキスト")] public TMP_Text actionPhaseGuideText;
    [Header("低層アニメーションレイヤー")] public Transform lowLayerAnim;
    [Header("サポートガイド")] public SideSupportGuide sideSupportGuide;

    private void Start()
    {
        startAnim.SetActive(true);
    }

    public void ShowTurnEndButton()
    {
        turnEndButton.gameObject.SetActive(true);
    }

    public void HideTurnEndButton()
    {
        turnEndButton.gameObject.SetActive(false);
    }

    public IEnumerator DisplayGuide(string text,float displayTime)
    {
        guideText.transform.parent.gameObject.SetActive(true);
        guideText.text = text;

        //表示時間を0以下にすると時間制限なく表示される
        if(displayTime <= 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(displayTime);
        if(guideText.text == text)
        {
            guideText.text = "";
            guideText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void HideGuide()
    {
        guideText.text = "";
        guideText.transform.parent.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.66f);
        guideText.transform.parent.gameObject.SetActive(false);        
    }

    /// <summary>
    /// 勝利演出
    /// </summary>
    public IEnumerator WinEffect()
    {
        //winText.gameObject.SetActive(true);
        winAnim.SetActive(true);

        yield return null;
        AfterEffect();
    }


    /// <summary>
    /// 敗北演出
    /// </summary>
    public IEnumerator LoseEffect()
    {
        //loseText.gameObject.SetActive(false);
        loseAnim.SetActive(true);
        
        yield return null;
        AfterEffect();
    }

    /// <summary>
    /// 演出終了後共通処理
    /// </summary>
    public void AfterEffect()
    {
        gameEndCanvas.gameObject.SetActive(true);
    }

    public IEnumerator DiceAnim(int result,Action endAnimCallback)
    {
        //インデックスは0からだから1ひく。
        Image displayDiceImage = pipImages[result - 1];

        yield return null;

        //出目を画面に表示する。
        displayDiceImage.gameObject.SetActive(true);
        //displayDiceImage.GetComponent<RectTransform>().position = Vector2.zero;

        yield return new WaitForSeconds(1);

        displayDiceImage.gameObject.SetActive(false);

        if (endAnimCallback != null)
        {
            endAnimCallback();
        }
    }

    public void GameGuideButton()
    {
        if(gameGuideButtonText.text == "説明を見る")
        {
            gameGuide.gameObject.SetActive(true);
            gameGuideButtonText.text = "説明を閉じる";
        }
        else if(gameGuideButtonText.text == "説明を閉じる")
        {
            gameGuide.gameObject.SetActive(false);
            gameGuideButtonText.text = "説明を見る";
        }
        
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public void DisplayActionPhaseGuide(string text)
    {
        actionPhaseGuideText.text = text;
        actionPhaseGuideText.transform.parent.gameObject.SetActive(true);
    }

    public void HideActionPhaseGuide()
    {
        actionPhaseGuideText.text = "";
        actionPhaseGuideText.transform.parent.gameObject.SetActive(false);
    }

}
