using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour,IPointerOver
{
    protected Image image;
    protected Card card;

    public bool isPointerOver { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        Close(false);
    }

    public void Open(Card thisCard,bool doAnim)
    {
        image.enabled = true;
        card = thisCard;

        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI,"メニューオープン時の確認");
        //ボタンが押せるかどうか確認
        UpdateButtonState();
        if(doAnim)
            StartCoroutine(OpenAnim());
    }

    public IEnumerator OpenAnim()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        for(int i = 0;i<10;i++)
        {
            image.color += new Color(0, 0, 0, 0.1f);
            yield return null;
        }
    }

    public void Close(bool doAnim)
    {
        image.enabled = false;
        card = null;
        if(doAnim)
            StartCoroutine(CloseAnim());
    }

    public IEnumerator CloseAnim()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        for (int i = 0; i < 10; i++)
        {
            image.color -= new Color(0, 0, 0, 0.1f);
            yield return null;
        }
    }

    public virtual void Clicked()
    {
        card.CloseButton();
        card.ownerPlayer.playerCanvas.cardInfoMenu.CloseMenu(card);
    }

    /// <summary>
    /// そのボタンが使用可能（押して処理をする条件を満たしているか）
    /// </summary>
    /// <returns></returns>
    public virtual bool EnableButton()
    {
        return true;
    }

    /// <summary>
    /// ボタンが使用可能か確認し、UIに反映する
    /// </summary>
    public void UpdateButtonState()
    {
        if (EnableButton())
        {
            image.color = new Color(1, 1, 1, 1);
        }
        else
        {
            image.color = new Color(0.65f, 0.55f, 0.5f, 1);
        }
    }
}
