
public class AntiqueBurst
{
    public EffectNugget invokeEffectNugget;
    public Effect antiqueBurstEffect;
    public int burstTime;//効果に書かれているアンティークバースト初期値
    public int antiqueCount;//アンティークバーストの残りターン数
    public Player startedTurnPlayer;

    /// <summary>
    /// 両チーム毎のアクションフェイズ開始時に加算
    /// </summary>
    public byte goesOnTurn;

    //色々効果の情報知っていてほしいからeffectとcard丸々受け取るようにしたけど密結合だし結局要らなくなったら値とか直接受け取るのでもよい
    public AntiqueBurst(Effect effect,Card userCard,Player thisStartedTurnPlayer)
    {
        burstTime = effect.mainValue.GetValue(userCard);
        antiqueCount = burstTime;
        invokeEffectNugget = effect.referenceEffectNugget;
        antiqueBurstEffect = effect;
        startedTurnPlayer = thisStartedTurnPlayer;
        MatchManager.instance.antiqueBurstList.StartAntiqueBurst(this);
        //場に残らないといけないかの処理
        if (effect.expandProperties && effect.expandProperties.GetComponent<Antique_ExProperties>().invokeOnlyField)
        {
            userCard.leftFieldCallback.Add(FailedAntiqueBurst);
        }
    }

    /// <summary>
    /// カードが場を離れ、アンティークバーストが失敗する
    /// </summary>
    public void FailedAntiqueBurst()
    {
        TriggerEffectManager.instance.RemoveAntiqueQueue(this);
    }

    /// <summary>
    /// アンティークバーストが完了したとき、場を離れた時のコールバックを解除する
    /// </summary>
    public void RemoveAntiqueBurst()
    {
        if (antiqueBurstEffect.expandProperties && antiqueBurstEffect.expandProperties.GetComponent<Antique_ExProperties>().invokeOnlyField)
        {
            invokeEffectNugget.card.leftFieldCallback.Remove(FailedAntiqueBurst);
        }
    }
}
