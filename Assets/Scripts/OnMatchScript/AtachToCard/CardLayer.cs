using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardLayer : MonoBehaviour
{
    [Header("重なったカードリスト")] public List<Card> layer = new List<Card>();
    [Header("カード")] public Card card;
    
    /// <summary>
    /// 渡されたカードをこのカードの下に重ねる
    /// </summary>
    /// <param name="underCard">下に重ねるカード</param>
    public void PileUnderThis(Card underCard)
    {
        UtilityLog.CardLog(card.gameObject.name + "の下に" + underCard.gameObject.name + "を重ねます。上のカード:", card, UtilityLog.LogCategory.ChangeCardState);
        //ChangeCardStateを実行すると重ねたいカードの下のレイヤーが解除されてしまうので重なっていたカードを記憶する
        List<Card> underCardLayer = new List<Card>();
        if(underCard.cardLayer.layer.Count > 0)
        {
            underCardLayer = underCard.cardLayer.layer.ToList();
        }
        underCard.ChangeCardState(Card.CardState.Piled);//state処理→親変更処理　じゃないとダメ。変えたかったらChangeCardStateのもともと重ねられていた場合の処理の一部をどこかに移す必要あり
        underCard.transform.parent = gameObject.transform;
        layer.Add(underCard);
        foreach(Card underCardLayerCard in underCardLayer)
        {
            PileUnderThis(underCardLayerCard);
        }
        ArrangementLayer();
    }

    /// <summary>
    /// このカードの下のカードをレイヤーから外す(重なった状態を解除する)
    /// </summary>
    /// <param name="card">外すカード</param>
    public void ReleaseUnderCard(Card releaseCard)
    {
        UtilityLog.CardLog(releaseCard.gameObject.name + "を" + card.gameObject.name + "のレイヤーから外します。上のカード:", card, UtilityLog.LogCategory.ChangeCardState);
        releaseCard.transform.parent = null;
        card.cardLayer.layer.Remove(releaseCard);
        if (card.cardState == Card.CardState.Field)
        {
            if (releaseCard.controlerPlayer == MatchManager.instance.players[0])
            {
                //releaseCard.moveToPos.Move(new Vector3(0, 0.1f, -25), 1);
                releaseCard.transform.position = new Vector3(0, 0.1f, -25);
            }
            else if (releaseCard.controlerPlayer == MatchManager.instance.players[1])
            {
                //releaseCard.moveToPos.Move(new Vector3(0, 0.1f, 25), 1);
                releaseCard.transform.position = new Vector3(0, 0.1f, 25);
            }
        }
    }

    /// <summary>
    /// このカードが一番上だった場合、このカードに重なっていたカードのレイヤーを解除する
    /// </summary>
    /// <param name="piledCardsCardState">重なっていたカードが送られる場所</param>
    public void UnPileTopThis(Card.CardState piledCardsCardState)
    {
        for(int i=layer.Count - 1;i >= 0;i--)
        {
            Card layerCard = layer[i];
            layerCard.ChangeCardState(piledCardsCardState);
            layerCard.transform.parent = null;
        }
        layer = new List<Card>();
    }

    /// <summary>
    /// レイヤーを基準にカードのy座標を並べ替える
    /// </summary>
    public void ArrangementLayer()
    {
        Transform parent = transform;
        float cardHeight = card.body.transform.lossyScale.y;
        float y = cardHeight * (parent.childCount + 1);
        //親の座標をセットする
        card.transform.position += new Vector3(0, y, 0);
        for (int childrenNum = 0; childrenNum < parent.childCount; childrenNum++)
        {
            //カードの大きさ分y座標の差をとるようにする
            y -= cardHeight;
            Transform child = parent.GetChild(childrenNum);
            child.transform.position = new Vector3(parent.position.x + (childrenNum + 1), y, parent.position.z + (childrenNum + 1));
        }
        UtilityLog.CardLog("レイヤーをアレンジしました " + card.transform.position + "上のカード", card, UtilityLog.LogCategory.ChangeCardState);
    }
}
