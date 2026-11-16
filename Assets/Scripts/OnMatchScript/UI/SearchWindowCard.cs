using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchWindowCard : ListedUICard
{
    public SearchCard searchCard;
    [Header("元のカード")] public Card sourceCard;
    [Header("イメージ")] public Image image;
    [Header("チェックマーク")] public Image checkMark;
    public bool isSelected;

    /// <summary>
    /// このウィンドウ上のこのカードがクリックされたときの入り口処理（+1)
    /// </summary>
    public void Clicked()
    {
        //ここは純粋なUIに対するレスポンス処理のみで、実際の処理はSelected()に含まれる。通信時に呼ばれる部分を分離するためにこのようにしている。
        MatchManager.instance.matchActionService.Submit(new SelectSearchCardCommand(searchCard.pScr,searchCard.inSearchWindowSCards.IndexOf(this),searchCard.searchingId));
    }

    /// <summary>
    /// ウィンドウ上のこのカードが選択された時の処理
    /// </summary>
    public void Selected()
    {
        if (!isSelected)
        {
            DisplaySelectedMark();

            if (!searchCard.isSummonMode)
            {
                UtilityLog.CardLog("サーチウィンドウのカードが選択されました", sourceCard);
                searchCard.SelectedCard(this);
                //imp:旧通信処理
                //if (searchCard.pScr == MatchManager.instance.localPlayer)
                //{
                //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentSelectedSearchCard), Photon.Pun.RpcTarget.Others, searchCard.inSearchWindowSCards.IndexOf(this), searchCard.searchingId);
                //    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Network, $"サーチウィンドウ内ID:{searchCard.inSearchWindowSCards.IndexOf(this)}が選ばれた情報を送ります");
                //}
            }
        }
    }

    /// <summary>
    /// SummonModeかそうでないかに限らず、サーチウィンドウ中でクリックされると選択済み状態にするという処理
    /// </summary>
    public void DisplaySelectedMark()
    {
        checkMark.enabled = true;
        image.color = new Color(100, 100, 100);
        isSelected = true;
    }

    public void PointerDown()
    {
        //サーチウィンドウモード(効果召喚)であれば問題なく、そうでなければ使用権の確認をする※ちょっときもいかもしれんからSearchCardにフィールドを作って判断する可能性あり
        if (searchCard.isSummonMode && !isSelected && (searchCard.pScr.cardPointer.searchWindowMode || searchCard.pScr.IsRemainUseRights(sourceCard)))
        {
            searchCard.pScr.cardPointer.CatchCard(sourceCard);
            sourceCard.SetCardRotation(new Vector3(90, searchCard.firstCardPos.transform.eulerAngles.y, 0));
            searchCard.CloseWindow();
        }
    }

    /// <summary>
    /// このカードの上にマウスカーソルが乗ったときの処理 ref+1
    /// </summary>
    public void PointerOver()
    {
        //sourceCard.controlerPlayer.playerCanvas.personalUICanvas.miniCardInfoCanvas.ShowCardInfo(sourceCard); imp:この行の処理だとウィンドウのオブジェクトがActiveにならず、下の処理ならActiveにしつつ同様の処理をしてくれる。おそらく今までこの処理にしてた特別な意図は特にない可能性が高いが、もしこれが意味がある処理で、変えたら影響がある可能性がゼロではないので一応残しておく
        sourceCard.controlerPlayer.playerCanvas.personalUICanvas.ShowMiniCardInfo(sourceCard);
    }

}
