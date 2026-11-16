using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("手札の持ち主のプレイヤー(スクリプト)")] public Player pScr;
    [Header("デッキ")] public Deck deck;
    [Header("ソーシャルディスタンス")] public float cardDistance;
    [Header("カードの厚さ")] public float cardThickness;

    public List<Card> hands = new List<Card>();

    private int handsBottomNum;
    private Vector3 firstHandPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && pScr == MatchManager.instance.localPlayer)
        {
            //imp:旧通信＋ローカル実行システム
            //Draw();

            //if(pScr == MatchManager.instance.localPlayer)
            //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.SpaceDraw), Photon.Pun.RpcTarget.Others);

            MatchManager.instance.matchActionService.Submit(new SpaceDrawCommand(pScr));
        }
    }



    public Card Draw()
    {
        Card drawnCard = null;

        //デッキトップを手札に加える
        if (deck.Cards.Count > 0)
        {
            drawnCard = deck.Cards[0];
            deck.Cards[0].AddedToHand();
            UtilityLog.CardLog("ドロー！！", drawnCard, UtilityLog.LogCategory.Whole);
        }
        else
        {
            Debug.Log(pScr + "のデッキがもうありません");
        }

        return drawnCard;
    }

    /// <summary>
    /// カードを手札に加える。(もともといた場所から離れる処理はないため基本Card.ChangeCardState()から呼び出し、他の場所からは呼び出さない。)
    /// </summary>
    /// <param name="card"></param>
    public void AddHand(Card card)
    {
        //カードを手札に加える
        hands.Add(card);
        //デッキからそのカードを取り除く
        //手札の一番後ろ(引いたカード)インデックス番号を設定
        handsBottomNum = hands.Count - 1;
        //手札に加えたカードの状態を手札にある状態とする
        hands[handsBottomNum].cardState = Card.CardState.Hand;
        SortHands();
        if (!card.registeredGM)
        {
            MatchManager.instance.handFieldCemeteryCards.Add(card);
            card.registeredGM = true;
        }
    }

    /// <summary>
    /// 手札を整理する。※移動させたはずのカードが手札に戻る場合完全に手札のリストから取り除いてから使用！
    /// </summary>
    public void SortHands()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.OtherSystem, "SortHandsが呼ばれました");
        //1枚目の手札の座標を設定
        firstHandPos = transform.position + new Vector3(-(hands.Count * cardDistance) / 2, 0, 0);
        
        for (int loops = 0; loops < hands.Count; loops++)
        {
            //カードがドラッグ中だった場合、例外としてスキップする
            if (hands[loops].standbySet)
            {
                continue;
            }
            Card tCardScr = hands[loops].GetComponent<Card>();
            tCardScr.moveToPos.Move(firstHandPos + new Vector3(loops * cardDistance, cardThickness * loops, 0), 5);

            //相手の手札は裏側に
            if (pScr.isLocalPlayer)
            {
                tCardScr.SetCardRotation(new Vector3(90, 0, 0));
            }
            else
            {
                tCardScr.SetCardRotation(new Vector3(270, 0, 0));
            }            
        }
    }

    /// <summary>
    /// 1つの手札を正しい場所に戻す
    /// </summary>
    /// <param name="card"></param>
    public void SortAHand(Card card)
    {
        int handIndex = hands.IndexOf(card);
        card.transform.position = transform.position + new Vector3(-(hands.Count * cardDistance) / 2, 0, 0) + new Vector3(handIndex * cardDistance, cardThickness * handIndex, 0);
    }

    /// <summary>
    /// 使用可能カードをハイライトする
    /// </summary>
    public void HighlightAvailableHand()
    {
        foreach(Card handCard in hands)
        {
            if (handCard.CanNormalUseCheck())
            {
                handCard.matchCardHighlight.HighlightBorderEnable(true);
                if (handCard.monsterScr)
                {
                    handCard.matchCardHighlight.ChangeHighlightBorderColor(new Color(1, 0.6f, 0, 1));
                }
                else
                {
                    handCard.matchCardHighlight.ChangeHighlightBorderColor(new Color(0.3f, 1, 0, 1));
                }
            }
        }
    }

    /// <summary>
    /// 使用可能カードのハイライトを消す
    /// </summary>
    public void RemoveHighlights()
    {
        foreach(Card card in hands)
        {
            card.matchCardHighlight.HighlightBorderEnable(false);
        }
    }

    /// <summary>
    /// 手札の中の使用可能カードを返す
    /// </summary>
    /// <returns>手札の使用可能なカード</returns>
    public List<Card> GetAvailableHandCard()
    {
        List<Card> canUseCards = new List<Card>();
        foreach (Card hand in hands)
        {
            if (hand.CanNormalUseCheck())
            {
                canUseCards.Add(hand);
            }
        }
        return canUseCards;
    }
}
