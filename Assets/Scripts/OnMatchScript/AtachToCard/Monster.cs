using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : CardTypeCommon
{
    [Header("バリア")] public Barrier barrier;
    [Header("アタックされない")] public bool notAttacked;
    [HideInInspector] public Attack attack;
    [Header("召喚サウンド")] public AudioSource summonSound;
    [Header("ディフェンスサウンド")] public AudioSource defenceSound;
    [Header("モンスターのアニメーション")] public MonsterAnimation monsterAnimation;


    private bool canAttack;

    [HideInInspector] public byte attackedNum;
    [HideInInspector] public byte turnAttackedNum; //アタックした回数

    [HideInInspector] public byte defencedNum;
    [HideInInspector] public byte turnDefencedNum; //アタックした回数

    [HideInInspector] public byte commandedNum;
    [HideInInspector] public byte turnCommandedNum;

    [HideInInspector] public int turnCanDefenceNum;

    private bool CanAttackMonster //アタック権があるかや、自分のターンであるかなどではなく、ルール上アタック可能なモンスターであるか
    {
        get
        {
            bool canAtk = true;
            if (attack.turnCanAttackNum <= 0 || (adventMonster && !adventMonster.Advented))
            {
                canAtk = false;
            }
            return canAtk;
        }

        set { canAttack = value; }
    }

    private bool CanDefenceMonster
    {
        get
        {
            bool canDef = true;
            if (turnCanDefenceNum <= 0 || (adventMonster && !adventMonster.Advented))
            {
                canDef = false;
            }
            return canDef;
        }

        set { canAttack = value; }
    }

    [HideInInspector] public AdventMonster adventMonster;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        attack = GetComponent<Attack>();
        adventMonster = GetComponent<AdventMonster>();
    }

    protected override void Start()
    {
        base.Start();

        
    }

    /// <summary>
    /// モンスターを召喚する
    /// </summary>
    public void Summon()
    {
        //召喚カウントに関する処理
        ownerPlayer.summonedNum++;
        ownerPlayer.turnSummonedNum++;
        ownerPlayer.playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.SummonedCount, 1);

        //フィールドに移動した状態の判定にする
        card.ChangeCardState(Card.CardState.Field);
        UtilityLog.CardLog(gameObject.name + "を召喚", card, UtilityLog.LogCategory.ChangeCardState);

        if (!card.registeredGM)
        {
            MatchManager.instance.handFieldCemeteryCards.Add(card);
            card.registeredGM = true;
        }

        //ライフブロッカー値の初期化
        if (card.lifeBlockerEffect)
        {
            card.lifeBlockerEffect.lifeBlockerValue = card.lifeBlockerEffect.defaultLifeBlockerValue.GetValue(card);
        }

        if (barrier.enableBarrier)
        {
            barrier.ExpandBarrier();
        }

        turnAttackedNum = 0;
        turnDefencedNum = 0;
        turnCommandedNum = 0;

        attack.attackStatus.BaseStatusValue = attack.attackStatus.DefaultStatusValue;

        if (adventMonster)
        {
            adventMonster.Advented = false;
        }

        summonSound.PlayOneShot(summonSound.clip);

        card.ComeInvokeTiming(EffectNugget.InvokeTiming.Appear);

        UtilityLog.CardLog("召喚処理完了！", card, UtilityLog.LogCategory.ChangeCardState);
    }

    public void AttackButton()
    {
        UtilityLog.CardLog("アタックボタン", card, UtilityLog.LogCategory.none);
        if (attack.CanAttack())
        {
            attack.AttackCostCheck();
            /*imp:旧通信処理
            if (attack.attackCost)
            {
                if (card.controlerPlayer == MatchManager.instance.localPlayer)
                    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentStartAttack), Photon.Pun.RpcTarget.Others, MatchManager.instance.GetGameCards().IndexOf(card));
            }
            else
            {
                if (card.controlerPlayer == MatchManager.instance.localPlayer)
                    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentStartAttack), Photon.Pun.RpcTarget.Others, MatchManager.instance.GetGameCards().IndexOf(card));
            }
            */
        }
    }

    public void DefenceButton()
    {
        UtilityLog.CardLog("ディフェンス処理開始", card, UtilityLog.LogCategory.none);
        if (CanDefence())
        {
            card.cardStatuses.SetStatusValue<Withstand>(StatusData.StatusType.WithStand,1,false,false);
            barrier.AttackAndDisableBarrier();
            AddDefenceCount();
            card.controlerPlayer.playerMatchLog.AddIntMatchData(PlayerMatchLog.IntMatchData.Defenced, 1);
            MatchManager.instance.turnDefencedMonsters.Add(this);
            MatchManager.instance.turnCommandedMonsters.Add(this);
            card.ComeInvokeTiming(EffectNugget.InvokeTiming.Defenced);
            MatchManager.instance.UpdatedGameStatus();
            //imp:旧通信処理
            //if (card.controlerPlayer == MatchManager.instance.localPlayer)
            //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentDefence), Photon.Pun.RpcTarget.Others, MatchManager.instance.GetGameCards().IndexOf(card));
            defenceSound.PlayOneShot(defenceSound.clip);
            monsterAnimation.PlayDefenceAnimation();
        }
    }

    /// <summary>
    /// アタック/ディフェンスを行えるかどうか返す。※コマンド不可の状態異常でなく、先行1ターン以外の自分ターンで処理中でないなど、あくまでアタック/ディフェンスをするための共通条件を取り出しているだけなので、アタック/ディフェンス権どちらかが残っているかなどは見ていないので、実際にはなにもコマンドできない場合がありうる。本当に、このメソッドはアタック/ディフェンスの共通条件を取り出しているだけということに注意
    /// </summary>
    /// <returns></returns>
    public bool CanCommand()
    {
        //warn:ボタンを表示するための条件で弾かれているので問題はないが、ここに降臨前はコマンドできないのもつけるべきではある？
        return (MatchManager.instance.turnPlayer == ownerPlayer && !card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Paralysis) && !card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved) && MatchManager.instance.runningProcess.Count == 0 && !MatchManager.instance.phaseManager.IsFirstPlayersFirstTurn() && (!adventMonster || adventMonster.Advented));        
    }

    public override void OpenMenu()
    {
        CardInfoMenu cardInfoMenu = ownerPlayer.playerCanvas.cardInfoMenu;
        List<CardButton> cardButtons = new List<CardButton>();

        //条件によって表示ボタンを変える
        if (CanAttackMonster)
        {
            cardButtons.Add(cardInfoMenu.attackButton);
        }
        if (CanDefenceMonster) 
        {
            cardButtons.Add(cardInfoMenu.defenceButton);
        }

        if((!adventMonster || adventMonster.Advented) && card.effectNuggets[(byte)EffectNugget.InvokeTiming.MyTurn]) //今後色々踏まえて効果が使える状態か否かで変える可能性
            cardButtons.Add(cardInfoMenu.effectButton);

        if (adventMonster && !adventMonster.Advented)
            cardButtons.Add(cardInfoMenu.adventButton);

        cardInfoMenu.OpenMenuOn3DSpace(card, cardButtons);
    }

    /// <summary>
    /// ディフェンスできるかどうか返す
    /// </summary>
    /// <returns></returns>
    public bool CanDefence()
    {
        if((turnCanDefenceNum - turnDefencedNum > 0 && turnAttackedNum == 0) && CanCommand())
        {
            UtilityLog.CardLog("ディフェンスできます", card, UtilityLog.LogCategory.none);
            return true;
        }
        else
        {
            UtilityLog.CardLog($"ディフェンスできません{turnCanDefenceNum - turnDefencedNum > 0},{turnAttackedNum == 0},{CanCommand()}", card, UtilityLog.LogCategory.none);
            return false;
        }
    }

    /// <summary>
    /// アタック時にカウンター変数を1増加させる
    /// </summary>
    public void AddAttackCount()
    {
        attackedNum++;
        turnAttackedNum++;
        commandedNum++;
        turnCommandedNum++;        
    }

    /// <summary>
    /// ディフェンス時にカウンター変数を1増加させる
    /// </summary>
    public void AddDefenceCount()
    {
        defencedNum++;
        turnDefencedNum++;
        commandedNum++;
        turnCommandedNum++;
    }
}
