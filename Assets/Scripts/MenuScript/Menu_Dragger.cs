using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Dragger : MonoBehaviour
{
    [Header("カメラ")] public Camera cameraScr;
    [Header("キャンバス")] public Canvas canvas;

    private Card_InDeckBox pointerOverCard;
    private Card_InDeckBox grabbingCard;
    private RectTransform grabbingCardRt;
    private RectTransform grabbingCardParentRt;
    private bool isOverlapOtherCard;
    private Image cardImage;


    private void Update()
    {
        if (grabbingCard)
        {
            if (isOverlapOtherCard)
            {
                
            }
            else
            {
                Vector2 MousePos = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    grabbingCardParentRt,
                    Input.mousePosition,
                    null,
                    out MousePos);
                grabbingCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    MousePos.x,
                    MousePos.y);

                if (grabbingCard.GetCanSetCardArea()) //移動可能な場所にマウスがあるとき
                {
                    cardImage.color = Color.white;
                }
                else
                {
                    cardImage.color = Color.red;
                }

                if (Input.GetMouseButtonUp(0)) //マウスを離したとき
                {
                    Debug.Log("ドロップしました");
                    cardImage.color = Color.white; //カードの色を元に戻す
                    grabbingCard.isGrabbed = false; //カードを掴まれていない状態とする
                    //設置可能な場合
                    if (grabbingCard.GetCanSetCardArea())
                    {
                        grabbingCard.ReleasedGrab();
                    }
                    //設置不可能な場合
                    else
                    {
                        grabbingCard.ReturnedToList();
                    }
                    grabbingCard = null;
                }
            }
        }
    }   

    public void GrabCard(Card_InDeckBox card_InDeckBox)
    {
        Debug.Log($"{card_InDeckBox} を掴みます");
        grabbingCard = card_InDeckBox;
        cardImage = card_InDeckBox.GetComponent<Image>();
        grabbingCardRt = grabbingCard.GetComponent<RectTransform>();
        grabbingCardParentRt = grabbingCard.transform.parent.gameObject.GetComponent<RectTransform>();        
    }
}
