using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 使う効果を選ぶ処理の選択肢のひとつ
/// </summary>
public class AEffectChoice : MonoBehaviour
{
    [Header("RectTransform")] public RectTransform rt;
    [Header("選択肢ボタンテキスト")] public Text choiceButtonText;

    [HideInInspector] public ChooseEffect chooseEffect;
    private EffectNugget invokeEffectNugget;
    public bool canChosen;
    public Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// 選択肢の番号をセットする
    /// </summary>
    /// <param name="numOfChoice"></param>
    public void SetNumOfChoice(int numOfChoice,EffectNugget thisEffectNugget)
    {
        choiceButtonText.text = numOfChoice + "";
        invokeEffectNugget = thisEffectNugget;
    }

    /// <summary>
    /// 選択肢のボタンがクリックされたときに直接呼ばれるラッパー部分※ここではコマンドのSubmitのみをし、ここには選択されたときの実際のシステム処理を書いてはならず、ローカルと通信共通の実際の処理はChoceThisChoiceに書かなければならない
    /// </summary>
    public void Clicked()
    {
        Debug.Log($"選択肢がクリックされました {gameObject.name}");
        if (canChosen)
        {
            MatchManager.instance.matchActionService.Submit(new ChoseEffectCommand(chooseEffect.chooserPlayer, chooseEffect.generatedChoices.IndexOf(this)));

            //imp:旧通信/実行処理
            ////選択通信
            //if (chooseEffect.chooserPlayer == MatchManager.instance.localPlayer)
            //{
            //    Debug.Log(chooseEffect.generatedChoices.IndexOf(this));
            //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentChoseEffect), Photon.Pun.RpcTarget.Others, chooseEffect.generatedChoices.IndexOf(this));
            //}

            //chooseEffect.EndChoose();
            //EffectManager.instance.EndEffect();
            //invokeEffectNugget.AddTriggerQueue();
        }
        else
        {
            Debug.Log("無効な選択肢です");
        }
    }

    /// <summary>
    /// ローカル側も受信側も呼ばれる、選択肢が決定されたときの共通処理
    /// </summary>
    public  void ChoseThisChoice()
    {
        chooseEffect.EndChoose();
        EffectManager.instance.EndEffect();
        invokeEffectNugget.AddTriggerQueue();
    }
}
