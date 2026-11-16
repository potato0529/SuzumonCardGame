using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DeckEditorManager : MonoBehaviour
{
    [Header("ドラッガー")] public Menu_Dragger dragger;
    [Header("キャンバス")] public Canvas canvas;
    [Header("編集中のデッキデータを反映するデッキデータスクリプト")] public DeckBoxData editingBox; //このデッキデータは編集中の状況がリアルタイムで反映される。
    [HideInInspector] public DeckBoxData accessingDeckInUserData; //ユーザーデータに既に保存されているデッキを編集している場合、そのデッキをこの変数に代入する。
    [Header("インデッキボックスカードプレハブ")] public Card_InDeckBox cardPrefab;
    [Header("新規デッキボックス(空)")] public DeckBoxData emptyDeckBox;
    [Header("カードスロットジェネレーター")] public DEditor_LCardSlotGenerator slotsGenerator;
    [Header("カード情報キャンバス")] public MiniCardInfoCanvas cardInfoCanvas;
    public CardListArea cardListArea;

    [Header("モンスターカウンター")] public TMP_Text monsterCounterText;
    [HideInInspector] public int monsterCount;
    [Header("アイテムカウンター")] public TMP_Text itemCounterText;
    [HideInInspector] public int itemCount;
    [Header("DeckBoxDataのUI表示順調整用の親")] public Transform deckBoxDataParent;
    [Header("セーブ時通知")] public OneShotNotice saveNotice;

    public static DeckEditorManager instance = null;
    //基本DeckEditorではDeckEditorManagerが最初に処理を行ってそこから他のクラスを読んでいくって感じ
    void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {
        OpenDeckEditor(GameManager.instance.transitionDeckEditorData);//本来はこっち
        //OpenDeckEditor(deckBoxData);//テスト用
        GameManager.instance.transitionDeckEditorData = null;
    }

    /// <summary>
    /// 指定されたデッキ情報を元にデッキエディターを起動する
    /// </summary>
    /// <param name="openData"></param>
    public void OpenDeckEditor(DeckBoxData openData)
    {
        Debug.Log(openData + "を開きます");

        //デッキデータをもとにそのデータと同じボックスの内容を再現する
        if (openData)
        {
            editingBox = Instantiate(openData,canvas.transform,false);
            editingBox.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            editingBox.transform.SetParent(deckBoxDataParent, false);
            Debug.Log("コピーしたデッキデータ" + editingBox);
            editingBox.deckName = openData.deckName;
            editingBox.boxImage = openData.boxImage;
            accessingDeckInUserData = openData;

            foreach (Card_InDeckBox deckBoxCardData in editingBox.cards)
            {
                //あとでListedCardから生成して初期設定する処理とまとめたい
                Debug.Log($"{deckBoxCardData} をロードします");
                //Card_InDeckBox generatedCard = Instantiate(cardPrefab.gameObject).GetComponent<Card_InDeckBox>(); //空のカードを生成
                //generatedCard.transform.SetParent(canvas.transform); //親をキャンバスに設定
                deckBoxCardData.transform.SetParent(canvas.transform,false);
                
                
                deckBoxCardData.belongDeck = editingBox;
                deckBoxCardData.transform.SetParent(editingBox.transform); //falseがあると追加分がなくなる
                deckBoxCardData.GetComponent<RectTransform>().anchoredPosition = deckBoxCardData.positionInDeckBox; //座標を設定
                //deckBoxCardData.sourceListedCard = slotsGenerator.SearchMatchCardData(deckBoxCardData.cardData);
                //generatedCard.cardData = deckBoxCardData.cardData;
                //generatedCard.GetComponent<Image>().sprite = deckBoxCardData.cardData.sprite;
                //editingBox.cards.Add(deckBoxCardData);

                ChangeCardCounter(deckBoxCardData, 1); //カードカウンターを更新
            }

            foreach (Card_InDeckBox deckBoxCardData in editingBox.specialCards)
            {
                //あとでListedCardから生成して初期設定する処理とまとめたい
                Debug.Log($"{deckBoxCardData} をロードします");
                //Card_InDeckBox generatedCard = Instantiate(cardPrefab.gameObject).GetComponent<Card_InDeckBox>(); //空のカードを生成
                //generatedCard.transform.SetParent(canvas.transform); //親をキャンバスに設定
                deckBoxCardData.transform.SetParent(canvas.transform, false);


                deckBoxCardData.belongDeck = editingBox;
                deckBoxCardData.transform.SetParent(editingBox.transform); //falseがあると追加分がなくなる
                deckBoxCardData.GetComponent<RectTransform>().anchoredPosition = deckBoxCardData.positionInDeckBox; //座標を設定
                //deckBoxCardData.sourceListedCard = slotsGenerator.SearchMatchCardData(deckBoxCardData.cardData);
                Debug.Log(deckBoxCardData.sourceListedCard);
                Debug.Log(deckBoxCardData.cardData, deckBoxCardData);
                //generatedCard.cardData = deckBoxCardData.cardData;
                //generatedCard.GetComponent<Image>().sprite = deckBoxCardData.cardData.sprite;
                //editingBox.cards.Add(deckBoxCardData);
            }
        }
        else
        {
            editingBox = emptyDeckBox;
        }
        
        Debug.Log("デッキのロードが完了！");

        //imp:（順番）この2行はもともとこのメソッドの最後にあったのでこれで問題が生じたら戻しましょう
        slotsGenerator.deckBoxData = editingBox;
        slotsGenerator.EnumerateDeckList(GameManager.instance.userData.possessionCards.ConvertAll(x => (ICardData)x));
    }

    /// <summary>
    /// 作成したデッキをユーザーデータに保存する。※ref+1
    /// </summary>
    public void SaveDeckData()
    {
        Debug.Log("デッキデータをユーザーデータに送ります");
        //ユーザーデータに既に保存されているデータを編集している場合
        if (accessingDeckInUserData)
        {
            //編集中のデッキデータをユーザーデータに上書きする

            DeckBoxData savedData = Instantiate(editingBox);
            savedData.transform.SetParent(GameManager.instance.userData.transform);
            //アクセスしているユーザーデータ内のデータと編集データを置き換える
            Destroy(GameManager.instance.userData.GetDeckBoxDatas()[GameManager.instance.userData.GetDeckBoxDatas().IndexOf(accessingDeckInUserData)].gameObject);
            GameManager.instance.userData.GetDeckBoxDatas()[GameManager.instance.userData.GetDeckBoxDatas().IndexOf(accessingDeckInUserData)] = savedData;      
            accessingDeckInUserData = savedData;
        }
        //新規のデッキを編集している場合
        else
        {
            editingBox.name = "Deck" + (GameManager.instance.userData.GetDeckBoxDatas().Count + 1);
            editingBox.deckName = "Deck" + (GameManager.instance.userData.GetDeckBoxDatas().Count + 1); 
            accessingDeckInUserData = GameManager.instance.userData.AddNewDeck(editingBox);
        }

        saveNotice.StartFade();
    }

    /// <summary>
    /// デッキエディターを閉じ、メニューに戻ってデッキリストを表示する
    /// </summary>
    public void CloseDeckEditor()
    {
        Debug.Log("デッキエディターを閉じ、デッキリスト画面に移行します");
        GameManager.instance.transitionMenuType = MenuTransition.MenuType.DeckList;
        GameManager.instance.TransitionMenu(MenuTransition.MenuType.DeckList);
    }

    /// <summary>
    /// デッキにカードが追加された時のカードカウンター処理
    /// </summary>
    /// <param name="card_InDeckBox"></param>
    public void ChangeCardCounter(Card_InDeckBox card_InDeckBox,int changeValue)
    {
        if(card_InDeckBox.cardData.cardType == Card.CardType.Monster)
        {
            monsterCount += changeValue;
            monsterCounterText.text = "モンスター:" + monsterCount;
        }
        else
        {
            itemCount += changeValue;
            itemCounterText.text = "アイテム:" + itemCount;
        }
    }
}
