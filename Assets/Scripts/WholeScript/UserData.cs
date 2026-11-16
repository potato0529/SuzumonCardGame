using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserData : MonoBehaviour
{
    [Header("ユーザーランク")] public int userRank;
    [Header("経験値")] public int exp;
    [SerializeField,Header("所有デッキ")] public List<DeckBoxData> deckBoxDatas;
    [Header("所有カード")] public List<PossessionCard> possessionCards = new List<PossessionCard>();
    [HideInInspector] public List<CardData> possessionCardDatas = new List<CardData>();//possessionCardsとインデックス番号が同じになるようにする。af:本来possessionCardsをprivateにして、Addされたときに共通のルートを通るようにしてインデックス番号などの一致を担保するべきだが、そもそもPossessionCardsの仕組みを廃止する可能性があるので不適切な設計のまま保留

    [Header("スズペイ"),SerializeField] private int suzupay;
    [Header("カードポイント")] public int[] cardPoints = new int[4];
    [Header("マッチ用データ")] public MatchPlayerData matchPlayerData;
    public int Suzupay
    {
        get { return suzupay; }
        set { suzupay = value; }
    }
    [Header("所有カードプレハブ")] public PossessionCard possessionCardPrefab;
    [Header("所有カードの親(見やすくするための空のオブジェクト)")] public Transform prosessionCardsParent;
    [Header("全カードを所有カードとするか")] public bool doPossessAllCard;

    private int needExpToRankup = 300;
    private int expToRankup;
    public DeckBoxData battleDeck;//今後バトルデッキの選び方は変えるかもだから要らなくなる可能性アリ

    [Header("文化祭先攻")] public bool customFirst;
    [Header("カスタムライフ")] public int customLife;

    public UserExperience userExperience;

    private void Awake()
    {
        userExperience = new UserExperience();

        //今は存在するカードがすべて所有カードとする
        if (doPossessAllCard)
        {
            for (int i = 0; i < GameManager.instance.cardDataBase.cardDatas.Count; i++)
            {
                CardData cardData = GameManager.instance.cardDataBase.cardDatas[i];
                PossessionCard generatedPossessionCard = Instantiate(possessionCardPrefab);
                generatedPossessionCard.cardData = cardData;
                generatedPossessionCard.inPossessionNum = cardData.deckLimit;
                possessionCards.Add(generatedPossessionCard);
                generatedPossessionCard.transform.SetParent(prosessionCardsParent);
                cardData.cardID = i;
                possessionCardDatas.Add(cardData);//ここでしかpossessionCardDatasは追加されず、そもそもおかしい設計。同フィールドのコメントアウト参照。
            }
        }
    }

    /// <summary>
    /// 新しく作成したデッキを保存する。
    /// </summary>
    /// <returns>ユーザーデータに複製したデッキを返す</returns>
    public DeckBoxData AddNewDeck(DeckBoxData thisDeckBoxData)
    {
        DeckBoxData newDeck = Instantiate(thisDeckBoxData);
        deckBoxDatas.Add(newDeck);
        newDeck.transform.SetParent(this.transform);
        Debug.Log($"新しいデッキ {thisDeckBoxData} を保存しました！");

        ////使用デッキを指定するシステムを仮で作るのめんどいからとりあえず最新の保存したデッキを仮で使用する仕様にしとく 今はこれ要らん 代わりにMatchMakerにある
        ////今はとりまデバッグ楽だし先にcarddataとして抜き取るようにする
        //for (int loops = 0; loops < newDeck.cards.Count; loops++)
        //{
        //    Debug.Log($"{matchPlayerData.deckCards.Count},{newDeck.cards.Count},{loops}");
        //    //matchPlayerData.deckCards[loops] = newDeck.cards[loops].cardData; なぜ以前これでエラーが起きなかったのかわからない
        //    matchPlayerData.deckCards.Add(newDeck.cards[loops].cardData);
        //}
        //for (int loops = 0; loops < newDeck.specialCards.Count; loops++)
        //{
        //    //matchPlayerData.specialDeckCards[loops] = newDeck.specialCards[loops].cardData;
        //    matchPlayerData.specialDeckCards.Add(newDeck.specialCards[loops].cardData);
        //}

        return newDeck;
    }

    /// <summary>
    /// デッキを削除する
    /// </summary>
    /// <param name="thisDeckBoxData"></param>
    public void RemoveDeck(DeckBoxData thisDeckBoxData)
    {
        deckBoxDatas.Remove(thisDeckBoxData);
        Destroy(thisDeckBoxData.gameObject);
        Debug.Log($"デッキ {thisDeckBoxData} を削除しました！");
    }

    public List<DeckBoxData> GetDeckBoxDatas()
    {
        return deckBoxDatas;
    }

    public void GetCard(CardData cardData)
    {
        //ゲットしたカードが既に入手済みか確認する
        bool isObtained = false;
        foreach(PossessionCard prosessionCard in possessionCards)
        {
            if(cardData == prosessionCard.cardData)
            {
                isObtained = true;
                prosessionCard.inPossessionNum++;
                break;
            }
        }
        if (!isObtained)
        {
            PossessionCard generatedProsessionCard = Instantiate(possessionCardPrefab);
            generatedProsessionCard.cardData = cardData;
            generatedProsessionCard.inPossessionNum++;
            possessionCards.Add(generatedProsessionCard);
            generatedProsessionCard.transform.SetParent(prosessionCardsParent);
        }
    }

    /// <summary>
    /// 経験値を得る時の処理
    /// </summary>
    /// <param name="getPoint"></param>
    public void GetExp(int getPoint)
    {
        exp += getPoint;
        expToRankup += getPoint;
        while(expToRankup >= needExpToRankup)
        {
            userRank++;
            expToRankup -= needExpToRankup;
        }
    }

    /// <summary>
    /// 次のランクまでの必要経験値に対して現在の経験値の割合を取得する
    /// </summary>
    public float GetNeedExpPercentage()
    {
        return (float)expToRankup / (float)needExpToRankup;
    }
}
