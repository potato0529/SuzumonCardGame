using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_InDeckBox : MonoBehaviour, ICardData
{
    [Header("カードデータ")] public CardData cardData;

    private bool inListArea;
    private bool overlapOtherCard;
    private string deckBoxAreaTag = "DeckBoxArea";
    private string inDBcardTag = "Card_InDeckBox";
    private string listAreaTag = "DeckEditor_CardListArea";
    private bool canSetCardArea;
    [HideInInspector] public bool isGrabbed;
    [HideInInspector] public bool viewed;
    public ListedCard sourceListedCard;
    [Header("所属するデッキ")] public DeckBoxData belongDeck;
    [Header("デッキ内の座標")] public Vector2 positionInDeckBox; //デッキボックス上の座標

    void Update()
    {
        overlapOtherCard = false;

        if (isGrabbed)
        {
            RaycastHit[] hits = Physics.BoxCastAll(transform.position - new Vector3(0, 1.25f, 0), new Vector3(transform.localScale.x, 2, transform.localScale.z) * 0.5f, Vector3.down, Quaternion.identity, 3);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.tag == deckBoxAreaTag)
                {
                    if (hit.transform.position.z > 0)
                        inListArea = true;
                }
                //自身以外の、親を持たないカード(カードの下に重なっていない)にhitしている ※もしCardの上にさらに親が追加される場合は変更必須
                if (hit.collider.tag == inDBcardTag && hit.collider.gameObject != gameObject && !hit.collider.gameObject.transform.parent)
                {
                    overlapOtherCard = true;
                }
            }
        }

        if (inListArea && !overlapOtherCard)
        {
            canSetCardArea = true;
        }
        else
        {
            canSetCardArea = false;
        }
    }

    /// <summary>
    /// カードがセット可能な場所にドラッグされているか
    /// </summary>
    /// <returns></returns>
    public bool GetCanSetCardArea()
    {
        return !inListArea;
    }

    public void PointerDowned()
    {
        DeckEditorManager.instance.dragger.GrabCard(this);
        DeckEditorManager.instance.cardInfoCanvas.gameObject.SetActive(true);
        DeckEditorManager.instance.cardInfoCanvas.ShowCardInfo(cardData);
    }

    /// <summary>
    /// 設置可能な状態でドロップされたとき
    /// </summary>
    public void ReleasedGrab()
    {
        if (!belongDeck)
        {
            belongDeck = DeckEditorManager.instance.editingBox;

            Debug.Log($"{belongDeck} に所属します");

            if (cardData.belongDeckType == CardData.BelongDeckType.MainDeck)
            {
                belongDeck.cards.Add(this);
                DeckEditorManager.instance.ChangeCardCounter(this, 1);
            }
            else if (cardData.belongDeckType == CardData.BelongDeckType.SpecialDeck)
            {
                belongDeck.specialCards.Add(this);
            }

            transform.SetParent(belongDeck.transform);
        }
        positionInDeckBox = GetComponent<RectTransform>().anchoredPosition;
    }

    /// <summary>
    /// デッキボックス外でドロップされ、リストに戻される。(デッキボックスから除外される)
    /// </summary>
    public void ReturnedToList()
    {
        //af:検索中に検索対象に含まれないカードを戻すとエラーが起きるので一時的にこの処理を置いていますが、最終的にはデッキエディタのカード表示の内部処理の仕組みを変え、エラーが出る問題を解決します。それができたらこれは消しましょう
        if(sourceListedCard)
            sourceListedCard.RemainingNum++;
        if (belongDeck)
        {
            if(cardData.belongDeckType == CardData.BelongDeckType.MainDeck)
            {
                belongDeck.cards.Remove(this);
                DeckEditorManager.instance.ChangeCardCounter(this, -1);
            }
            else if(cardData.belongDeckType == CardData.BelongDeckType.SpecialDeck)
            {
                belongDeck.specialCards.Remove(this);
            }
            //Destroyされるからbelongデッキをリセットしなくていい
        }
          
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == listAreaTag)
        {
            inListArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == listAreaTag)
        {
            inListArea = false;
        }
    }

    #region interface
    public CardData GetCardData()
    {
        return cardData;
    }

    public void SetCardData(CardData thisCardData)
    {
        cardData = thisCardData;
    }
    #endregion
}
