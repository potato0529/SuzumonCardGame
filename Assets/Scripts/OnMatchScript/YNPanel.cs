using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class YNPanel : MonoBehaviour
{
    [Header("YESNOコントローラー")] public YESorNO yesNo;

    public Action callback;

    //YESorNOボタンがクリックされたとき直接呼ばれるトリガー。実際のローカルと通信の共通処理はDecidedCP()で行うのでこのメソッドは分離されており、この中に実際の処理を書いてはならない
    public virtual void OnClicked()
    {
        //MatchManager.instance.matchActionService.Submit(new YesNoCommand(yesNo.pScr,));
    }

    /// <summary>
    /// クリックされたときに呼ばれ、押されたボタンがそのボタンの処理をする(yesならtrueを返す処理)
    /// </summary>
    public virtual void DecidedCP()
    {
        DecideChoice();
        yesNo.ChooseEnd();
    }

    /// <summary>
    /// クリックされたときの固有処理
    /// </summary>
    protected virtual void DecideChoice()
    {

    }
}
