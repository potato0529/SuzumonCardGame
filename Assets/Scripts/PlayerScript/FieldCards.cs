using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FieldCards : MonoBehaviour
{
    [Header("持ち主")] public Player pScr;
    [Header("フィールドカード")] public List<Card> cards; 

    public List<Monster> GetFieldMonsters()
    {
        List<Monster> monsters = new List<Monster>();
        foreach(Card card in cards)
        {
            if (card.monsterScr)
            {
                monsters.Add(card.monsterScr);
            }
        }
        return monsters;
    }

    public void TurnEnd()
    {
        foreach(Card card in cards)
        {
            if (card.effectNuggets[((byte)EffectNugget.InvokeTiming.MyTurn)])
            {
                //ターン内の毎ターン効果使用回数をリセットする
                EffectNugget actPhaseEffectNugget = card.effectNuggets[((byte)EffectNugget.InvokeTiming.MyTurn)];
                actPhaseEffectNugget.SetTurnUsedNum(0);
            }
        }
    }

    public bool ExistLifeBlocker()
    {
        Debug.Log("ライフブロッカーを探します");
        foreach(Monster monster in GetFieldMonsters())
        {
            EffectNugget lifeblockerEffect = monster.card.effectNuggets[((byte)EffectNugget.InvokeTiming.LifeBlock)];
            if (lifeblockerEffect && lifeblockerEffect.GetComponent<LifeBlocker>() && lifeblockerEffect.GetComponent<LifeBlocker>().lifeBlockerValue > 0 && !monster.card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Preserved))
            {
                return true;
            }
        }
        return false;
    }

    public List<Card> GetMeetConditionFieldCard(Func<Card, bool> expandTargetCondition)
    {
        List<Card> meetConditionCards = new List<Card>();
        foreach(Card card in cards)
        {
            if (expandTargetCondition(card))
            {
                meetConditionCards.Add(card);
            }
        }
        return meetConditionCards;
    }

    /// <summary>
    /// 場の利用可能なカードをハイライトする
    /// </summary>
    public void HighlightAvailableCard()
    {
        foreach (Card card in cards)
        {
            if (card.monsterScr && (card.monsterScr.attack.CanAttack() || card.monsterScr.CanDefence()))
            {
                card.matchCardHighlight.HighlightBorderEnable(true);
                card.matchCardHighlight.ChangeHighlightBorderColor(new Color(1, 0.6f, 0, 1));
            }

            EffectNugget myTurnEffect = card.effectNuggets[(int)EffectNugget.InvokeTiming.MyTurn];
            if (myTurnEffect && myTurnEffect.CanUseCheck())
            {
                card.matchCardHighlight.HighlightBorderEnable(true);
                if (card.monsterScr)
                {
                    card.matchCardHighlight.ChangeHighlightBorderColor(new Color(1, 0.6f, 0, 1));
                }
                else
                {
                    card.matchCardHighlight.ChangeHighlightBorderColor(new Color(0.3f, 1, 0, 1));
                }
            }
        }
    }

    /// <summary>
    /// 使用可能カードのハイライトを消す
    /// </summary>
    public void RemoveHighlights()
    {
        foreach (Card card in cards)
        {
            card.matchCardHighlight.HighlightBorderEnable(false);
        }
    }

    /// <summary>
    /// フィールド効果が使用可能なカードを取得する
    /// </summary>
    public List<Card> GetFieldCardsUsableEffect()
    {
        List<Card> activatableEffectCards = new List<Card>();

        foreach(Card card in cards)
        {
            if (card.CanUseMyTurnEffect())
            {
                activatableEffectCards.Add(card);
            }
        }
        return activatableEffectCards;
    }

    /// <summary>
    /// Attackできるフィールドのモンスターを取得する
    /// </summary>
    /// <returns></returns>
    public List<Monster> GetCanAttackMonsters()
    {
        List<Monster> canAttackMonsters = new List<Monster>();

        foreach (Monster monster in GetFieldMonsters())
        {
            if (monster.attack.CanAttack())
            {
                canAttackMonsters.Add(monster);
            }
        }

        return canAttackMonsters;
    }

    /// <summary>
    /// Defenceできるフィールドのモンスターを取得する
    /// </summary>
    /// <returns></returns>
    public List<Monster> GetCanDefenceMonsters()
    {
        List<Monster> canDefenceMonsters = new List<Monster>();

        foreach (Monster monster in GetFieldMonsters())
        {
            if (monster.CanDefence())
            {
                canDefenceMonsters.Add(monster);
            }
        }

        return canDefenceMonsters;
    }

    /// <summary>
    /// 降臨できるフィールドのモンスターを取得する
    /// </summary>
    /// <returns></returns>
    public List<AdventMonster> GetCanAdventMonsters()
    {
        List<AdventMonster> canAdventMonsters = new List<AdventMonster>();

        foreach (Monster monster in GetFieldMonsters())
        {
            AdventMonster adventMonster = monster.adventMonster;

            if (adventMonster && !adventMonster.Advented && adventMonster.CanAdventCheck())
            {
                canAdventMonsters.Add(adventMonster);
            }
        }

        return canAdventMonsters;
    }

}
