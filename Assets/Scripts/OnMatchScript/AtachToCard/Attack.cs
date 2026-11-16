using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("アタック可能回数")] public int turnCanAttackNum = 1; //1ターンにアタックできる回数
    [Header("アタックステータス")] public AttackStatus attackStatus = new AttackStatus();
    [Header("アタックコスト")] public EffectNugget attackCost;
    [Header("アタック条件")] public Condition attackCondition;
    [Header("アタック可能なモンスター条件")] public TargetCardConditionsNugget conditionsNugget;
    [Header("アタックアニメーションプレハブ")] public GameObject attackAnimPrefab;
    [Header("アタックパーティクル")] public ParticleSystem attackChargeParticle;

    public Monster monster;
    [HideInInspector] public Player player;
    private Card card;
    
    [Header("現在のアタックダメージ")] public int attackDamage;

    private bool cardPointer;
    private Card attackTargetCard;
    private SelectableTarget attackTarget;


    // Start is called before the first frame update
    void Start()
    {
        card = GetComponent<Card>();
        player = card.ownerPlayer;
        monster = GetComponent<Monster>();
        //cardPointer = cardScr.ownerPlayer.cardPointer;
        attackDamage = attackStatus.StatusValue;
        attackCost = card.effectNuggets[((byte)EffectNugget.InvokeTiming.AttackCost)];
        card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Attack);
        
    }

    /// <summary>
    /// アタックできるかどうか調べる
    /// </summary>
    /// <returns></returns>
    public bool CanAttack()
    {
        if ((turnCanAttackNum - monster.turnAttackedNum > 0 && monster.turnDefencedNum == 0) && monster.CanCommand())
        {
            if (attackCondition.FulfillConditions(card))
            {
                if (attackCost && attackCost.CanUseCheck())
                {
                    UtilityLog.CardLog("アタックコストを払うことができます。アタッカー情報:", card, UtilityLog.LogCategory.none);
                    return true;
                }
                else if (!attackCost)
                {
                    UtilityLog.CardLog("アタックコストはありません。アタッカー情報:", card, UtilityLog.LogCategory.none);
                    return true;
                }
                else
                {
                    UtilityLog.CardLog("アタックコストを払うことができません。アタッカー情報:", card, UtilityLog.LogCategory.none);
                    return false;
                }
            }
            else
            {
                UtilityLog.CardLog("カードの効果によるアタック条件を満たしていません。アタッカー情報:", card, UtilityLog.LogCategory.none);
                return false;
            }
        }
        else
        {
            UtilityLog.CardLog($"アタック条件を満たしていません。 アタック権:{turnCanAttackNum - monster.turnAttackedNum > 0} 自分のターン:{MatchManager.instance.turnPlayer == player} 動ける:{!card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Paralysis)}", card, UtilityLog.LogCategory.none);
            return false;
        }
    }

    /// <summary>
    /// プレイヤーがアタックコマンドを行ったとき、アタックコストが必要か確認してそれに応じて処理を行う
    /// </summary>
    public void AttackCostCheck()
    {
        if (!attackCost)
        {
            SelectTarget();
            UtilityLog.CardLog("コストはありません。攻撃します", card, UtilityLog.LogCategory.none);
        }
        else
        {
            player.yon.ChooseStart(PayAttackCostYes, PayAttackCostNo,"アタックコストを払いますか？",false,false);
            UtilityLog.CardLog("アタックコストが必要です。払うか決めてください。", card, UtilityLog.LogCategory.none);
        }
    }

    /// <summary>
    /// アタック対象を選択する処理を開始する
    /// </summary>
    public void SelectTarget()
    {
        //コールバックをこのオブジェクトに指定し、選ばれたカードにアタックするようにする
        List<Card> selectableTargets = conditionsNugget.GetMeetConditionsCards(monster.card, (checkCard) => { return !(checkCard.monsterScr.barrier.GetRemainBarrier() || checkCard.monsterScr.notAttacked); });
        player.cardPointer.SelectAttackCard(this, 1, selectableTargets, PassAttackTarget);
    }

    /// <summary>
    /// CardPointerからターゲットを受け取る
    /// </summary>
    /// <param name="targets"></param>
    public void PassAttackTarget(List<SelectableTarget> targets)
    {
        StartCoroutine(SelectedAttackTarget(targets[0]));
    }

    /// <summary>
    /// アタック対象が選ばれたとき、その対象にアタックできるか確認し、アタックできる場合アタックの処理を呼び、アタックする前に何か選択処理が入る場合は待機する
    /// </summary>
    /// <param name="target"></param>
    public IEnumerator SelectedAttackTarget(SelectableTarget selectableTarget)
    {
        UtilityLog.CardLog("アタック対象が選ばれました。アタッカー情報:", card, UtilityLog.LogCategory.none);

        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.AttackWhole, new RunningProcess(player));

        attackTarget = selectableTarget;
        attackTargetCard = attackTarget.GetComponent<Card>();

        if (attackTargetCard != null)
            UtilityLog.CardLog("アタック対象が選ばれました。アタック対象情報:", attackTargetCard, UtilityLog.LogCategory.none);
        else
            Debug.Log("アタック対象が選ばれました。アタック対象:相手プレイヤー");

        MatchManager.instance.turnAttackedMonsters.Add(monster);
        MatchManager.instance.turnCommandedMonsters.Add(monster);

        card.ComeInvokeTiming(EffectNugget.InvokeTiming.BeforeAttack);
        while (!MatchManager.instance.CanProceedProcess())
        {
            yield return null;
        }

        monster.barrier.AttackAndDisableBarrier();
        monster.AddAttackCount();
        card.controlerPlayer.playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.Attacked,1);
        //MatchManager.instance.turnAttackedMonsters.Add(monster); なんかturnAttackedMonsterに追加する処理2重にあるんだけどなぜだ

        if (attackTargetCard)
        {
            attackTargetCard.selectableTarget.targeted = false;

            attackTargetCard.ComeInvokeTiming(EffectNugget.InvokeTiming.BeforeBeAttacked);
            while (!MatchManager.instance.CanProceedProcess())
            {
                yield return null;
            }

            card.ComeInvokeTiming(EffectNugget.InvokeTiming.BeforeAttackMonster);
            while (!MatchManager.instance.CanProceedProcess())
            {
                yield return null;
            }
            
            AttackAnim(attackTarget);
        }
        //ターゲットがnullの場合プレイヤーとみなす（プレイヤーを選択した場合CardPointerのselectedTargetsに何も入らない為）
        else
        {
            //相手の場にライフブロッカーが存在しなければプレイヤーへのアタックが確定し、そうでない場合相手がライフブロッカーをするか選択するまで待機する
            if (!player.opponentPlayer.fieldCards.ExistLifeBlocker())
            {
                Debug.Log("ライフブロッカーはいません");
                AttackAnim(attackTarget);
            }
            else
            {
                yield return null;
                Debug.Log(player.opponentPlayer.gameObject + "ライフブロッカーがいます");
                player.opponentPlayer.spSelect.processType = SPSelect.ProcessType.lifeBlocker;
                player.opponentPlayer.spSelect.attacker = this;
                StartCoroutine(player.opponentPlayer.spSelect.DecideDoLifeBlock(card)); //warn:色々改変したせいで今ライフブロッカー動かんかも（多分インスペクター直せば行ける）ライフブロッカーをしたタイミングでウェポンが効果を発動するのはTiming.LifeBlockとはまた別のタイミングが必要っぽいのでまた後で
            }
        }
    }

    /// <summary>
    /// アタックアニメーションを開始する(続けてアタックの処理もする) ※条件に応じてキャンセルされる可能性もある。もしこれを実行したときに確実にアタックが確定する保証が欲しくなったら仕組みを変えよう。
    /// </summary>
    /// <param name="target"></param>
    private void AttackAnim(SelectableTarget target)
    {
        //アタック開始時場にいなかった場合キャンセルされる。af:この判定ってこの場所でいいのか？
        if (card.cardState != Card.CardState.Field || card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Paralysis) || card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved) || (attackTargetCard && attackTargetCard.cardState != Card.CardState.Field))
        {
            EndAttack();
            return;
        }
        MatchManager.instance.SetRunningProcess(MatchManager.RunningProcessType.Attacking, new RunningProcess(player));
        
        GameObject attackAnimObj = Instantiate(attackAnimPrefab);
        attackAnimObj.transform.position = transform.position;
        ParticleSystem newParticle = Instantiate(attackChargeParticle);
        newParticle.transform.position = transform.position;
        newParticle.Play();
        Destroy(newParticle.gameObject, 5.0f);
        //直接攻撃か否かによって与えるオブジェクトを分ける
               
        StartCoroutine(attackAnimObj.GetComponent<AttackAnim>().StartAttackAnim(attackTarget, this));        
    }

    /// <summary>
    /// アタックが確定したときに呼ばれ、対象にアタックする ※効果起動後に何か処理をするならenumにしないとだめ
    /// </summary>
    public void StartAttack(SelectableTarget target)
    {
        Debug.Log(target + "にアタック！");

        (target.GetComponent(typeof(IAttacked)) as IAttacked).Attacked(this,attackDamage);

        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.Attacking);

        if (attackTargetCard)
        {
            //モンスターにアタックした後の効果
            card.ComeInvokeTiming(EffectNugget.InvokeTiming.AfterAttackMonster);
        }
        //ターゲットがnullの場合プレイヤーとみなす（プレイヤーを選択した場合CardPointerのselectedTargetsに何も入らない為）
        else
        {
            //プレイヤーにアタックした後の効果
            card.ComeInvokeTiming(EffectNugget.InvokeTiming.AfterAttackPlayer);
        }
        //アタックした後の効果
        card.ComeInvokeTiming(EffectNugget.InvokeTiming.AfterAttack);
       
        UtilityLog.CardLog("アタックしました", card, UtilityLog.LogCategory.none);
        EndAttack();
    }

    /// <summary>
    /// アタック処理が終了する
    /// </summary>
    public void EndAttack()
    {
        MatchManager.instance.UpdatedGameStatus();
        MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.AttackWhole);
        UtilityLog.CardLog("アタック処理が終了しました", card, UtilityLog.LogCategory.none);
    }

    /// <summary>
    /// 自分の攻撃に対して相手がライフブロックをし、攻撃が中断される
    /// </summary>
    public void OpponentLifeBlockYes()
    {
        UtilityLog.CardLog("アタックできません。相手がライフブロッカーを使用するそうです。アタッカー情報:", card, UtilityLog.LogCategory.none);
        StartCoroutine(card.controlerPlayer.playerCanvas.DisplayGuide("相手はライフブロッカーを実行し、このアタックは無効になります。",2));
    }

    /// <summary>
    /// 相手がライフブロックをしないと決め、攻撃が続行される
    /// </summary>
    public void OpponentLifeBlockNo()
    {
        UtilityLog.CardLog("相手がライフブロックを行わないと決定したので、アタックを続行します。アタッカー情報:", card, UtilityLog.LogCategory.none);
        StartCoroutine(card.controlerPlayer.playerCanvas.DisplayGuide("相手はライフブロッカーを行いませんでした。",2));
        //アタックが確定する
        AttackAnim(attackTarget);
    }

    /// <summary>
    /// アタックコストを支払うと決める
    /// </summary>
    public void PayAttackCostYes()
    {
        Debug.Log("(YES)コストを払うと決めました");
        attackCost.UseEffectOrder(CompAttackCostProcess);
    }

    /// <summary>
    /// コストを払い終わったら呼ばれ、アタック処理を開始する
    /// </summary>
    public void CompAttackCostProcess()
    {
        Debug.Log("コストが完了しました");
        SelectTarget();
    }

    /// <summary>
    /// アタックコストを支払わず、攻撃をしないと決める
    /// </summary>
    public void PayAttackCostNo()
    {
        Debug.Log("(NO)コストを払わず、攻撃を中止しました");
    }

    public int GetStatusValue()
    {
        return attackDamage;
    }
}
