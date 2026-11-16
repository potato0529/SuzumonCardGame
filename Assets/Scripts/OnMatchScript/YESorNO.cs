using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class YESorNO : MonoBehaviour,IOwnerPlayer
{
    [Header("画面の主")] public Player pScr;
    [Header("YES")] public YES yes;
    [Header("NO")] public NO no;
 
    public bool active;
    private string choosingText;
    public bool ifNoIsChosenDontRPC;
    public bool doSendCommandMode;//通信が必要なコマンド扱いのYesNoかどうかのモード。一度の選択で毎回リセットされなければならない。UI上の確認でしかなくてコマンドとして認識する必要がない、コスト付きで使用するかどうかの確認などで用いる。UI上の確認でしかないYesNoとライフブロッカーなどのコマンドとして残すべきYesNoは本来別物として扱うべきなので、いずれこの機能は廃止し別のクラスを作る予定

    void Start()
    {
        SetActiveYN(false);
    }

    /// <summary>
    /// YESかNOか選択する処理を開始する
    /// </summary>
    /// <param name="decideYes">決定がYESだったときに実行する関数</param>
    /// <param name="decideNo">決定がNOだったときに実行する関数</param>
    public void ChooseStart(Action decideYes, Action decideNo,string text,bool dontRPCIfNoIsChosen,bool doSendCommand)
    {
        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.YesNo,new RunningProcess(pScr));
        active = true;
        SetActiveYN(true);
        yes.callback = decideYes;
        no.callback = decideNo;
        choosingText = text;
        //pScr.playerCanvas.guideText.gameObject.SetActive(true);
        StartCoroutine(pScr.playerCanvas.DisplayGuide(text, 0));
        ifNoIsChosenDontRPC = dontRPCIfNoIsChosen;
        doSendCommandMode = doSendCommand;

        //必ずしもPlayer(だけ)に返すわけじゃなくなったり疎結合にしたくなったらコールバックとかにした方がいいかも
        pScr.OnYesNoStarted(doSendCommand);

        Debug.Log("yesnoスタート！");
    }

    /// <summary>
    /// YESorNO処理を終了
    /// </summary>
    public void ChooseEnd()
    {
        SetActiveYN(false);
        pScr.playerCanvas.HideGuide();
        active = false;
        doSendCommandMode = true;

        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.YesNo);
    }

    /// <summary>
    /// YES,NOオブジェクトのアクティブ状況を設定
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveYN(bool isActive)
    {
        yes.gameObject.SetActive(isActive);
        no.gameObject.SetActive(isActive);
    }

    public Player GetPlayer()
    {
        return pScr;
    }
}
