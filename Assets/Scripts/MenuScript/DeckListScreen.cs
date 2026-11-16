using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckListScreen : ActivityScreen
{
    [Header("デッキスロットジェネレーター")] public DeckSlotGenerator slotGenerator;
    [Header("デッキボックスデータプレハブ(仮)")] public DeckBoxData deckBoxDataPrefab; //クリップボード機能を正式に作ったら修正するからなくなるかも
    public Card_InDeckBox card_InDeckBoxPrefab;
    [Header("デッキデータテキスト")] public TMP_InputField deckValueInputField;
    [Header("デッキ選択フレーム")] public GameObject selectedDeckFrame;
    [Header("右情報UI")] public GameObject rightInfoUI;
    [Header("デッキ名info")] public TMP_Text deckNameInfoText;
    [Header("バトルデッキに登録しました！")] public OneShotNotice registeredBattleDeckNotice;
    [Header("デッキIDをコピーしました")] public OneShotNotice copiedDeckNotice;
    [Header("CPUの使用デッキに登録しました")] public OneShotNotice registeredCPUDeckNotice;

    private DeckBoxImage selectedDeck;

    public string editorDeckId;
    public DeckBoxData editorDeckBox;//従来はeditorDeckIdを使って生成と設置両方をしていたが、今はこっちのほうで別の方で生成してからおいている
    [Header("エディタ用ゲームマネージャー")] public GameManager editorGameManager;
    [Header("エディタ生成デッキの親")] public Transform editorGeneratedDeckParent;

    [Header("デッキジェネレーター")] public DeckGeneratorFromDatas deckGeneratorFromDatas;

    private void Start()
    {
        //1つでもデッキがあれば1番目のデッキが選択されている扱いにする
        if(slotGenerator.GetDeckBoxImages().Count > 0)
        {
            SelectedDeck(slotGenerator.GetDeckBoxImages()[0]);
        }
        else
        {
            rightInfoUI.gameObject.SetActive(false);
        }
        
    }

    private void Update()
    {
        float cKey = Input.GetAxisRaw("CKey");
        if(cKey > 0)
        {
            
        }

        float dKey = Input.GetAxisRaw("DKey");
        if(dKey > 0)
        {
            
        }

        float vKey = Input.GetAxisRaw("VKey");
        if(vKey > 0)
        {
            
        }

        //float iKey = Input.GetAxisRaw("IKey");
        //if (iKey > 0)
        //{
        //    slotGenerator.RemoveDeckImages();
        //    GameManager.instance.userData.deckBoxDatas = new List<DeckBoxData>();
        //}
    }

    public override void DisplayScreen()
    {
        base.DisplayScreen();
        slotGenerator.EnumerateDecks();
    }

    public override void HiddenScreen()
    {
        //リスト画面を閉じる時生成されたデッキイメージを全て削除する
        slotGenerator.RemoveDeckImages();
        //このメソッドを呼び出すと非アクティブになってしまうため最後に呼び出す
        base.HiddenScreen();
    }

    /// <summary>
    /// 新しいデッキを作成する(デッキリストに追加するのは決まっていないが、とりあえず新規の編集画面に移行する。+ref
    /// </summary>
    public void CreateNewDeck()
    {
        GameManager.instance.TransitionDeckEditor(null);
    }

    /// <summary>
    /// テキストデータをもとにデッキを生成する+ref
    /// </summary>
    public void CopyTextValueDeck()
    {
        DeckBoxData generatedDeckBoxData = deckGeneratorFromDatas.CopyTextValueDeck(deckValueInputField.text);

        //DeckBoxData generatedDeckBoxData = Instantiate(deckBoxDataPrefab);
        //string[] cardIds = deckValueInputField.text.Split('/');

        //generatedDeckBoxData.deckName = $"コピーデッキ({deckValueInputField.text})";
        //for (int i=0;i< cardIds.Length;i++)
        //{
        //    string cardIdText = cardIds[i];
        //    int cardId = int.Parse(cardIdText);
        //    Card_InDeckBox generatedDeckCard = Instantiate(card_InDeckBoxPrefab);
        //    generatedDeckCard.cardData = GameManager.instance.cardDataBase.cardDatas[cardId];
        //    generatedDeckCard.GetComponent<Image>().sprite = generatedDeckCard.cardData.sprite;

        //    //デッキエディター内の場所を設定
        //    int columns = 10;
        //    int rowsNum = (i / columns);
        //    int width = 25;
        //    generatedDeckCard.positionInDeckBox = new Vector2(-210 + i * width - rowsNum * width * columns, 240 - rowsNum * 50);
        //    if(generatedDeckCard.cardData.belongDeckType == CardData.BelongDeckType.MainDeck)
        //    {
        //        generatedDeckBoxData.cards.Add(generatedDeckCard);
        //    }
        //    else
        //    {
        //        generatedDeckBoxData.specialCards.Add(generatedDeckCard);
        //    }
            
        //    generatedDeckCard.transform.SetParent(generatedDeckBoxData.transform);
        //}
        DeckBoxData deckAddedToUserData = GameManager.instance.userData.AddNewDeck(generatedDeckBoxData);
        slotGenerator.EnumerateDecks();

        //なんか謎にうまくいかない←何が？
        SelectedDeck(slotGenerator.GetDeckBoxImages()[GameManager.instance.userData.deckBoxDatas.IndexOf(deckAddedToUserData)]);
    }

    /// <summary>
    /// テキストデータをもとにデッキを生成する+ref 現在デッキデータ生成部分を共有した機能を作ったが、元々これがそうだったっぽいが、念のため放置しておく
    /// </summary>
    [ContextMenu("CopyTextValueDeckEditor")]
    public void CopyTextValueDeckEditor()
    {
        DeckBoxData generatedDeckBoxData = Instantiate(deckBoxDataPrefab);
        generatedDeckBoxData.transform.SetParent(editorGeneratedDeckParent);
        string[] cardIds = editorDeckId.Split('/');

        generatedDeckBoxData.deckName = "CopiedDeck(" + editorDeckId + ")";
        for (int i = 0; i < cardIds.Length; i++)
        {
            string cardIdText = cardIds[i];
            int cardId = int.Parse(cardIdText);
            Card_InDeckBox generatedDeckCard = Instantiate(card_InDeckBoxPrefab);
            generatedDeckCard.cardData = editorGameManager.cardDataBase.cardDatas[cardId];
            generatedDeckCard.GetComponent<Image>().sprite = generatedDeckCard.cardData.sprite;

            //デッキエディター内の場所を設定
            int columns = 17;
            int rowsNum = (i / columns);
            int width = 25;
            generatedDeckCard.positionInDeckBox = new Vector2(-210 + i * width - rowsNum * width * columns, 240 - rowsNum * 50);
            if (generatedDeckCard.cardData.belongDeckType == CardData.BelongDeckType.MainDeck)
            {
                generatedDeckBoxData.cards.Add(generatedDeckCard);
            }
            else
            {
                generatedDeckBoxData.specialCards.Add(generatedDeckCard);
            }

            generatedDeckCard.transform.SetParent(generatedDeckBoxData.transform);
        }
        DeckBoxData deckAddedToUserData = editorGameManager.userData.AddNewDeck(generatedDeckBoxData);
        //slotGenerator.EnumerateDecks();

        //なんか謎にうまくいかない
        //SelectedDeck(slotGenerator.GetDeckBoxImages()[GameManager.instance.userData.deckBoxDatas.IndexOf(deckAddedToUserData)]);
    }

    /// <summary>
    /// 設定されたデータをエディタ上でデッキに保存する　※現在、保存処理がうまくいっていない
    /// </summary>
    [ContextMenu("CopyTextValueDeckEditor2(うまくいってない)")]
    public void CopyTextValueDeckEditor2()
    {
        DeckBoxData generatedDeckBoxData = editorDeckBox;
        DeckBoxData deckAddedToUserData = editorGameManager.userData.AddNewDeck(generatedDeckBoxData);
        //slotGenerator.EnumerateDecks();

        //なんか謎にうまくいかない←というかエディタ内でセレクトする必要なくね
        //SelectedDeck(slotGenerator.GetDeckBoxImages()[GameManager.instance.userData.deckBoxDatas.IndexOf(deckAddedToUserData)]);

    }

    /// <summary>
    /// 情報を表示するデッキが選択される
    /// </summary>
    /// <param name="deckBoxImage"></param>
    public void SelectedDeck(DeckBoxImage deckBoxImage)
    {
        //無選択でUIが表示されていなかったとき、UIを表示する
        if (!selectedDeck) rightInfoUI.gameObject.SetActive(true);

        selectedDeck = deckBoxImage;
        selectedDeckFrame.transform.position = deckBoxImage.transform.position;
        deckNameInfoText.text = deckBoxImage.deckBoxData.deckName;
    }

    /// <summary>
    /// 選ばれたデッキを編集する+ref
    /// </summary>
    public void EditDeck()
    {
        selectedDeck.EditDeck();
    }

    /// <summary>
    /// 選ばれたデッキのIDをコピーする+ref
    /// </summary>
    public void CopyDeckId()
    {
        string deckDataValue = new DeckIdGenerator().GenerateDeckId(selectedDeck.deckBoxData);
        //imp:旧デッキID生成システム。新版の動作に問題がないことが確認できれば削除してよい
        //List<Card_InDeckBox> deckCards = selectedDeck.deckBoxData.cards;

        //for (int i = 0; i < deckCards.Count; i++)
        //{
        //    Card_InDeckBox card_InDeckBox = deckCards[i];
        //    CardData cardData = card_InDeckBox.cardData;//これ
        //    int cardId = GameManager.instance.cardDataBase.cardDatas.IndexOf(cardData);
        //    deckDataValue += cardId;
        //    //最後のみ/をつけない
        //    if (i < deckCards.Count - 1)
        //    {
        //        deckDataValue += "/";
        //    }
        //}

        //List<Card_InDeckBox> specialDeckCards = selectedDeck.deckBoxData.specialCards;
        //if (specialDeckCards.Count > 0)
        //{
        //    deckDataValue += "/";
        //}
        //for (int i = 0; i < specialDeckCards.Count; i++)
        //{
        //    Card_InDeckBox card_InDeckBox = specialDeckCards[i];
        //    CardData cardData = card_InDeckBox.cardData;//これ
        //    int cardId = GameManager.instance.cardDataBase.cardDatas.IndexOf(cardData);
        //    deckDataValue += cardId;
        //    //最後のみ/をつけない
        //    if (i < specialDeckCards.Count - 1)
        //    {
        //        deckDataValue += "/";
        //    }
        //}
        GUIUtility.systemCopyBuffer = deckDataValue;

        copiedDeckNotice.StartFade();
        Debug.Log("デッキデータをクリップボードにコピーしました。テキスト:" + deckDataValue);
    }

    /// <summary>
    /// デッキを削除する+ref
    /// </summary>
    public void RemoveDeck()
    {
        if (slotGenerator.GetDeckBoxImages().IndexOf(selectedDeck) >= 1)
        {
            SelectedDeck(slotGenerator.GetDeckBoxImages()[slotGenerator.GetDeckBoxImages().IndexOf(selectedDeck) - 1]);
        }
        else if (slotGenerator.GetDeckBoxImages().Count > 0)
        {
            SelectedDeck(slotGenerator.GetDeckBoxImages()[0]);
        }
        GameManager.instance.userData.RemoveDeck(selectedDeck.deckBoxData);
        slotGenerator.GetDeckBoxImages().Remove(selectedDeck);
        Destroy(selectedDeck.gameObject);
        slotGenerator.ReloadList();
        //Debug.Log("この後？？コードが問題ではないのか？");
    }

    /// <summary>
    /// バトルデッキに登録する+ref
    /// </summary>
    public void RegisterBattleDeck()
    {
        GameManager.instance.userData.battleDeck = selectedDeck.deckBoxData;
        registeredBattleDeckNotice.StartFade();
    }

    public void RegisterCPUDeck()
    {
        GameManager.instance.cpuMatchPlayerData.deckData = selectedDeck.deckBoxData;
        registeredCPUDeckNotice.StartFade();
    }
}
