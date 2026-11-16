using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListedPiledCard : ListedUICard
{
    public Card topCard;//このカードを重ねている一番上のカード
    public Card sourceCard;
    public bool isEquippingWeapon;
    private FocusedUI focusedUI;
    private List<CardButton> cardButtons = new List<CardButton>();
    private CardInfoMenu cardInfoMenu;

    private void Start()
    {
        focusedUI = GetComponent<FocusedUI>();
        cardInfoMenu = sourceCard.ownerPlayer.playerCanvas.cardInfoMenu;
    }

    public void Clicked()
    {     //   ウェポンである   上のカードに装備されているウェポンである       //MyTurn効果を持っている
        if (sourceCard.weapon && sourceCard.weapon.equipTarget.card == topCard && sourceCard.effectNuggets[(byte)EffectNugget.InvokeTiming.MyTurn])
        {
            cardButtons.Clear();
            cardButtons.Add(cardInfoMenu.effectButton);
            cardButtons.Add(cardInfoMenu.closeButton);
            cardInfoMenu.OpenMenuOnCanvasUI(sourceCard, GetComponent<RectTransform>().position, transform, cardButtons);

            //フォーカス管理
            foreach(CardButton cardButton in cardButtons)
            {
                cardButton.GetComponent<FocusedUI>().isFocused = true;
            }
        }
    }

    public void Close()
    {
        focusedUI.isFocused = false;
        foreach(CardButton cardButton in cardButtons)
        {
            cardButton.GetComponent<FocusedUI>().isFocused = false;
        }
        cardInfoMenu.CloseMenu(sourceCard);
    }

    
}
