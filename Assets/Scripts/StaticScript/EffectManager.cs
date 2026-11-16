using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour,IPassTarget
{
    public static EffectManager instance;
    private Player userPlayer;
    public Ability ability;
    private Effect effect;
    private Cost cost;
    private int mainValue;
    public Card userCard;
    public GameObject uniqueEffectNugget;
    private SearchCard searchCard;
    private CardPointer cardPointer;
    private IENVariable iEN;
    private List<Card> interruptedTargets = new List<Card>();
    public List<List<Card>> targetsSets = new List<List<Card>>();
    private string selectingText = "";
    private bool compMultipleTargetCardEffect = false;//複数ターゲットの効果の複数効果が完了した
    
    private void Start()
    {
        instance = this;
    }

    /// <summary>
    /// コストを払うことができるかチェックする
    /// </summary>
    /// <returns>コストを払うことができればtrue</returns>
    public bool CanCostCheck(Player pScr,IENVariable thisUser,Card cardScr,Cost costInfo)
    {
        bool canCost = true;

        List<Card> selectableTargets = new List<Card>();
        if (costInfo.costType.IsCostTargetCard())
        {
            selectableTargets = costInfo.targetDecideNugget.SearchTotalTargets(cardScr,costInfo.GetCostExpandCondition(cardScr), (card) => { return !card.cantCost || card.cardState == Card.CardState.Hand; });
        }
        if(selectableTargets.Count > 0)
        UtilityLog.CardLog(selectableTargets[0] + "をコストの対象にできます", selectableTargets[0], UtilityLog.LogCategory.Cost);
        //ステータスコストの発動条件考慮忘れてね？
        switch (costInfo.costType)
        {
            case Cost.Costs.life:
                if (costInfo.mainValue.GetValue(cardScr) < pScr.life)
                {
                    canCost = true;
                }
                else
                {
                    canCost = false;
                }
                break;

            case Cost.Costs.hand:                
                
                selectableTargets.Remove(cardScr);
                if (costInfo.focusValue.GetValue(cardScr) <= selectableTargets.Count || costInfo.allowBelowTargets)
                {
                    Debug.Log("手札コストを払えます");
                    canCost = true;
                }
                else
                {
                    Debug.Log("手札コストを払えません");
                    canCost = false;
                }
                break;

            case Cost.Costs.fieldMonster:
                //af:自分除くか除かないかの情報をもとに下の処理をするか決めたい
                //seletableFieldTargets.Remove(cardScr);
                if (costInfo.focusValue.GetValue(cardScr) <= selectableTargets.Count || costInfo.allowBelowTargets)
                {
                    Debug.Log("フィールドコストを払えます");
                    canCost = true;
                }
                else
                {
                    Debug.Log("フィールドコストを払えません");
                    canCost = false;
                }
                break;

            case Cost.Costs.DestroyFieldMonster:
                if (costInfo.focusValue.GetValue(cardScr) <= selectableTargets.Count || costInfo.allowBelowTargets)
                {
                    Debug.Log("フィールドコストを払えます");
                    canCost = true;
                }
                else
                {
                    Debug.Log("フィールドコストを払えません");
                    canCost = false;
                }
                break;

            case Cost.Costs.Bounce:
                //af:自分除くか除かないかの情報をもとに下の処理をするか決めたい
                //selectableTargets.Remove(cardScr);

                if (costInfo.focusValue.GetValue(cardScr) <= selectableTargets.Count || costInfo.allowBelowTargets)
                {
                    Debug.Log("フィールドコストを払えます");
                    canCost = true;
                }
                else
                {
                    Debug.Log("フィールドコストを払えません");
                    canCost = false;
                }
                break;

            case Cost.Costs.Status:
                if (costInfo.focusValue.GetValue(cardScr) <= selectableTargets.Count || costInfo.allowBelowTargets)
                {
                    Debug.Log("ステータスコストを払えます");
                    canCost = true;
                }
                else
                {
                    Debug.Log("ステータスコストを払えません");
                    canCost = false;
                }
                break;

            case Cost.Costs.SendCemeteryCard:
                //seletableFieldTargets.Remove(cardScr);
                if (costInfo.focusValue.GetValue(cardScr) <= selectableTargets.Count || costInfo.allowBelowTargets)
                {
                    Debug.Log("コストを払えます");
                    canCost = true;
                }
                else
                {
                    Debug.Log("コストを払えません");
                    canCost = false;
                }
                break;

            case Cost.Costs.ReturnDeck:
                if (costInfo.focusValue.GetValue(cardScr) <= selectableTargets.Count || costInfo.allowBelowTargets)
                {
                    Debug.Log("コストを払えます");
                    canCost = true;
                }
                else
                {
                    Debug.Log("コストを払えません");
                    canCost = false;
                }
                break;

        }

        return canCost;
    }
 
    /// <summary>
    /// 効果を使うことができるかチェックする
    /// </summary>
    /// <returns>効果を使うことができればtrue</returns>
    public bool CanUseEffectCheck(Player pScr,IENVariable thisUser,Card cardScr,Effect effectInfo)
    {
        bool canUseEffect = true;

        //af:あとでConditionTypeによってチェックするターゲットを分ける
        if (effectInfo.effectType.IsEffectTargetCard() || effectInfo.effectType.IsEffectMultipleTargetCard())
        {
            canUseEffect = effectInfo.targetDecideNugget.SearchTotalTargets(cardScr,effectInfo.GetEffectExpandCondition(cardScr)).Count > 0;
            if (effectInfo.effectType.IsEffectMultipleTargetCard() && canUseEffect)
            {
                canUseEffect = effectInfo.secondDecideNugget.SearchTotalTargets(cardScr, effectInfo.GetEffectExpandCondition(cardScr)).Count > 0;
            }
        }

        switch (effectInfo.effectType)
        {
            case Effect.EffectTypes.Draw:
                break;
            case Effect.EffectTypes.Search:
                canUseEffect = effectInfo.targetDecideNugget.SearchTotalTargets(cardScr).Count > 0;
                break;
            
            case Effect.EffectTypes.Damage:
                break;

            case Effect.EffectTypes.healLife:
                break;
            
            case Effect.EffectTypes.handdeath:
                break;

            case Effect.EffectTypes.lifeBlocker:
                break;

            case Effect.EffectTypes.changeField:
                if (FieldManager.instance.NowFieldNum != effectInfo.mainValue.GetValue(cardScr))
                {
                    canUseEffect = true;
                }
                else
                {
                    canUseEffect = false;
                }
                break;

            case Effect.EffectTypes.ChooseEffect:
                foreach(EffectNugget choice in effectInfo.GetComponent<Effect_Choices>().effectNuggetChoices)
                {
                    if (choice.CanUseCheck())
                    {
                        return true;
                    }
                }
                return false;
        }
        return canUseEffect;
    }

    public void PayCost(Player pScr, IENVariable thisUser, Card cardScr, Cost costInfo)
    {
        SetCostParam(pScr, thisUser, cardScr, costInfo);
        cost.ResetParam();

        if (costInfo.costType.IsCostTargetCard())
        {
            //※1 ※1で囲まれた領域は、ステータスコストのときのみの処理を書こうとしていたが、何故かこれがあるとばぐるので一回無効とし、※1のあとにある普通のSearchTargetで（元のまんま）
            //if (costInfo.costType == Cost.Costs.Status)
            //{
            //    //ability.selectInfo.expandTargetConditions.Add((card) => { return card.cardStatuses.statuses[(byte)costInfo.statusType] && (costInfo.allowBelowValue || card.cardStatuses.statuses[(byte)costInfo.statusType].StatusValue >= costInfo.mainValue.Value); });
            //    //warn:ステータスコストのときのみ、コストにできないカードも対象にできるようにしているが、正直この条件分岐は体系的でなく無理やりなのでどうにかしたい
            //    SearchTarget(costInfo.targetDecideNugget.SearchTotalTargets(cardScr, costInfo.GetCostExpandCondition(cardScr), (card) => { return true; }), ability.focusType, ability.focusValue.GetValue(cardScr), costInfo.costType.GetCostName() + "カード");

            //}
            //else
            //{
            //    SearchTarget(costInfo.targetDecideNugget.SearchTotalTargets(cardScr, costInfo.GetCostExpandCondition(cardScr), (card) => { return !card.cantCost || card.cardState == Card.CardState.Hand; }), ability.focusType, ability.focusValue.GetValue(cardScr), costInfo.costType.GetCostName() + "カード");

            //}
            //※1

            SearchTarget(costInfo.targetDecideNugget.SearchTotalTargets(cardScr, costInfo.GetCostExpandCondition(cardScr), (card) => { return !card.cantCost || card.cardState == Card.CardState.Hand; }), ability.focusType, ability.focusValue.GetValue(cardScr), costInfo.costType.GetCostName() + "カード");

        }
        else if(costInfo.costType == Cost.Costs.life)
        {
            pScr.ReceiveDamage(mainValue);
            EndCost();
        }
    }

    #region 様々なUseEffect
    /// <summary>
    /// 万能UseEffect(効果の種類に応じてメソッドを呼び分ける。基本はこのメソッドを使えば全ての一般効果を使用できる。)
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="cardScr"></param>
    /// <param name="effectInfo"></param>
    public void UseEffect(Player pScr, IENVariable thisUser, Card cardScr, Effect effectInfo)
    {
        SetEffectParam(pScr, thisUser, cardScr, effectInfo);
        effectInfo.ResetParam();
        if (effectInfo.effectType.NeedOnlyOneValue())
        {
            UseEffect_OnlyOneValue(userPlayer, effectInfo.effectType, effectInfo.mainValue.GetValue(cardScr));
        }
        else if (effectInfo.effectType.IsEffectTargetPlayer())
        {
            UseEffect_TargetPlayer(cardScr, effectInfo);
        }
        else if (effectInfo.effectType == Effect.EffectTypes.Summon || effectInfo.effectType == Effect.EffectTypes.ForcedSummon)
        {
            UseEffect_Summon(effectInfo);
        }
        else if (effectInfo.effectType.IsEffectTargetCard())
        {
            UseEffect_TargetCard(userCard, effectInfo);
        }
        else if (effectInfo.effectType.OtherEffect())
        {
            UseEffect_Other(userPlayer, userCard, effectInfo);
        }
        else if (effectInfo.effectType.IsEffectNeedNoData())
        {
            UseEffect_NeedNoData(userPlayer, userCard, effectInfo);
        }
        else
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Defect, "効果の処理カテゴリに当てはまりませんでした");
        }
    }

    /// <summary>
    /// 効果を使う(効果が1種類のみで、選択が不要)
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="cardScr"></param>
    /// <param name="thisEffectType"></param>
    /// <param name="effectNum"></param>
    private void UseEffect_OnlyOneValue(Player pScr, Effect.EffectTypes thisEffectType, int effectNum)
    {
        switch (thisEffectType)
        {
            case Effect.EffectTypes.Draw:
                for (byte loops = 1; loops <= effectNum; loops++)
                {
                    pScr.Draw();
                }
                break;

            case Effect.EffectTypes.Damage:
                pScr.opponentPlayer.ReceiveDamage(effectNum);
                break;

            case Effect.EffectTypes.healLife:
                pScr.HealLife(effectNum);
                break;

            case Effect.EffectTypes.changeField:
                Debug.Log("フィールドを変更します");
                FieldManager.instance.ChangeField(effectNum);
                break;

            case Effect.EffectTypes.ChangeSummonRights:
                userCard.controlerPlayer.remainingSummonRights += mainValue;
                break;

            case Effect.EffectTypes.ChangeItemRights:
                userCard.controlerPlayer.remainingUseItemRights += mainValue;
                Debug.Log("アイテム権を増やしました");
                break;

                //今後コルーチンにしてDiceもこっちに移動する
        }
        EndEffect();
    }

    /// <summary>
    /// 効果を使う（ユーザーを対象に適用する効果)
    /// </summary>
    /// <param name="userCard"></param>
    /// <param name="effectInfo"></param>
    private void UseEffect_TargetPlayer(Card userCard, Effect effectInfo)
    {
        List<Player> targetPlayers = effectInfo.targetPlayer.GetTargetPlayers(userCard.controlerPlayer);

        foreach (Player targetPlayer in targetPlayers)
        {
            switch (effectInfo.effectType)
            {
                case Effect.EffectTypes.Draw:
                    for (byte loops = 1; loops <= effectInfo.mainValue.GetValue(userCard); loops++)
                    {
                        targetPlayer.Draw();
                    }
                    break;

                case Effect.EffectTypes.Damage:
                    targetPlayer.ReceiveDamage(effectInfo.mainValue.GetValue(userCard));
                    break;

                case Effect.EffectTypes.healLife:
                    targetPlayer.HealLife(effectInfo.mainValue.GetValue(userCard));
                    break;

                case Effect.EffectTypes.SetLife:
                    targetPlayer.ChangeLife(mainValue);
                    break;
            }
        }
        EndEffect();
    }

    /// <summary>
    /// ターゲットに対して実行する効果を使う。
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="cardScr"></param>
    /// <param name="effectInfo"></param>
    private void UseEffect_TargetCard(Card cardScr, Effect effectInfo)
    {
        SearchTarget(effectInfo.targetDecideNugget.SearchTotalTargets(cardScr,effectInfo.GetEffectExpandCondition(userCard)),ability.focusType,ability.focusValue.GetValue(cardScr),effectInfo.effectType.GetEffectName() + "カード");
    }

    /// <summary>
    /// ターゲット設定に応じてターゲットを探す。(選択処理をするか、即決定されてUseEffectOrderまで実行される)
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="cardScr"></param>
    /// <param name="ability"></param>
    private void SearchTarget(List<Card> cards,Ability.TargetFocusType targetFocusType,int focusValue,string selectText)
    {
        switch (targetFocusType)
        {
            case Ability.TargetFocusType.All:
                PassTargets(cards);
                break;

            case Ability.TargetFocusType.Select:
                userPlayer.cardPointer.SetCardImage(userCard);
                userPlayer.cardPointer.SelectCard(focusValue, ability.allowBelowTargets, cards, CardPointer.SelectType.ability, PassTargets,selectText);
                break;

            case Ability.TargetFocusType.Random:
                List<Card> pickCardsPool = cards;
                List<Card> randomizedCards = new List<Card>();
                for (int i = 0; i < focusValue; i++)
                {
                    Card randomizedCard = SuzumonTools.RandomTargets(pickCardsPool);
                    randomizedCards.Add(randomizedCard);
                    pickCardsPool.Remove(randomizedCard);
                }
                PassTargets(randomizedCards);
                break;
        }
    }

    /// <summary>
    /// 効果を使う(サーチウィンドウが必要)
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="cardScr"></param>
    /// <param name="effectInfo"></param>
    private void UseEffect_Summon(Effect effectInfo)
    {
        switch (effectInfo.effectType)
        {
            case Effect.EffectTypes.Summon:
                searchCard.gameObject.SetActive(true); //サーチウィンドウをアクティブにする
                cardPointer.searchWindowMode = true;
                //召喚条件を満たしていないモンスターを取り除くなど、効果特殊条件の設定が必要
                List<Card> summonableCards = effectInfo.targetDecideNugget.SearchTotalTargets(userCard,effectInfo.GetEffectExpandCondition(userCard));
                searchCard.EffectSummon(summonableCards, EffectSummoned, effect.focusValue.GetValue(userCard),"場に出すカード"); //サーチウィンドウで対象カードをサーチする
                break;

            case Effect.EffectTypes.ForcedSummon:
                searchCard.gameObject.SetActive(true); //サーチウィンドウをアクティブにする
                cardPointer.searchWindowMode = true;
                //召喚条件を満たしていないモンスターを取り除くなど、効果特殊条件の設定が必要
                List<Card> forcedSummonableCards = effectInfo.targetDecideNugget.SearchTotalTargets(userCard, effectInfo.GetEffectExpandCondition(userCard));
                cardPointer.isForcedSummon = true;
                searchCard.EffectSummon(forcedSummonableCards, EffectSummoned, effect.focusValue.GetValue(userCard), "場に出すカード"); //サーチウィンドウで対象カードをサーチする
                break;
        }
    }

    public void EffectSummoned(List<Card> summonedCards)
    {
        targetsSets.Add(summonedCards);//普通にPassTargets()に渡すのもありじゃね この2行もともとifの外にあったけど明らかに召喚時のみにすべきだよね？
        EndEffect();//imp この行を消して下に戻す可能性もあり
    }

    /// <summary>
    /// infoは必要だけど選択は不要など、その他の効果
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="cardScr"></param>
    /// <param name="effectInfo"></param>
    private void UseEffect_Other(Player pScr, Card cardScr, Effect effectInfo)
    {
        switch (effectInfo.effectType)
        {
            case Effect.EffectTypes.AntiqueBurst:
                TriggerEffectManager.instance.AddAntiqueQueue(new AntiqueBurst(effectInfo,cardScr,MatchManager.instance.turnPlayer));
                EndEffect();//今後変更
                break;

            case Effect.EffectTypes.ChooseEffect:
                Effect_Choices effect_Choices = effectInfo.GetComponent<Effect_Choices>();
                if (effect_Choices.doIChoose)
                {
                    pScr.chooseEffect.StartChoose(effect_Choices.effectNuggetChoices);
                }
                else
                {
                    pScr.opponentPlayer.chooseEffect.StartChoose(effect_Choices.effectNuggetChoices);
                }
                //EndEffect(); //今後変更
                break;

            case Effect.EffectTypes.RollDice:
                StartCoroutine(MatchManager.instance.dice.RollDice(Dice));
                break;
        }
    }

    /// <summary>
    /// infoは必要だけど選択は不要など、その他の効果
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="cardScr"></param>
    /// <param name="effectInfo"></param>
    private void UseEffect_NeedNoData(Player player, Card card, Effect effectInfo)
    {
        switch (effectInfo.effectType)
        {
            case Effect.EffectTypes.ExchangeLife:
                //入れ替え処理では一旦入れ替え前の各値を格納する必要がある
                int controllerPlayerLife = card.controlerPlayer.life;
                int opponentPlayerLife = card.controlerPlayer.opponentPlayer.life;

                card.controlerPlayer.ChangeLife(opponentPlayerLife);
                card.controlerPlayer.opponentPlayer.ChangeLife(controllerPlayerLife);
                break;
        }
        EndEffect();
    }

    /// <summary>
    /// サイコロを振る効果でサイコロで出た値を受け取る
    /// </summary>
    /// <param name="diceValue"></param>
    public void Dice(int diceValue)
    {
        mainValue = diceValue;
        EndEffect();
    }
    #endregion

    /// <summary>
    /// UseEffectにおいて処理が完了したときの共通処理
    /// </summary>
    public void EndEffect()
    {
        iEN.CompUsedEffect();
        EndProcess();
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Effect,"単体効果処理が終了します");
    }

    public void EndCost()
    {
        iEN.PaidCost();
        EndProcess();
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Cost, "単体コスト処理が終了します");
    }

    /// <summary>
    /// ターゲットを選んで発動する効果で、選択されたカードに対して効果を実行する
    /// </summary>
    /// <param name="targets"></param>
    public void UseEffectForTarget(List<Card> targets)
    {
        Debug.Log("ターゲットに対し効果を実行します " + effect.effectType);
        ability.targetedCardsValue = targets.Count;
        foreach (Card target in targets)
        {
            UtilityLog.CardLog($"{target} に対し {effect.effectType} を実行", target, UtilityLog.LogCategory.Effect);
            if (!uniqueEffectNugget)
            {
                switch (effect.effectType)
                {
                    case Effect.EffectTypes.DestroyCard:
                        target.BeDisruptedByEffect(Effect.EffectTypes.DestroyCard, userCard);

                        //Destroy(target.selectableTarget); 不要なはず

                        break;

                    case Effect.EffectTypes.SendCemetery:
                        target.ChangeCardState(Card.CardState.Cemetery);

                        //Destroy(target.lockon); 不要なはず

                        break;

                    case Effect.EffectTypes.Bounce:
                        target.Bounced(); //are:これChangeCardStateじゃないとだめなんじゃね
                        break;

                    case Effect.EffectTypes.ReturnDeck:
                        target.ChangeCardState(Card.CardState.Deck, mainValue);
                        break;

                    case Effect.EffectTypes.Noroi:
                        target.Noroware();
                        break;

                    case Effect.EffectTypes.Search:
                        target.AddedToHand();
                        break;

                    case Effect.EffectTypes.setAttackDamage:
                        //target.monsterScr.attack.attackStatus.AddGivedStatus(effect); givedStatusだと意味が変わっちゃう
                        target.cardStatuses.SetStatusValue<AttackStatus>(StatusData.StatusType.Attack, mainValue, true, false);
                        break;

                    case Effect.EffectTypes.changeAttackDamage:
                        Debug.Log(target.name + "のアタックダメージを" + ability.mainValue + "増加させます");
                        //target.monsterScr.attack.attackStatus.AddGivedStatus(effect); givedStatusだと意味が変わっちゃう
                        //target.monsterScr.attack.attackStatus.AddValue(mainValue);
                        target.cardStatuses.SetStatusValue<AttackStatus>(StatusData.StatusType.Attack, mainValue, false, false);
                        break;

                    case Effect.EffectTypes.scapegoat:
                        Protected addedProtected = target.gameObject.AddComponent<Protected>();
                        addedProtected.protectCard = userCard;
                        break;

                    case Effect.EffectTypes.PileCard:
                        if(targetsSets.Count == 1)
                        {
                            StartCoroutine(DelaySearchTarget(effect.secondDecideNugget.SearchTotalTargets(userCard),ability.focusType2,ability.focusValue2.GetValue(userCard),"下に重なるカード"));
                        }
                        else
                        {
                            targetsSets[0][0].cardLayer.PileUnderThis(target);
                            compMultipleTargetCardEffect = true;
                        }
                        
                        break;

                    case Effect.EffectTypes.Paralysis:
                        //target.cardStatuses.AddParalysis(ability.mainValue);
                        target.monsterScr.barrier.BreakBarrier();
                        target.cardStatuses.SetStatusValue<Paralysis>(StatusData.StatusType.Paralysis,mainValue,false,false);
                        target.Paralysed();
                        break;

                    case Effect.EffectTypes.Preservation:
                        if (targetsSets.Count == 1)
                        {
                            StartCoroutine(DelaySearchTarget(effect.secondDecideNugget.SearchTotalTargets(userCard), ability.focusType2, ability.focusValue2.GetValue(userCard), "下に重なるカード"));
                        }
                        else
                        {
                            target.Preserved();
                            Preserved preservedStatus = target.cardStatuses.SetStatusValue<Preserved>(StatusData.StatusType.Preserved, mainValue, true, false);
                            preservedStatus.preservingParent = targetsSets[0][0];
                            targetsSets[0][0].leftFieldCallback.Add(preservedStatus.ParentLeaveField);
                            compMultipleTargetCardEffect = true;
                        }
                        break;

                    case Effect.EffectTypes.Poison:
                        if (!target.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Poison))
                        {
                            target.cardStatuses.SetStatusValue<Poison>(StatusData.StatusType.Poison, mainValue, true, false);
                        }
                        else
                        {
                            target.cardStatuses.SetStatusValue<Poison>(StatusData.StatusType.Poison, -1, false, false);
                        }
                        target.Poisoned();
                        break;

                    case Effect.EffectTypes.AddStatus:
                        target.cardStatuses.SetStatusValue<Status>(ability.statusType, mainValue, false,false);
                        break;

                    case Effect.EffectTypes.GiveBarrier:
                        target.monsterScr.barrier.SetRemainBarrier(true);
                        break;

                    case Effect.EffectTypes.ReturnSpecialDeck:
                        target.ChangeCardState(Card.CardState.SpecialDeck);
                        break;

                    case Effect.EffectTypes.Choose:
                        break;
                }
            }
            else
            {
                Debug.Log("固有効果です");
                IPassTarget iPT = uniqueEffectNugget.GetComponent(typeof(IPassTarget)) as IPassTarget;
                iPT.PassTargets(targets);
            }
        }

        if (!effect.effectType.IsEffectMultipleTargetCard() || compMultipleTargetCardEffect)
        {
            //効果妨害を受けて相手が避けるなどの割り込み処理が入る場合終了処理EndEffect()を行わず中断する
            if (interruptedTargets.Count == 0)
            {               
                EndEffect();
                compMultipleTargetCardEffect = false;
            }
        }
    }

    /// <summary>
    /// 即席過ぎて流石にあとで処理かえる
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="targetFocusType"></param>
    /// <param name="focusValue"></param>
    /// <returns></returns>
    public IEnumerator DelaySearchTarget(List<Card> cards,Ability.TargetFocusType targetFocusType,int focusValue,string selectText) 
    {
        yield return null;
        SearchTarget(cards, targetFocusType, focusValue,selectText);
    }

    /// <summary>
    /// ターゲットが外部クラスによって決まっている状態で、選択されたカードに対して効果を実行する
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="thisUserCard"></param>
    /// <param name="thisEffectType"></param>
    /// <param name="thisEffectNum">ターゲット数はターゲットが決まってるので不要であるため、ターゲット数以外の値が不要であれば0、必要であればその値を</param>
    public void UseEffectForTarget(List<Card> targets, Player pScr, IENVariable thisUser, Card thisUserCard, Effect.EffectTypes thisEffectType, int thisEffectNum)
    {
        Debug.Log("ターゲットに対し効果を実行します");
        userPlayer = pScr;
        userCard = thisUserCard;
        searchCard = pScr.searchCard;
        cardPointer = pScr.cardPointer;
        mainValue = thisEffectNum;
        iEN = thisUser;

        UseEffectForTarget(targets);
    }

    public IEnumerator EndInterrupt(Card interrupter)
    {
        yield return null;
        interruptedTargets.Remove(interrupter);
    }

    /// <summary>
    /// ターゲットを選んでコストにする効果で、選択されたカードをコストにする
    /// </summary>
    /// <param name="targets"></param>
    public void PayTargetCost(List<Card> targets)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Cost, "対象をコストとします");
        ability.targetedCardsValue = targets.Count;
        //iEN = user.GetComponent(typeof(IENVariable)) as IENVariable; mnp:iENは既に清吉を持っているはず
        foreach (Card target in targets)
        {
            switch (cost.costType)
            {
                case Cost.Costs.hand:
                    UtilityLog.CategorizedLog(UtilityLog.LogCategory.Cost, "コストとして墓地に送ります");
                    UtilityLog.CardLog("コストとして墓地に送ります", target, UtilityLog.LogCategory.Cost);
                    target.ChangeCardState(Card.CardState.Cemetery);
                    break;

                case Cost.Costs.fieldMonster:
                    target.ChangeCardState(Card.CardState.Cemetery);
                    break;

                case Cost.Costs.DestroyFieldMonster:
                    target.BeDestroyed();
                    break;

                case Cost.Costs.SendCemeteryCard:
                    target.ChangeCardState(Card.CardState.Cemetery);
                    break;

                case Cost.Costs.Bounce:
                    target.Bounced();
                    break;

                case Cost.Costs.Status:
                    if (target.cardStatuses.statusesDictionary.ContainsKey(ability.statusType))
                    {
                        if (cost.statusType != StatusData.StatusType.Poison)
                        {
                            target.cardStatuses.statusesDictionary[ability.statusType].AddValue(mainValue);
                        }
                        else
                        {
                            target.cardStatuses.SetStatusValue<Poison>(StatusData.StatusType.Poison, -1, false, false);
                        }
                    }
                    else
                    {
                        target.cardStatuses.SetStatusValue<Status>(ability.statusType,mainValue,false,false);
                    }
                    break;

                case Cost.Costs.ReturnDeck:
                    target.ChangeCardState(Card.CardState.Deck, mainValue);
                    break;
            }
        }
        EndCost();
    }

    /// <summary>
    /// CardPointerなどから選ばれたターゲットを受け取る
    /// </summary>
    /// <param name="targets"></param>
    public void PassTargets(List<Card> thisTargets)
    {
        targetsSets.Add(thisTargets);
        foreach(Card target in thisTargets)
        {
            if(effect && effect.effectType != Effect.EffectTypes.none)
            {
                UseEffectForTarget(targetsSets[targetsSets.Count - 1]);
            }
            else if(cost && cost.costType != Cost.Costs.none)
            {
                PayTargetCost(targetsSets[targetsSets.Count - 1]);
            }
            else
            {
                Debug.Log("ターゲットを渡す先がありません");
            }
        }
        if(thisTargets.Count == 0)
        {
            Debug.Log("ターゲットが無ェ！！");
            if (effect)
            {
                EndEffect();
            }
            else if(cost)
            {
                EndCost();
            }
        }
    }

    public void AddInterrupt(Card interruptCard)
    {
        interruptedTargets.Add(interruptCard);
    }

    /// <summary>
    /// 使用中の効果の情報を保存する
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="thisUserCard"></param>
    /// <param name="thisEffectType"></param>
    /// <param name="thisEffectNum"></param>
    private void SetEffectParam(Player pScr, IENVariable thisUser, Card thisUserCard,Effect effectInfo)
    {
        effect = effectInfo;
        SetUserParam(pScr, thisUser, thisUserCard, effectInfo);
    }

    /// <summary>
    /// 使用中のコストの情報を保存する
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="thisUserCard"></param>
    /// <param name="thisEffectType"></param>
    /// <param name="thisEffectNum"></param>
    private void SetCostParam(Player pScr, IENVariable thisUser, Card thisUserCard, Cost costInfo)
    {
        cost = costInfo;
        SetUserParam(pScr, thisUser, thisUserCard, costInfo);
    }

    /// <summary>
    /// 使用中の効果の情報を保存する処理の共通処理
    /// </summary>
    /// <param name="pScr"></param>
    /// <param name="thisUser"></param>
    /// <param name="thisUserCard"></param>
    /// <param name="thisEffectType"></param>
    /// <param name="thisEffectNum"></param>
    private void SetUserParam(Player pScr, IENVariable thisUser, Card thisUserCard ,Ability thisAbility)
    {
        //UtilityLog.CategorizedLog(UtilityLog.LogCategory.Effect, thisAbility.name + "の効果情報をセットします"); ←このログは要らんことが多いな
        userPlayer = pScr;
        userCard = thisUserCard;
        searchCard = pScr.searchCard;
        cardPointer = pScr.cardPointer;
        ability = thisAbility;
        
        mainValue = ability.mainValue.GetValue(userCard);
        
        iEN = thisUser;
        //Debug.Log("効果情報のセットが終了しました"); ←このログは要らんことが多いな
    }

    /// <summary>
    /// EffectManagerが担当した1つの効果が完了し、関連データを初期化する
    /// </summary>
    public void EndProcess()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.EffectNuggetProcess, "効果の情報を初期化します");
        //Abilityに前回効果の記録をする
        ability.usedMainValue = mainValue;
        ability.usedConditionNuggetValue = ability.focusValue.GetValue(userCard);
        if (targetsSets.Count >= 1)
        {
            ability.targetedCards = targetsSets[0];
            if (targetsSets.Count >= 2)
                ability.secondTargetedCards = targetsSets[1];
        }

        targetsSets = new List<List<Card>>();

        ResetUserParam();
    }

    /// <summary>
    /// 効果情報を初期化する
    /// </summary>
    private void ResetUserParam()
    {
        userPlayer = null;
        userCard = null;
        searchCard = null;
        cardPointer = null;
        iEN = null;
        effect = null;
        cost = null;
    }
}
