using System.Windows.Input;
using UnityEngine;

public class MatchActionService
{
    private readonly IMatchTransmitter transmitter;

    public readonly CommandExecutor executor; //原則として、オンライン通信時のCommand処理のタイミングのズレを埋めるためのNetworkPlayerからのExecute命令(タイミングのズレを埋めるだけで、他の処理はなにも挟まない規則があるので実質MatchActionServicdから実行しているようなものであり、疑似的なprivate)を除いて、他クラスからここにアクセスしてはならない。

    private HistoryRecorder recorder;

    public MatchActionService(IMatchTransmitter transmitter,HistoryRecorder historyRecorder)
    {
        this.transmitter = transmitter;

        executor = new CommandExecutor();
        recorder = historyRecorder;
    }


    /// <summary>
    /// ローカル側からコマンドの実行を提出する（ここから実際のコマンドの全体処理と通信を行う。受信側はこれは呼ばれず、実際の処理部分の共通処理のみ呼ばれる）
    /// </summary>
    /// <param name="command"></param>
    public void Submit(PlayerCommand command)
    {
        Debug.Log($"コマンド {command} がSubmitされました");

        Dispatch(command);

        transmitter.Send(command);
    }

    /// <summary>
    /// ローカル側も受信側も共通のコマンドが発生したとき、それをローカルと受信側両方に反映させるための処理。（例えばローカルがカードを仕様した場合、ローカル側のみがこれを呼ぶ。そして、ここからローカル側ではExecutorで実際のコマンドをゲームに反映させる処理とオンラインにコマンドを送信する処理を行う。
    /// </summary>
    /// <param name="command"></param>
    public void Dispatch(PlayerCommand command)
    {
        executor.Execute(command);

        recorder.Record(command);
    }

    /// <summary>
    /// af:UseCardNormalCommandが現在唯一Dispatchを通れないので応急処置でこれを設置。ちゃんと近いうちにUseCardNormalCommandを完全に正式なCommandにし、早急にこのメソッドを削除すること。
    /// </summary>
    /// <param name="command"></param>
    public void TempRecord(PlayerCommand command)
    {
        recorder.Record(command);
    }
}
