using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 複数の効果からどれを実行するか選ぶ処理
/// </summary>
public class ChooseEffect : MonoBehaviour,IOwnerPlayer
{
    [Header("選択肢ボタンプレハブ")] public AEffectChoice choicePrefab;
    [Header("選択肢の間隔")] public float choicesDistance;
    [Header("選択を行うプレイヤー")] public Player chooserPlayer;

    public List<AEffectChoice> generatedChoices = new List<AEffectChoice>();

    public bool active;

    /// <summary>
    /// 効果選択処理が開始する
    /// </summary>
    /// <param name="effectNugggetChoices"></param>
    public void StartChoose(List<EffectNugget> effectNugggetChoices)
    {
        Debug.Log("効果選択開始！");
        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.ChooseEffect ,new RunningProcess(chooserPlayer));
        active = true;
        List<AEffectChoice> canUseChoices = new();

        //各選択肢をセット
        for(int loops = 0;effectNugggetChoices.Count > loops; loops++)
        {
            AEffectChoice generatedChoice = Instantiate(choicePrefab);
            generatedChoices.Add(generatedChoice);
            //選択肢の初期設定
            generatedChoice.chooseEffect = this;
            generatedChoice.transform.SetParent(chooserPlayer.canvas.transform);
            generatedChoice.rt.position = transform.position;
            generatedChoice.rt.position += new Vector3(choicesDistance * loops + 1, 0, 0);
            generatedChoice.SetNumOfChoice(loops + 1,effectNugggetChoices[loops]);
            bool canChoose = effectNugggetChoices[loops].CanUseCheck();
            Debug.Log($"選択肢有効チェック {loops},{canChoose}");
            generatedChoice.canChosen = canChoose;
            if (canChoose)
                canUseChoices.Add(generatedChoice);
            if (generatedChoice.canChosen)
            {
                generatedChoice.image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                generatedChoice.image.color = new Color(1, 1, 1, 0.5f);
            }
        }

        if(canUseChoices.Count == 1 && chooserPlayer == MatchManager.instance.localPlayer) //オンライン対戦において、もしこの処理を受信側が通る場合、送信側の選択を待つ必要がある（もし自動選択する状況であろうと、送信側から自動選択されたコマンドが送られてくる）のでこちらで勝手に選択処理を進めてはならいので、ローカルプレイヤーのときのみ自動選択をする。
        {
            Debug.Log("有効な選択肢が1つしかないため、自動で選択されます");
            canUseChoices[0].Clicked();
        }
        else
        {
            chooserPlayer.OnEffectChoosingStarted(canUseChoices);
        }
        
    }

    /// <summary>
    /// 選択が決定されて効果選択処理が終了する
    /// </summary>
    public void EndChoose()
    {
        Debug.Log("効果選択処理が終了しました");
        foreach(AEffectChoice effectChoice in generatedChoices)
        {
            Destroy(effectChoice.gameObject);
        }
        generatedChoices = new List<AEffectChoice>();

        active = false;
        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.ChooseEffect);
    }

    public Player GetPlayer()
    {
        return chooserPlayer;
    }
}
