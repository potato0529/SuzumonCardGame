using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable]
public class TargetCardConditionsNugget
{
    [Header("条件の候補"),SerializeField] public TargetCardConditions[] overloadConditions = new TargetCardConditions[0];

    /// <summary>
    /// 試合内の全てのカードからこのターゲット条件に当てはまるカードを取得する。(あくまで追加条件は処理の数を減らすための便利なメソッドという扱いで、このクラスに付属するものではない。）※カード群から検索するとき、必ずこのメソッドを通らないといけないような仕組みにしないでください
    /// </summary>
    /// <param name="player">ownerPlayer条件の基準となるプレイヤー</param>
    public List<Card> GetMeetConditionsCards(Card userCard,params Func<Card, bool>[] expandTargetConditions)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "条件'" + this + "'で選択可能なカードを探します");
        List<Card> selectableTargets = new List<Card>();

        foreach (TargetCardConditions targetCardConditions in overloadConditions)
        {
            List<Card> stateCards = targetCardConditions.targetCardState.GetCardsCorrespondingToState();

            string selectableCardsText = "";//デバッグ用
            foreach (Card card in stateCards)
            {
                card.canBeChosen = false;
                if (MeetCondition(userCard,card,expandTargetConditions))
                {
                    selectableTargets.Add(card);
                    selectableCardsText += card.name + ",";
                }
            }
            if(selectableTargets.Count > 0)
                UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "選択可能なカード'" + selectableCardsText.TrimEnd(',') + "'が見つかりました！");

        }
        if (overloadConditions.Length == 0)
        {
            foreach (Card card in MatchManager.instance.GetGameCards())
            {
                selectableTargets.Add(card);
            }
        }

        if (selectableTargets.Count == 0)
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Pointer, "選択可能なカードが見つかりませんでした");
        }

        return selectableTargets;
    }

    /// <summary>
    /// カードが選択可能な条件を満たすか返す
    /// </summary>
    /// <param name="card">調べるカード</param>
    /// <returns>条件を満たすか否か</returns>
    public bool MeetCondition(Card userCard,Card checkCard, params Func<Card, bool>[] expandTargetConditions)
    {
        bool targetCardConditionsCheck = false;
        foreach (TargetCardConditions conditions in overloadConditions)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           //SelectTypeの値の信頼性が担保されなければここはバグるかもしれない。
            bool targetCardConditionCheck = true;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    //
            if (GameManager.instance.logController.meetConditionInfo) Debug.Log($"{checkCard.name} State:{checkCard.cardState == conditions.targetCardState || conditions.targetCardState == Card.CardState.none},Type:{conditions.MeetCardTypeCondition(checkCard)},Category:{conditions.targetCategory == Card.Category.none || checkCard.DoCategoriesMeetCondition(conditions.targetCategory)},Name:{CheckNameCondition(checkCard.cardData.cardName, conditions.targetCardName)},Owner:{(conditions.targetOwner == TargetCardConditions.TargetOwners.Myself && checkCard.controlerPlayer == userCard.controlerPlayer) || (conditions.targetOwner == TargetCardConditions.TargetOwners.Opponent && checkCard.controlerPlayer == userCard.controlerPlayer.opponentPlayer) || (conditions.targetOwner == TargetCardConditions.TargetOwners.Either)},Attribute:{checkCard.attribute == conditions.targetAttribute || conditions.targetAttribute == CardData.CardAttribute.none},Others:{checkCard.selectableTarget.targeted == false && (checkCard.cardState != Card.CardState.Field || !checkCard.monsterScr || !checkCard.monsterScr.adventMonster || checkCard.monsterScr.adventMonster.Advented) && (conditions.targetOptions.Contains(TargetCardConditions.TargetOption.ContainPreserved) || !checkCard.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved))}");

            if (((checkCard.cardState == conditions.targetCardState || conditions.targetCardState == Card.CardState.none) && (conditions.MeetCardTypeCondition(checkCard)) && (conditions.targetCategory == Card.Category.none || checkCard.DoCategoriesMeetCondition(conditions.targetCategory)) && CheckNameCondition(checkCard.cardData.cardName,conditions.targetCardName) && ((conditions.targetOwner == TargetCardConditions.TargetOwners.Myself && checkCard.controlerPlayer == userCard.controlerPlayer) || (conditions.targetOwner == TargetCardConditions.TargetOwners.Opponent && checkCard.controlerPlayer == userCard.controlerPlayer.opponentPlayer) || (conditions.targetOwner == TargetCardConditions.TargetOwners.Either)) && (checkCard.attribute == conditions.targetAttribute || conditions.targetAttribute == CardData.CardAttribute.none) && checkCard.selectableTarget.targeted == false) && (checkCard.cardState != Card.CardState.Field || !checkCard.monsterScr || !checkCard.monsterScr.adventMonster || checkCard.monsterScr.adventMonster.Advented) && (conditions.targetOptions.Contains(TargetCardConditions.TargetOption.ContainPreserved) || !checkCard.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved)))
            {
                foreach (TargetOtherCondition otherConditions in conditions.otherConditions)
                {
                    if (otherConditions.DoesTargetFulfillConditions(userCard,checkCard))
                    {

                    }
                    else
                    {
                        targetCardConditionCheck = false;
                    }
                }               
            }
            else
            {
                targetCardConditionCheck = false;
            }

            //結果的にこの条件は満たしているのか
            if (targetCardConditionCheck)
            {
                targetCardConditionsCheck = true;
                break;
            }
        }
        if (overloadConditions.Length == 0)
        {
            if(GameManager.instance.logController.meetConditionInfo) Debug.Log($"条件がありません。 {checkCard} が当てはまっていることとします。");
            targetCardConditionsCheck = true;
        }

        if (targetCardConditionsCheck)
        {
            foreach (Func<Card, bool> expandTargetCondition in expandTargetConditions)
            {
                if (expandTargetCondition(checkCard))
                {
                    
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DebugLogTargetConditionsNugget()
    {
        foreach (TargetCardConditions condition in overloadConditions)
        {
            condition.DebugLogConditions();
        }
    }

    /// <summary>
    /// カード名の名前条件を確認する
    /// </summary>
    /// <param name="checkName"></param>
    /// <returns></returns>
    public bool CheckNameCondition(string checkName,string conditionName)
    {
        if (conditionName == "") return true;

        if (conditionName.StartsWith("*"))
        {
            return checkName.Contains(conditionName.Remove(0,1));            
        }
        else
        {
            return checkName == conditionName;
        }
    }

    /// <summary>
    /// このターゲット条件だとサーチウィンドウを展開する必要があるか調べる
    /// </summary>
    /// <returns></returns>
    public bool DoNeedSearchWindow()
    {
        foreach (TargetCardConditions targetCardConditions in overloadConditions)
        {
            if (targetCardConditions.targetCardState == Card.CardState.Deck || targetCardConditions.targetCardState == Card.CardState.Cemetery || targetCardConditions.targetCardState == Card.CardState.SpecialDeck)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 関数で条件を設定してオーバーロードコンディションのうちどれか1つでもその条件に当てはまるか調べる
    /// </summary>
    /// <param name="funcToCheck"></param>
    /// <returns></returns>
    public bool ConditionsCheck(Func<TargetCardConditions, bool> funcToCheck)
    {
        foreach (TargetCardConditions targetCardConditions in overloadConditions)
        {
            if (funcToCheck(targetCardConditions))
            {
                return true;
            }
        }
        return false;
    }

    [ContextMenu(nameof(COnan))]
    public void COnan()
    {
        
    }
}

[Serializable]
public class TargetCardConditions
{
    [Header("対象の状態(CardState)")] public Card.CardState targetCardState;
    [Header("対象のカードタイプ"),FormerlySerializedAs("targetCardType")] public DetailCardType targetCardType;
    [Header("対象の属性")] public CardData.CardAttribute targetAttribute;
    [Header("対象カテゴリー")] public Card.Category targetCategory;
    [Header("対象カード名")] public string targetCardName;
    [Header("対象の持ち主")] public TargetOwners targetOwner;
    public List<TargetOtherCondition> otherConditions = new List<TargetOtherCondition>();
    public List<TargetOption> targetOptions = new List<TargetOption>();

    public enum TargetOwners
    {
        Myself,
        Opponent,
        Either,
        none
    }

    public enum DetailCardType
    {
        none = 0,
        Monster = 1,
        Item = 2,
        NormalMonster = 3,
        EvoMonster = 4,
        AdventMonster = 5,
        NormalItem = 6,
        Weapon = 7,
        Eternal = 8,
        Field = 9,
        ExtraItem = 10    
    }

    public enum TargetOption
    {
        ContainPreserved
    }

    public void DebugLogConditions()
    {
        Debug.Log($"State={targetCardState},Type={targetCardType},Category={targetCategory},Name={targetCardName},Owner={targetOwner}");
    }

    /// <summary>
    /// カードタイプが条件と一致しているか確かめる
    /// </summary>
    /// <param name="detailCardType"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool MeetCardTypeCondition(Card target)
    {
        if(targetCardType == DetailCardType.none)
        {
            return true;
        }
        if (targetCardType.GetRoughCardType() == Card.CardType.Monster)
        {
            if (target.cardType == Card.CardType.Monster)
            {
                switch (targetCardType)
                {
                    case DetailCardType.Monster:
                        return true;

                    case DetailCardType.NormalMonster:
                        return target.cardData.effectData.monsterEffectData.monsterType == MonsterEffectData.MonsterType.Normal;

                    case DetailCardType.EvoMonster:
                        return target.cardData.effectData.monsterEffectData.monsterType == MonsterEffectData.MonsterType.Evolution;

                    case DetailCardType.AdventMonster:
                        return target.cardData.effectData.monsterEffectData.monsterType == MonsterEffectData.MonsterType.Advent;
                }
            }
            else return false;
        }
        else if (targetCardType.GetRoughCardType() == Card.CardType.Item)
        {
            if (target.cardType == Card.CardType.Item)
            {
                switch (targetCardType)
                {
                    case DetailCardType.Item:
                        return true;

                    case DetailCardType.NormalItem:
                        return target.cardData.effectData.itemEffectData.itemType == ItemEffectData.ItemType.Normal;

                    case DetailCardType.Weapon:
                        return target.cardData.effectData.itemEffectData.itemType == ItemEffectData.ItemType.Weapon;

                    case DetailCardType.Eternal:
                        return target.cardData.effectData.itemEffectData.itemType == ItemEffectData.ItemType.Eternal;

                    case DetailCardType.Field:
                        return target.cardData.effectData.itemEffectData.itemType == ItemEffectData.ItemType.Field;
                }
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
