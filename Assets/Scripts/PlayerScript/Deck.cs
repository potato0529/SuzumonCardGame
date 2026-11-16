using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Deck : MonoBehaviour
{
    [Header("オーナープレイヤー")] public Player ownerPlayer;

    [SerializeField] private List<Card> cards = new List<Card>();

    [Header("開始時シャッフルしない（デバッグ用)")] public bool dontShuffle;
    [Header("開始時ドロー数")] public byte startDrawNum;

    [Header("デッキスロットのジェネレーター")] public CardSlotsGenerator cardSlotsGenerator;
    [Header("デッキを表示するキャンバス")] public Canvas deckCanvas;
    [Header("デッキに表示するカードプレハブ"), SerializeReference] public InMatchListedUICard inMatchListedUICardPrefab;

    public List<Card> Cards
    {
        get 
        {
            return cards; 
        }
    }

    private void Start()
    {
        //Awakeを起こすため
        deckCanvas.gameObject.SetActive(true);
        deckCanvas.gameObject.SetActive(false);

        foreach (Card card in cards)
        {
            InMatchListedUICard inMatchListedUICardInMyCanvas = cardSlotsGenerator.GenerateListedCard(inMatchListedUICardPrefab, card);
            inMatchListedUICardInMyCanvas.sourceCard = card;
        }

        cardSlotsGenerator.SortArrange();
        StartCoroutine(DelayDraw());
    }

    // Start is called before the first frame update
    public void DeckInit()
    {
        //デッキをシャッフル
        if(!dontShuffle)
            Shuffle();
        ArrangementDeck();
    }

    /// <summary>
    /// 時間差でドローする
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayDraw()
    {
        yield return new WaitForSeconds(1.0f);
        //初期6枚ドロー
        for (int drawNum = 0; drawNum < startDrawNum; drawNum++)
        {
            yield return new WaitForSeconds(0.05f);
            ownerPlayer.handScr.Draw();
        }
    }

    /// <summary>
    /// デッキのリストをランダムにシャッフルする
    /// </summary>
    public void Shuffle()
    {
        for (int bottom = cards.Count - 1; bottom > 0; bottom--)
        {
            int random = MatchManager.instance.duelRandom.Range(0, bottom + 1);
            Card temp = cards[bottom];
            cards[bottom] = cards[random];
            cards[random] = temp;
        }

        UtilityLog.CategorizedLog(UtilityLog.LogCategory.OtherSystem, "シャッフルしました");
    }

    /// <summary>
    /// デッキのカードの座標をカードリストの順番に従って変更して並べる 
    /// </summary>
    public void ArrangementDeck()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.OtherSystem, "デッキをリスト順にソートしました");
        float y = 0.1f;
        for(int deckNum = cards.Count - 1;deckNum >= 0; deckNum--)
        {
            y += cards[0].body.transform.lossyScale.y;

            //transform関連の設定
            cards[deckNum].transform.position = transform.position + new Vector3(0, y, 0);
            cards[deckNum].SetCardRotation(new Vector3(270,180,0));

            cards[deckNum].cardState = Card.CardState.Deck;
        }
    }

    /// <summary>
    /// デッキにカードを追加する
    /// </summary>
    /// <param name="card"></param>
    /// <param name="deckNum">デッキ番号(100にするとランダムとなる)(負の数の場合、下からその絶対値番目となる) ※番号はテキストの日本語準拠</param>
    public void AddDeck(Card card,int deckNum)
    {
        //100を入れるとランダムに
        if(deckNum == 100)
        {
            Debug.Log($"{card} を ランダムにデッキに加えます");
            cards.Insert(MatchManager.instance.duelRandom.Range(0, cards.Count - 1), card);
        }
        //下からの時と上からの時で処理少し変わるっぽい
        else if(Math.Abs(deckNum) <= cards.Count + 1)
        {
            //正の数だった場合、デッキ上から数える
            if(deckNum >= 0)
            {
                Debug.Log($"{card} を デッキ上から {deckNum} 番目に置きます");
                cards.Insert(deckNum - 1, card);
            }
            //負の数だった場合、デッキ下から数える
            else
            {
                Debug.Log($"{card} を デッキ下から {deckNum * -1} 番目に置きます");
                cards.Insert(cards.Count + 1 - deckNum * -1, card);
            }            
        }
        else
        {
            //上からn番目の指定で、nがデッキ枚数を超えている場合、デッキトップに置く。
            if(deckNum >= 0)
            {
                Debug.Log($"{deckNum} はデッキの枚数を超えるので {card} をデッキトップに置きます");
                cards.Insert(0,card);
            }
            //下からn番目の指定で、nがデッキ枚数を超えている場合、デッキボトムに置く。
            else
            {
                Debug.Log($"{deckNum * -1} はデッキの枚数を超えるので {card} をデッキボトムに置きます");
                cards.Add(card);
            }
        }
        //デッキに戻るアニメーションほしかったらコルーチンにするなりして頑張れ
        card.moveToPos.StopMoveToVector();
        ArrangementDeck();
        InMatchListedUICard inMatchListedUICardInMyCanvas = cardSlotsGenerator.GenerateListedCard(inMatchListedUICardPrefab, card);
        inMatchListedUICardInMyCanvas.sourceCard = card;

        cardSlotsGenerator.SortArrange();
    }

    public void LeaveDeck(Card card)
    {
        int thisIndex = cards.IndexOf(card);
        cards.Remove(card);
        cardSlotsGenerator.RemoveListedCard(thisIndex);
        cardSlotsGenerator.SortArrange();
    }

    public void OpenDeckWindow()
    {
        deckCanvas.gameObject.SetActive(true);

    }
}
