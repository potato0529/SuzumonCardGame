using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// ゲームからCPUへの入力要求を受け取り、適切なStrategyを使って入力を生成するクラス。具体的には、CpuAgentはゲームから、Cpuとして何か行動してほしいという大きい単位での要請を受け取り、Cpuが行動する流れの最初から最後までを管理する。行動内容の詳細はCpuStrategyが受け持つ。
/// </summary>
public class CpuAgent
{
    private readonly CancellationTokenSource cts = new CancellationTokenSource();

    private CpuStrategy cpuStrategy;

    /// <summary>
    /// Cpuの自分アクションフェイズ全体の行動の流れを実行する
    /// </summary>
    /// <param name="cpuPlayer"></param>
    /// <returns></returns>
    public async UniTask CpuFlow(Player cpuPlayer)
    {
        Debug.Log("CPUフローを開始します");

        while (cpuStrategy.HasAvailableCommands(cpuPlayer))
        {
            while(MatchManager.instance.runningProcess.Count > 0)
            {
                Debug.Log("CPUフローを待機中");
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            await UniTask.WaitUntil(() => MatchManager.instance.runningProcess.Count == 0);

            PlayerCommand decidedCommand = cpuStrategy.DecideCommand(cpuPlayer);
            if (decidedCommand != null)
            {
                MatchManager.instance.matchActionService.Submit(decidedCommand);

                //進行中の処理が完全に終わってから、ディレイをかけて次の命令を開始する
                await UniTask.WaitUntil(() => MatchManager.instance.runningProcess.Count == 0);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

                //0.5秒待った時に処理中で、処理が重複してしまう場合があったのでここで最終チェック。正直あまり綺麗じゃない処理であり、af:今後runningProcessが0になったときに次のUpdate処理などを待たずに間を埋めず次の処理を実行するようにする、あるいはrunningProcessの枠をとりうる処理を開始する場合、全て統一したラインで処理を開始し、そのときそれぞれの処理の中で優先順位を規定して実行するようにする。そうすれば、一度runningProcessが0になったことを確認できれば0.5秒待ったりしたとしても、手動でなにか操作しない限り新しく処理は開始しないようにできる
                await UniTask.WaitUntil(() => MatchManager.instance.runningProcess.Count == 0);

            }
        }

        MatchManager.instance.matchActionService.Submit(new PhaseButtonCommand(cpuPlayer));
    }

    /// <summary>
    /// 与えられたカードを選択する
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="selectType"></param>
    public void SelectCardOrder(Player cpuPlayer,List<Card> cards,CardPointer.SelectType selectType)
    {
        Debug.Log($"{cards.Count} 枚のカードの中からCPUがカードを選択します");
        MatchManager.instance.matchActionService.Submit(new SelectCardCommand(cpuPlayer,cpuStrategy.SelectCardId(cards, selectType),cpuPlayer.cardPointer.nowSelectingId));
        cpuPlayer.cardPointer.SelectedOneTargetCP();//warn:SelectedCard()内のSelectedOneTargetCP()がコメントアウトされている部分のコメント参照
    }

    /// <summary>
    /// Cpuの戦略を設定する
    /// </summary>
    /// <param name="_cpuStrategy"></param>
    public void SetStrategy(CpuStrategy _cpuStrategy)
    {
        cpuStrategy = _cpuStrategy;
    }
    
    /// <summary>
    /// YesNoどちらかを決める。ちゃんとCPUを作る際は判断材料などが引数で必要になるかも
    /// </summary>
    public async UniTask DecideYesNo(Player cpuPlayer,bool doSendCommand)
    {

        //本来doSendCommandなど詳細の情報を運ぶ必要があるのは構造的にあまりよくない。YesNoを近いうちに分離するので、そのときdoSendCommandという概念は消え、そのときそれぞれのYesNoでCPUはどう対応すればよいかの設計を考えなければならない
        if (doSendCommand)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            MatchManager.instance.matchActionService.Submit(new YesNoCommand(cpuPlayer, cpuStrategy.DecideYesNo(cpuPlayer)));
        }
        else
        {
            cpuPlayer.yon.yes.DecidedCP();
        }
    }

    public async UniTask DecideEffectChoice(Player cpuPlayer,List<AEffectChoice> effectChoices)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        MatchManager.instance.matchActionService.Submit(new ChoseEffectCommand(cpuPlayer, cpuPlayer.chooseEffect.generatedChoices.IndexOf(cpuStrategy.DecideEffectChoice(effectChoices))));
    }

    /// <summary>
    /// 効果使用を行う
    /// </summary>
    /// <param name="cpuPlayer"></param>
    /// <param name="cards"></param>
    public void EffectUseOrder(Player cpuPlayer, List<Card> cards)
    {
        Card useCard = MatchManager.instance.GetGameCards()[cpuStrategy.SelectUseCardId(cards)];

        //af:現在カードをドラッグして使用する系の処理は完全にはコマンド化できていないので一旦直接SetCardするが、その問題が解決したらSubmit処理にする
        cpuPlayer.cardPointer.SetCardTemp(useCard,cpuPlayer.fieldCardPlacement.Placed(useCard));
    }
}
