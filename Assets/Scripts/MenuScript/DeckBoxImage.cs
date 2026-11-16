using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// デッキリストに表示されるデッキUIに関するデータとそれ自体。基本UserDataなどでは運用せず、UserDataのDeckBoxDataから名前やデッキ内容のデータを取得する。もし今後デッキの装飾など、データが追加で必要になったらUserDataにそれだけを保管するための変数を作る
/// </summary>
public class DeckBoxImage : MonoBehaviour
{
    [Header("デッキデータ")] public DeckBoxData deckBoxData;
    [Header("デッキ名テキスト")] public TMP_Text deckNameText;

    /// <summary>
    /// このデッキを編集する
    /// </summary>
    public void EditDeck()
    {
        Debug.Log("編集するデッキ" + deckBoxData.cards[0].cardData);
        GameManager.instance.TransitionDeckEditor(deckBoxData);
    }

    /// <summary>
    /// デッキリストで選択される
    /// </summary>
    public void SelectedDeck()
    {
        MenuManager.instance.menuTransition.activityScreens[1].GetComponent<DeckListScreen>().SelectedDeck(this);
    }
}
