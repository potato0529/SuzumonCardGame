using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseManager : MonoBehaviour
{
    [Header("フェイズ表示")] public Image[] nowPhaseImages;
    [Header("フェイズイメージ")] public Sprite[] phaseImages;

    public TurnPhases nowPhase = TurnPhases.waiting;
    public MatchManager matchManager;

    private bool endActionPhase;

    private bool endDetailProcess;

    public int turnNum;

    private bool oldNoroi; //旧呪い処理をONにする

    public enum TurnPhases
    {
        waiting,
        turnStart,
        draw,
        actionPhase,
        endPhase,
        turnChanging
    }

    /// <summary>
    /// ゲームが開始して最初のフェーズ処理
    /// </summary>
    public void FirstPhase()
    {
        nowPhase = TurnPhases.waiting;
        NextPhase();
    }

    /// <summary>
    /// 次のフェーズに進む
    /// </summary>
    public void NextPhase()
    {
        //従来までrunningProcess確認してたけどもし必要だったら戻そう
        matchManager.SetRunningProcess(MatchManager.RunningProcessType.PhaseProcess,null);
        nowPhase++;

        if(nowPhase > TurnPhases.turnChanging)
        {
            nowPhase = TurnPhases.turnStart;
        }

        StartCoroutine(PhaseAnim());
        
        PhaseProcess();
        
    }

    /// <summary>
    /// フェーズ開始時のアニメーションをする
    /// </summary>
    /// <returns></returns>
    private IEnumerator PhaseAnim()
    {
        foreach (Image phaseImage in nowPhaseImages)
        {
            if (phaseImages[(byte)nowPhase])
            {
                phaseImage.gameObject.SetActive(true);
                phaseImage.color = new Color(1, 1, 1, 0.25f);
                phaseImage.sprite = phaseImages[(byte)nowPhase];
            }
        }

        for(int i = 0; i < 15; i++)
        {
            nowPhaseImages[0].color += new Color(0, 0, 0, 0.05f);
            nowPhaseImages[1].color += new Color(0, 0, 0, 0.05f);
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < 15; i++)
        {
            nowPhaseImages[0].color -= new Color(0, 0, 0, 0.05f);
            nowPhaseImages[1].color -= new Color(0, 0, 0, 0.05f);
            yield return null;
        }

        foreach (Image phaseImage in nowPhaseImages)
        {
            phaseImage.gameObject.SetActive(false);
            phaseImage.color = new Color(1, 1, 1, 0.25f);
            phaseImage.sprite = null;
        }
    }

    /// <summary>
    /// フェーズ開始時の処理を実行(現在のフェーズ毎に異なる)
    /// </summary>
    private void PhaseProcess()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Whole, nowPhase + "の処理を実行します");

        switch (nowPhase)
        {
            case TurnPhases.waiting:
                PhaseEnd(TurnPhases.waiting);
                break;

            case TurnPhases.turnStart:
                StartCoroutine(TurnStart());
                break;

            case TurnPhases.draw:
                StartCoroutine(DrawPhase());                
                break;

            case TurnPhases.actionPhase:
                StartCoroutine(ActionPhase());
                break;

            case TurnPhases.endPhase:
                StartCoroutine(EndPhase());
                break;

            case TurnPhases.turnChanging:
                StartCoroutine(TurnChanging());
                break;
        }
    }

    /// <summary>
    /// 今のフェーズが終わり次のフェーズに進む
    /// </summary>
    private void PhaseEnd(TurnPhases turnPhase)
    {
        Debug.Log(turnPhase + "が終了しました");
        matchManager.CompRunningProcess(MatchManager.RunningProcessType.PhaseProcess);
        NextPhase();
    }

    #region 各フェーズ全体コルーチン

    public IEnumerator TurnStart()
    {
        Debug.Log("ターン開始");
        matchManager.turnPlayer.TurnStart();
        if(matchManager.turnPlayer == matchManager.players[0])
        {
            turnNum++;
            foreach(Player player in matchManager.players)
            {
                player.playerCanvas.turnNum.text = turnNum.ToString();
            }
        }
        yield return new WaitForSeconds(0.2f);
        PhaseEnd(TurnPhases.turnStart);
    }

    /// <summary>
    /// ドローフェーズの処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator DrawPhase()
    {
        yield return new WaitForSeconds(1);

        float drawnCount = 0;

        if (matchManager.turnPlayer.deck.Cards.Count > 0 && !IsFirstPlayersFirstTurn())
        {
            Card drawnCard = matchManager.turnPlayer.Draw();
            matchManager.turnPlayer.playerCanvas.personalUICanvas.ShowDrawCard(drawnCard);

            drawnCount += Time.deltaTime;
            while ((!Input.GetMouseButton(0) && drawnCount <= 1.5f) && matchManager.turnPlayer == matchManager.localPlayer)
            {
                yield return null;
            }

            matchManager.turnPlayer.playerCanvas.personalUICanvas.HideDrawCard();
        }
        else
        {
            Debug.Log($"{matchManager.turnPlayer} のデッキはもうありません");
        }

        PhaseEnd(TurnPhases.draw);
    }

    public void EndActionPhase()
    {
        Debug.Log("アクションフェイズを終了");
        endActionPhase = true;
    }
    /// <summary>
    /// アクションフェイズ開始時の処理
    /// </summary>
    /// <returns></returns>
    IEnumerator ActionPhase()
    {
        Debug.Log("アクションフェーズ開始時処理");
        if (oldNoroi)
        {
            StartCoroutine(NoroiProcess());
            while (matchManager.runningProcess.ContainsKey(MatchManager.RunningProcessType.EveryTurnNoroi))
            {
                yield return null;
            }
            Debug.Log("呪い処理終了");
        }

        endDetailProcess = false;
        matchManager.turnPlayer.StartActionPhase();
        while (!endDetailProcess)
        {
            yield return null;
        }

        TriggerEffectManager.instance.GoOnAntique();
        while (matchManager.runningProcess.ContainsKey(MatchManager.RunningProcessType.AntiqueBurst))
        {
            yield return null;
        }
        Debug.Log("アンティーク処理終了");
        

        matchManager.turnPlayer.playerCanvas.ShowTurnEndButton();

        matchManager.CompRunningProcess(MatchManager.RunningProcessType.PhaseProcess);

        matchManager.turnPlayer.StartActionPhaseAndCanCommand();

        while (!endActionPhase)
        {
            yield return null;
        }
        endActionPhase = false;
        matchManager.turnPlayer.playerCanvas.HideTurnEndButton();

        PhaseEnd(TurnPhases.actionPhase);
    }

    /// <summary>
    /// エンドフェイズの処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator EndPhase()
    {
        endDetailProcess = false;
        matchManager.turnPlayer.EndActionPhase();        
        while (!endDetailProcess)
        {
            yield return null;
        }

        StartCoroutine(TriggerEffectManager.instance.InvokeTET());

        while (matchManager.runningProcess.ContainsKey(MatchManager.RunningProcessType.TurnEndTriggerChecking))
        {
            yield return null;
        }

        foreach (Monster commandedMonster in matchManager.turnCommandedMonsters)
        {
            commandedMonster.turnAttackedNum = 0;
            commandedMonster.turnDefencedNum = 0;
            commandedMonster.turnCommandedNum = 0;
        }
        
        foreach (EffectNugget effectNugget in matchManager.turnUsedEffects)
        {
            effectNugget.turnUsedNum = 0;
        }

        

        yield return new WaitForSeconds(0.5f);

        PhaseEnd(TurnPhases.endPhase);
    }

    public IEnumerator TurnChanging()
    {
        matchManager.turnPlayer.TurnEnd();

        if (matchManager.turnPlayer == matchManager.players[0])
        {
            matchManager.turnPlayer = matchManager.players[1];
        }
        else if (matchManager.turnPlayer == matchManager.players[1])
        {
            matchManager.turnPlayer = matchManager.players[0];
        }
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Whole, matchManager.turnPlayer + "のターン");
        nowPhase = TurnPhases.waiting;
        //foreach (Text phaseText in phaseTexts)
        //    phaseText.text = matchManager.turnPlayer.gameObject.name + " " + "Turn";

        if (matchManager.turnPlayer == matchManager.players[0])
        {

        }
        else
        {

        }

        yield return new WaitForSeconds(0.7f);

        PhaseEnd(TurnPhases.turnChanging);
    }

    #endregion

    IEnumerator NoroiProcess()
    {
        Debug.Log("呪い処理開始");

        matchManager.SetRunningProcess(MatchManager.RunningProcessType.EveryTurnNoroi,null);

        if (matchManager.turnPlayer.noroiNum > 0)
        {
            StartCoroutine(matchManager.turnPlayer.spSelect.ChooseNoroiSuffer(matchManager.turnPlayer.noroiNum));
        }

        while (matchManager.runningProcess.ContainsKey(MatchManager.RunningProcessType.Select))
        {
            yield return null;
        }

        matchManager.CompRunningProcess(MatchManager.RunningProcessType.EveryTurnNoroi);                
    }

    public void EndDetailProcess()
    {
        endDetailProcess = true;
    }

    /// <summary>
    /// 先攻1ターン目であるか
    /// </summary>
    public bool IsFirstPlayersFirstTurn()
    {
        return matchManager.turnPlayer == MatchManager.instance.players[0] && turnNum == 1;
    }
}
