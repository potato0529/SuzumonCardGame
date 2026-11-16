using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 立場としてはユーザーデータとあまり変わらないけどマッチに必要なデータだけ分けてあるだけ
/// </summary>
public class MatchPlayerData : MonoBehaviourPunCallbacks
{
    [Header("ユーザーネーム")] public string userName;
    [Header("使用デッキ")] public DeckBoxData deckData;
    [Header("メインデッキカード")] public List<CardData> deckCards = new List<CardData>();
    [Header("スペシャルデッキカード")] public List<CardData> specialDeckCards = new List<CardData>();
    [Header("勝利数")] public int winCount;
    [Header("敗北数")] public int loseCount;
    [Header("カスタム先攻")] public bool customFirst;
    [Header("カスタムライフ")] public int customLife;

    public string deckId;

    /// <summary>
    /// 渡されたDeckBoxDataからメインデッキとスペシャルデッキ両方の中にあるカードデータを抽出してMatchPlayerDataに登録する　af:そもそもデッキ内のカードデータが何故かそれぞれCard_InDeckBox単位で登録されてなきゃこんな変換作業は不要であるため、今後その構造を改めるべき
    /// </summary>
    /// <param name="deckBoxData">このMatchPlayerDataに登録するデッキ</param>
    public void RegisterDeckData(DeckBoxData deckBoxData)
    {
        deckId = "";
        for (int loops = 0; loops < deckBoxData.cards.Count; loops++)
        {
            CardData cardData = deckBoxData.cards[loops].cardData;
            deckCards.Add(cardData);
            deckId += GameManager.instance.cardDataBase.cardDatas.IndexOf(cardData) + "/";
        }
        for (int loops = 0; loops < deckBoxData.specialCards.Count; loops++)
        {
            CardData cardData = deckBoxData.specialCards[loops].cardData;
            specialDeckCards.Add(deckBoxData.specialCards[loops].cardData);
            deckId += GameManager.instance.cardDataBase.cardDatas.IndexOf(cardData) + "/";

        }
    }

    /// <summary>
    /// Photonで送られてきた相手プレイヤーの情報を取得する
    /// </summary>
    /// <param name="changedPlayer"></param>
    /// <param name="customProps"></param>
    public void GetMatchPlayerDatas(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable customProps)
    {
        string name = (customProps["playerName"] is string value) ? value : null;
        userName = name;

        int[] mainDeckCardIds = (customProps["mainDeckCardIds"] is int[] value2) ? value2 : new int[0];
        foreach (int cardId in mainDeckCardIds)
        {
            deckCards.Add(GameManager.instance.cardDataBase.cardDatas[cardId]);
            deckId += cardId + "/";
        }

        int[] spDeckCardIds = (customProps["spDeckCardIds"] is int[] value3) ? value3 : new int[0];
        foreach (int cardId in spDeckCardIds)
        {
            specialDeckCards.Add(GameManager.instance.cardDataBase.cardDatas[cardId]);
            deckId += cardId + "/";
        }

        customFirst = (customProps["customFirst"] is bool value4) ? value4: false;

        int customLifee = (customProps["customLife"] is int value5) ? value5 : 0;
        customLife = customLifee;
    }
}
