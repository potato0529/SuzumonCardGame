using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// デッキエディットメニューでデッキに並べるために取り出すカードリストに表示されるカード
/// </summary>
public class ListedCard : ListedUICard
{
    private Menu_Dragger dragger;
    [Header("クリックすると生成されるデッキに入れるためのカード(プレハブ)")] public Card_InDeckBox deckCard;
    [SerializeField,Header("所持数")] private int remainingNum;
    [Header("所持数のUIテキスト")] public TextMeshProUGUI remmainingNumText;

    public CardData CardData
    {
        get { return cardData; }
        set { cardData = value; Debug.Log($"カードデータが{value}に変更されました"); }
    }

    public int RemainingNum
    {
        get { return remainingNum; }
        set 
        { 
            remainingNum = value;
            //Debug.Log(remmainingNumText.text,remmainingNumText.gameObject);
            remmainingNumText.text = remainingNum.ToString();
        }
    }

    private void Awake()
    {
        remmainingNumText.text = remainingNum.ToString();
        dragger = DeckEditorManager.instance.dragger;
    }

    
    /// <summary>
    /// クリックされたとき（押した瞬間）※ref+1
    /// </summary>
    public void PointerDowned()
    {
        if (remainingNum > 0 && DeckEditorManager.instance.editingBox.GetInDeckCardNum(cardData) < cardData.deckLimit)
        {
            Card_InDeckBox card_InDeckBox = Instantiate(deckCard);
            //生成カードの初期設定
            card_InDeckBox.cardData = cardData;
            card_InDeckBox.transform.SetParent(DeckEditorManager.instance.canvas.transform,false);
            card_InDeckBox.GetComponent<Image>().sprite = cardData.sprite;
            card_InDeckBox.sourceListedCard = this;
            
            RemainingNum--;
            dragger.GrabCard(card_InDeckBox);            
            Debug.Log(card_InDeckBox + "が生成されました");
        }
        else
        {
            Debug.Log("もうこのカードを所持していません");
        }

        DeckEditorManager.instance.cardInfoCanvas.gameObject.SetActive(true);
        DeckEditorManager.instance.cardInfoCanvas.ShowCardInfo(cardData);
    }

    
}
