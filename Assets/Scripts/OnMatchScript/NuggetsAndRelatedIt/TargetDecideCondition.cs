using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TargetDecideCondition
{
    [Header("ターゲットの決定方法")] public TargetConditionType targetConditionType;
    [Header("ターゲット条件")] public TargetCardConditionsNugget targetConditionsNugget;
    [Header("参照する効果")] public Ability referenceEffect; //ターゲット決定関係で一部のモードで必要な指定の効果データ
    //[Header("参照する効果（汎用版"), Interface(typeof(IUsedTargetResult))] public MonoBehaviour _ReferenceEffect;
    [SerializeField, InterfaceType(typeof(IUsedTargetResult)), Header("関連カード参照")] public UnityEngine.Object _usedTargetResult;
    public IUsedTargetResult usedTargetResult => _usedTargetResult as IUsedTargetResult;
    [Header("デッキ・墓地などのインデックス指定")] public IndexSelector indexSelector;
    [Header("関連ターゲットの絞り込み")] public TotalTargetDecideNugget relatedTargetDecideNugget;
    [Header("除外条件")] public bool isExcludeCondition;
    
    //使いまわせるようにコンディションタイプを増やしているが、カードに付属しているものかどうかなどで対応しているものは限られる。カードに付属していないのに付属カード参照が必要なタイプを選択したときはエラーになるという前提。本来こういう例外を実現できてしまうのはあまりよくないが、利便性を高めるためにそこは正しく判断しようという感じ。
    public enum TargetConditionType
    {
        AllCondition,
        Myself,
        TopNCards,
        SelectedTargets, //前の効果で既に選択されたターゲット
        EquipCard, //このウェポン(カード)を装備しているカード
        PiledCards,
        UsedTargetResult
    }

    public virtual List<Card> SearchMeetConditionTypeCards()
    {
        return new List<Card>();
    }

    /// <summary>
    /// 条件に当てはまるカード群を取得する。
    /// </summary>
    /// <param name="player"></param>
    /// <param name="userCard"></param>
    /// <returns></returns>
    public List<Card> SearchTargets(Card userCard, params Func<Card, bool>[] expandTargetConditions)
    {
        switch (targetConditionType)
        {
            
            case TargetConditionType.Myself:
                List<Card> myself = new List<Card>();
                //tcn条件に当てはまった場合のみ
                if(targetConditionsNugget.MeetCondition(userCard,userCard,expandTargetConditions))
                    myself.Add(userCard);

                return myself;

            case TargetConditionType.AllCondition:
                List<Card> meetConditionCards = targetConditionsNugget.GetMeetConditionsCards(userCard, expandTargetConditions);
                return indexSelector.GetIndexObjects(meetConditionCards,userCard);

            case TargetConditionType.TopNCards:

                Player targetPlayer = null;
                if(targetConditionsNugget.overloadConditions[0].targetOwner == TargetCardConditions.TargetOwners.Myself)
                {
                    targetPlayer = userCard.controlerPlayer;
                }
                else if (targetConditionsNugget.overloadConditions[0].targetOwner == TargetCardConditions.TargetOwners.Opponent)
                {
                    targetPlayer = userCard.controlerPlayer.opponentPlayer;
                }
                //複数のstateが指定されているTopNCardsは意味が分からないのでindex0のstateを見て判断する
                List<Card> checkCards = targetConditionsNugget.overloadConditions[0].targetCardState.GetCardsCorrespondingToState(targetPlayer);
                checkCards = indexSelector.GetIndexObjects(checkCards, userCard);
                List<Card> resultCards = new List<Card>();
                foreach (Card checkCard in checkCards)
                {
                    if (targetConditionsNugget.MeetCondition(userCard, checkCard))
                    {
                        resultCards.Add(checkCard);
                    }
                }
                return resultCards;

            case TargetConditionType.SelectedTargets:
                Debug.Log($"使われた効果 {referenceEffect} の選択されたターゲット {referenceEffect.targetedCards} をターゲットにします");
                List<Card> selectedTargets = new List<Card>();
                foreach(Card applyCard in referenceEffect.targetedCards)
                {
                    //tcn条件に合っているか調べる
                    if (targetConditionsNugget.MeetCondition(userCard, applyCard))
                        selectedTargets.Add(applyCard);
                }
                return selectedTargets;

            case TargetConditionType.EquipCard:
                //現在は装備処理をしなくても重なっているだけで効果を発動してしまうので応急処置で何もしないようにしていますが今後しっかり明確にしたい
                if (userCard.weapon)
                {
                    if (!userCard.weapon.equipTarget)
                    {
                        break;
                    }
                    List<Card> equipCards = new List<Card>();
                    //tcn条件に合っているか調べる
                    Card equipCard = userCard.weapon.equipTarget.card;
                    if(targetConditionsNugget.MeetCondition(userCard,equipCard))
                        equipCards.Add(equipCard);

                    return equipCards;
                }
                else
                {
                    Debug.LogError("効果ターゲットに装備モンスターが選択されていますが、この効果はウェポンカードのものではありません");
                }
                break;

            case TargetConditionType.PiledCards:
                List<Card> piledCards = new List<Card>();
                foreach(Card parentLayerCard in relatedTargetDecideNugget.SearchTotalTargets(userCard))
                {
                    foreach(Card piledCard in parentLayerCard.cardLayer.layer)
                    {
                        if(targetConditionsNugget.MeetCondition(userCard, piledCard))
                        {
                            piledCards.Add(piledCard);
                        }
                    }
                }
                return piledCards;

            case TargetConditionType.UsedTargetResult:
                Debug.Log(usedTargetResult);
                Debug.Log($"起こった誘発タイミング {usedTargetResult} に関連するカード {usedTargetResult.GetTargetedResultCards()} をターゲットにします");
                List<Card> resultTargets = new List<Card>();
                foreach (Card applyCard in usedTargetResult.GetTargetedResultCards())
                {
                    //tcn条件に合っているか調べる
                    if (targetConditionsNugget.MeetCondition(userCard, applyCard))
                        resultTargets.Add(applyCard);
                }
                return resultTargets;
        }
        return new List<Card>();
    }
}
