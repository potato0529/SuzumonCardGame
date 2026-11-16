using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventMonster : MonoBehaviour
{
    [Header("降臨数")] public int adventNum;
    [Header("降臨生贄条件")] public TargetCardConditionsNugget conditionsNugget;
    [Header("降臨後の効果情報")] public EffectData adventedEffectData;
    [HideInInspector] public Monster monster;
    private Card card;
    private bool advented;
    public bool Advented
    {
        get { return advented; }
        set
        {
            advented = value;
            if(value == false && card.cardState == Card.CardState.Field)
            {
                card.SetColor(new Color(1, 1, 1, 0.6f));
            }
            else
            {
                card.SetColor(new Color(1, 1, 1, 1));
            }
        }
    }

    private void Awake()
    {
        monster = GetComponent<Monster>();
        card = monster.card;
    }

    /// <summary>
    /// 降臨ボタンを押し、降臨できるか確認したのち降臨する。
    /// </summary>
    public void StartAdvent()
    {
        //コマンド可能条件
        if (MatchManager.instance.runningProcess.Count == 0 && MatchManager.instance.phaseManager.nowPhase == PhaseManager.TurnPhases.actionPhase && monster.ownerPlayer.cardPointer.pointerMode == CardPointer.PointerModes.cardInfo)
        {
            if (CanAdventCheck())
            {
                MatchManager.instance.matchActionService.Submit(new AdventButtonCommand(card.controlerPlayer, MatchManager.instance.GetCardId(card)));
                //imp:旧通信/実行処理
                //DecideSacrifice();
                //if (monster.ownerPlayer == MatchManager.instance.localPlayer)
                //    MatchManager.instance.networkPlayer.photonView.RPC(nameof(MatchManager.instance.networkPlayer.OpponentAdventButton), Photon.Pun.RpcTarget.Others, MatchManager.instance.GetGameCards().IndexOf(monster.card));
            }
        }

    }

    /// <summary>
    /// 降臨のいけにえを決める
    /// </summary>
    /// <param name="adventMonster"></param>
    public void DecideSacrifice()
    {
        List<Card> selectableTargets = conditionsNugget.GetMeetConditionsCards(card,(thisCard) => { return !thisCard.cantCost; });
        monster.card.controlerPlayer.cardPointer.SelectAdventCost(adventNum, selectableTargets, PassTargets);
    }

    public bool CanAdventCheck()
    {
        return adventNum <= conditionsNugget.GetMeetConditionsCards(card, (thisCard) => { return !thisCard.cantCost; }).Count + (card.controlerPlayer.life - 1);
        
    }

    public void PassTargets(List<SelectableTarget> selectableTargets)
    {
        Debug.Log("降臨のいけにえが決まりました");
        foreach(SelectableTarget selectableTarget in selectableTargets)
        {
            Card card = selectableTarget.GetComponent<Card>();
            //コストがカードだった場合
            if (card)
            {
                card.ChangeCardState(Card.CardState.Cemetery);
            }
            else
            {
                monster.card.controlerPlayer.ReceiveDamage(1);
            }
        }
        Advent();
    }

    /// <summary>
    /// いけにえが決まったのち、実際に降臨する(効果が変化する)
    /// </summary>
    public void Advent()
    {
        //アタックダメージ設定
        card.cardStatuses.SetStatusValue<AttackStatus>(StatusData.StatusType.Attack, adventedEffectData.monsterEffectData.attackDamage, true, false).DefaultStatusValue = adventedEffectData.monsterEffectData.attackDamage;
        //monster.attack.attackStatus.DefaultStatusValue = adventedEffectData.monsterEffectData.attackDamage;
        monster.attack.turnCanAttackNum = adventedEffectData.monsterEffectData.turnCanAttackNum;
        monster.notAttacked = adventedEffectData.monsterEffectData.notAttacked;

        //設計が悪く非常に分かりづらく、ここだけみるとAdventすることによってCardのeffectNuggets[]の要素に対しては何も変更を行っていないように見えるが、Instantiateをすることによって複製されたGenericEffectNuggetのAwake()内にある、「card.effectNuggets[((byte)invokeTiming)] = this;」という処理によって実はeffectNuggets[]に対して直接書き換えが行われている。
        GameObject generatedEffectNuggetsParent = Instantiate(adventedEffectData.effectNuggetsParent.gameObject);
        generatedEffectNuggetsParent.transform.SetParent(monster.transform);

        List<EffectNugget> generatedEffectNuggets = new List<EffectNugget>();

        for (int loops = 0; generatedEffectNuggetsParent.transform.childCount > loops; loops++)
        {
            EffectNugget childsEffectNugget = generatedEffectNuggetsParent.transform.GetChild(loops).GetComponent<EffectNugget>();
            if (childsEffectNugget)
            {
                generatedEffectNuggets.Add(childsEffectNugget);
                childsEffectNugget.card = card;
                childsEffectNugget.name = "EffectNugget(" + card.cardData.cardName + ")";
                
                //warn:おそらく、GenericEffectNuggetのAwake()内の処理をInstantiate時ではなくこのタイミングで呼ぶためにわざわざ最初は非アクティブで、ここでアクティブにしていると考えられる。この実装は気持ち悪いので、こんなことするくらいなら専用の初期化メソッドなどを作るべきな気はするが、降臨とそうじゃないときの処理を共通化するのも難しい。
                childsEffectNugget.gameObject.SetActive(true);
            }
        }

        //初期ステータスを反映
        foreach (StatusData statusData in adventedEffectData.statusDatas)
        {
            card.cardStatuses.SetStatusValue<Status>(statusData.statusType, statusData.statusValue,true,false).DefaultStatusValue = statusData.statusValue;
        }

        //降臨時、召喚時効果にあたるものを発動する
        card.ComeInvokeTiming(EffectNugget.InvokeTiming.Appear);
        
        Advented = true;
        card.ComeInvokeTiming(EffectNugget.InvokeTiming.Advented);
        UtilityLog.CardLog($"{card} の降臨が完了しました", card, UtilityLog.LogCategory.none);
    }

    /// <summary>
    /// データ内のEffectNuggetを複製し、親を設定する
    /// </summary>
    /// <param name="duplicateSourceEffectNugget">複製元の効果ナゲットデータ</param>
    /// <param name="parentCard">所属するカード</param>
    public EffectNugget DuplicateEffectNuggetData(EffectNugget duplicateSourceEffectNugget, Card parentCard)
    {
        EffectNugget generatedEffectNugget = Instantiate(duplicateSourceEffectNugget);
        generatedEffectNugget.transform.SetParent(parentCard.effectNuggetsParent);
        generatedEffectNugget.card = parentCard;
        generatedEffectNugget.name = "EffectNugget(" + card.name + ")";
        generatedEffectNugget.gameObject.SetActive(true);
        return generatedEffectNugget;
    }
}