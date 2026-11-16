using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ability : MonoBehaviour,IUsedTargetResult
{
    [Header("ターゲット数を満たさずに決定してもよいか")] public bool allowBelowTargets;

    [HideInInspector] public Card userCard;
    public VariableInt mainValue;
    [HideInInspector] public NValueGenerator tempValueGenerator;
    [HideInInspector] public int usedMainValue;
    [HideInInspector] public int usedConditionNuggetValue;
    [HideInInspector] public int targetedCardsValue; //使われた効果のターゲット数

    #region 特定の効果でのみ必要な効果情報
    [Header("プレイヤー対象")] public TargetCardConditions.TargetOwners targetPlayer;
    [HideInInspector] public Effect subEffectInfo; //効果の中で必要な効果情報
    [Header("ステータスタイプ")] public StatusData.StatusType statusType;
    [Header("効果で参照するEffectNugget")] public EffectNugget referenceEffectNugget;
    [HideInInspector] public List<Card> targetedCards = new List<Card>(); //使われた効果のターゲット
    public List<Card> secondTargetedCards = new List<Card>();
    [Header("拡張プロパティ")] public ExpandProperties expandProperties;
    #endregion

    [Header("ターゲットの絞り方")] public TargetFocusType focusType;
    [Header("ターゲット絞り込み値")] public VariableInt focusValue;
    [Header("ターゲットの決定")] public TotalTargetDecideNugget targetDecideNugget;
    [Header("ターゲットの絞り方2")] public TargetFocusType focusType2;
    [Header("ターゲット絞り込み値2")] public VariableInt focusValue2;
    [Header("もう1種のターゲット条件(Select以外)")] public TotalTargetDecideNugget secondDecideNugget;
    

    public enum TargetFocusType
    {        
        Select,
        All,
        Random
    }

    /// <summary>
    /// 1回の発動ごとのデータを初期化
    /// </summary>
    public void ResetParam()
    {
        usedMainValue = 0;
        targetedCards = new List<Card>();
        usedConditionNuggetValue = 0;
        targetedCardsValue = 0;
    }

    public void SerializableChangeTemp()
    {
        //tempValueGenerator.operatorUnits = mainValue.valueGenerator.operatorUnits;
        //tempValueGenerator.firstValue = mainValue.valueGenerator.firstValue;
        //targetDecideNugget.SerializableChange();
    }

    public void SerializableChange()
    {
        //mainValue.valueGenerator.operatorUnits = tempValueGenerator.operatorUnits;
        //mainValue.valueGenerator.firstValue = tempValueGenerator.firstValue;
    }

    public List<Card> GetTargetedResultCards()
    {
        return targetedCards;
    }
}