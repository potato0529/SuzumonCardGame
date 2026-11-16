using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SPSelect : MonoBehaviour
{
    [Header("プレイヤー")] public Player player;
    [Header("ライフブロッカー音")] public AudioSource lifeBlockerSound;

    private bool doGetNoroi;
    private bool decideDoGetNoroi;
    [HideInInspector] public Attack attacker; //ライフブロッカー用の変数。現在アタック処理を実行しているカード

    public enum ProcessType
    {
        none,
        noroi,
        lifeBlocker,
    }

    public ProcessType processType = ProcessType.none;

    public void DoLifeBlockYes()
    {
        attacker.OpponentLifeBlockYes();
        Debug.Log("ライフブロッカーをすると決定しました");
        //ライフブロッカー可能なモンスターを取得
        List<Card> canLifeblockMonsters = player.fieldCards.cards.ToList();
        for(int i=canLifeblockMonsters.Count - 1;i>=0;i--)
        {
            Card card = canLifeblockMonsters[i];
            if (card.monsterScr && card.lifeBlockerEffect && card.lifeBlockerEffect.lifeBlockerValue >= 1 && !card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved))//今後ExistLifeBlockerと条件を共有したい
            {

            }
            else
            {
                canLifeblockMonsters.Remove(card);
            }
        }
        player.cardPointer.SelectCard(1,false, canLifeblockMonsters,  CardPointer.SelectType.none, PassTargetsToLifeBlocker,"ライフブロッカー");
    }

    public void DoLifeBlockNo()
    {
        attacker.OpponentLifeBlockNo();
        Debug.Log("ライフブロックをしませんでした");
    }

    public void ReceiveNoroiYes()
    {
        decideDoGetNoroi = true;
        doGetNoroi = true;
    }

    public void ReceiveNoroiNo()
    {
        decideDoGetNoroi = true;
        doGetNoroi = false;
    }

    /// <summary>
    /// ライフブロッカーをするか決める
    /// </summary>
    /// <returns></returns>
    public IEnumerator DecideDoLifeBlock(Card attacker)
    {
        Debug.Log("ライフブロックするか決めています");
        player.yon.ChooseStart(DoLifeBlockYes,DoLifeBlockNo,attacker.cardData.cardName + "のライフへの攻撃をブロックしますか？",false,true);

        //StartCoroutine(player.playerCanvas.DisplayGuide("ライフブロッカーするか決めてください。",0));
        StartCoroutine(player.opponentPlayer.playerCanvas.DisplayGuide("相手がライフブロッカーをするか決めています...",0));
        yield return null;
    }

    /*呪いがリワークされて不要になった処理
    public void Noroware(Card target)
    {
        processType = ProcessType.noroi;
        StartCoroutine(ChoiceDoGetNoroi(target));
    }

    IEnumerator ChoiceDoGetNoroi(Card target)
    {

        Debug.Log("呪いを受け入れるか");

        //StartCoroutine(player.playerCanvas.DisplayGuide("呪いを受けるか決めてください。", 0));
        StartCoroutine(player.opponentPlayer.playerCanvas.DisplayGuide("相手が呪いを受けるか決めています...", 0));

        player.yon.ChooseStart(ReceiveNoroiYes,ReceiveNoroiNo, target.cardData.cardName + "にかけられる呪いを受けますか？",false);;
        while (!decideDoGetNoroi)
        {
            yield return null;
        }

        player.opponentPlayer.playerCanvas.HideGuide();

        if (doGetNoroi)
        {
            Debug.Log("呪いを受けると決定しました");
            player.noroiNum++;
        }
        else
        {
            Debug.Log("対象のモンスターを呪殺しました");
            target.ChangeCardState(Card.CardState.Cemetery);
        }
        decideDoGetNoroi = false;
    }
    */
    
    /// <summary>
    /// 呪いの影響で墓地に送らなくてはならないカードを決める
    /// </summary>
    /// <param name="selectNum">受けている呪いの数</param>
    /// <returns></returns>
    public IEnumerator ChooseNoroiSuffer(int selectNum)
    {
        //コールバックをこのオブジェクトに指定して選ばれたカードをPassTargetsで受け取って墓地に送るようにする
        List<Card> noroiSufferTargets = player.handScr.hands;
        player.cardPointer.SelectCard(selectNum,false, noroiSufferTargets,  CardPointer.SelectType.noroi, PassTargetsToNoroi,"呪いで捨てる手札");
        yield return null;
    }

    //とりあえず呪いの影響の場合のみ　別の用途でもこのクラスで使う場合は要検討
    public void PassTargetsToNoroi(List<Card> targets)
    {
        foreach (Card target in targets)
        {
            target.ChangeCardState(Card.CardState.Cemetery);
        }        
    }

    public void PassTargetsToLifeBlocker(List<Card> targets)
    {
        LifeBlocker lifeBlocker = targets[0].effectNuggets[((byte)EffectNugget.InvokeTiming.LifeBlock)].GetComponent<LifeBlocker>();
        lifeBlocker.lifeBlockerValue--;
        if (lifeBlocker.lifeBlockerValue <= 0)
        {
            targets[0].BeDestroyed();
        }

        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.AttackWhole);//Attack内ではAttackWholeをいつ終わらせればよいかわからず、ここがライフブロッカーに分岐したときの処理の終着点なため、ここで処理の終了を宣言する。

        targets[0].ComeInvokeTiming(EffectNugget.InvokeTiming.LifeBlocked);
        lifeBlockerSound.PlayOneShot(lifeBlockerSound.clip);
        Debug.Log("ライフブロッカー！");
    }
}
