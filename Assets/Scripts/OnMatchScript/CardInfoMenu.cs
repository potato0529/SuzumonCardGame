using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfoMenu : MonoBehaviour
{
    [Header("対応カメラ")] public Camera cameraToDisplay;
    [Header("攻撃ボタン")] public AttackButton attackButton;
    [Header("効果発動ボタン")] public EffectButton effectButton;
    [Header("ディフェンスボタン")] public DefenceButton defenceButton;
    [Header("情報閲覧ボタン")] public GameObject infoButton;
    [Header("閉じるボタン")] public CloseButton closeButton;
    [Header("降臨ボタン")] public AdventButton adventButton;
    private Vector2 cardPosOnScreen;
    public Card openingMenuCard;
    public List<CardButton> nowButtons = new List<CardButton>();
    public bool opening = false;
    
    public void OpenMenuOnCanvasUI(Card card, Vector3 displayPos, Transform buttonsParent, List<CardButton> cardButtons)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), displayPos, null, out var uiLocalPos);
        //OpenMenu(card, uiLocalPos , buttonsParent, cardButtons);
        OpenMenu(card, new Vector2(0,0), buttonsParent, cardButtons);
    }

    public void OpenMenuOn3DSpace(Card card, List<CardButton> cardButtons)
    {
        
        Vector2 displayPos = cameraToDisplay.WorldToScreenPoint(card.transform.position);
        //Vector2 displayPos = RectTransformUtility.WorldToScreenPoint(cameraToDisplay,card.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), displayPos, null, out var uiLocalPos);
        OpenMenu(card, uiLocalPos, card.controlerPlayer.canvas.transform, cardButtons);
    }

    /// <summary>
    /// カードメニューを開いて画面に表示する
    /// </summary>
    /// <param name="card"></param>
    /// <param name="cardButtons"></param>
    private void OpenMenu(Card card,Vector2 displayPos,Transform buttonsParent,List<CardButton> cardButtons)
    {
        UtilityLog.CardLog(card.gameObject.name + "のメニューを開きます" + gameObject.name, card, UtilityLog.LogCategory.none);
        opening = true;
        openingMenuCard = card;
        cardPosOnScreen = displayPos - new Vector2(0,170);
        nowButtons = cardButtons;

        transform.SetParent(buttonsParent);

        //まず全てのボタンを閉じる
        attackButton.Close(false);
        effectButton.Close(false);
        defenceButton.Close(false);
        closeButton.Close(false);
        adventButton.Close(false);

        int buttonDistance = 150;

        //閉じるボタンは確定であるので先に設定
        closeButton.GetComponent<RectTransform>().localPosition = cardPosOnScreen + new Vector2(0, -buttonDistance);
        closeButton.Open(card,true);

        //引数で受け取ったボタンの配置
        byte buttonNum = 0;
        foreach(CardButton button in cardButtons)
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, button + "を開きます");
            buttonNum++;
            Vector2 buttonPos = Vector2.zero;
            switch (buttonNum)
            {
                case 1:
                    buttonPos = new Vector2(0, buttonDistance);
                    break;

                case 2:
                    buttonPos = new Vector2(-buttonDistance * 0.5f, 0);
                    break;

                case 3:
                    buttonPos = new Vector2(buttonDistance * 0.5f, 0);
                    break;

                case 4:
                    buttonPos = new Vector2(0, -buttonDistance);
                    break;
            }
            //もともとlocalだったけどlocalじゃないとダメなん？
            button.GetComponent<RectTransform>().localPosition = cardPosOnScreen + buttonPos;
            //button.GetComponent<RectTransform>().anchoredPosition += buttonPos;
            button.Open(card,true);
        }
    }

    public void CloseMenu(Card card)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, "メニューを閉じます");
        attackButton.Close(true);
        effectButton.Close(true);
        closeButton.Close(true);
        adventButton.Close(true);
        defenceButton.Close(true);
        openingMenuCard = null;
        opening = false;
    }

    /// <summary>
    /// ボタンの状態（使用可能か）を更新する
    /// </summary>
    public void UpdateButtonState()
    {
        if (opening)
        {
            foreach (CardButton cardButton in nowButtons)
            {
                cardButton.UpdateButtonState();
            }
        }
    }
}
