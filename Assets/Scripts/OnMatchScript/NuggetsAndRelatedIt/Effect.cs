using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Effect : Ability
{
    [Header("効果の種類")] public EffectTypes effectType;
    [Header("効果の適用条件※//")] public Condition condition;//効果の発動条件とは別に、複数効果があってそれぞれ別の条件を満たさなければ使えない場合に必要。基本は発動条件を設定すればよい
    
    public enum EffectTypes
    {
        none = 0,
        Draw = 1,
        Damage = 2,
        healLife = 3,
        Search = 4,
        Summon = 5,
        DestroyCard = 6,
        SendCemetery = 7,
        Bounce = 8,
        ReturnDeck = 9,
        Noroi = 10,
        Paralysis = 11,
        Preservation = 12,
        Poison = 33,
        AddStatus = 13,
        setAttackDamage = 14,
        changeAttackDamage = 15,
        protect = 16,
        scapegoat = 17,
        PileCard = 18,
        DisableEffect = 19,//
        Hijack = 20,//
        ChangeController = 21,//
        lifeBlocker = 22,
        changeField = 23,
        handdeath = 24,        
        SkipMyTurn = 25,//
        AntiqueBurst = 26,
        RollDice = 27,
        ChooseEffect = 28,
        GiveBarrier = 29,
        ChangeSummonRights = 30,
        ChangeItemRights = 31,
        ReturnSpecialDeck = 32,
        SetLife = 34,
        ChangeVariableParam = 35,
        Choose = 36,
        ForcedSummon = 37,
        ExchangeLife = 38
    }

    public Func<Card,bool> GetEffectExpandCondition(Card userCard)
    {
        switch (effectType)
        {
            case EffectTypes.Summon:
                return (card) => { return !card.useCardCost && !card.evolutionNugget; };

            case EffectTypes.Preservation:
                //return (card) => { return card != userCard; };
                return (card) => { return true; };

            case EffectTypes.GiveBarrier:
                return (card) => { return !card.monsterScr.barrier.GetRemainBarrier(); };
        }
        return (card) => { return true; };
    }
}