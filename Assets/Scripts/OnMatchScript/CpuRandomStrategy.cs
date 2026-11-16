using UnityEngine;
using System.Collections.Generic;

public class CpuRandomStrategy : CpuStrategy
{
    /// <summary>
    /// 可能であれば優先順位に応じてコマンドを実行する。実行可能あるいはCPUが実行したいと考えるコマンドがなければnullを返す
    /// </summary>
    /// <param name="cpuPlayer"></param>
    /// <returns></returns>
    public override PlayerCommand DecideCommand(Player cpuPlayer)
    {
        List<Card> canUseHandCard = cpuPlayer.handScr.GetAvailableHandCard();
        
        if (canUseHandCard.Count>0)
        {
            Card useCard = canUseHandCard[MatchManager.instance.cpuRandom.Range(0, canUseHandCard.Count)];
            return new UseCardNormalCommand(cpuPlayer, MatchManager.instance.GetCardId(useCard), cpuPlayer.fieldCardPlacement.Placed(useCard));
        }

        List<Card> fieldCardUsableEffect = cpuPlayer.fieldCards.GetFieldCardsUsableEffect();
        if (fieldCardUsableEffect.Count > 0)
        {
            return new EffectButtonCommand(cpuPlayer, MatchManager.instance.GetCardId(cpuCardSelector.SelectRandomCard(fieldCardUsableEffect)));
        }

        List<AdventMonster> canAdventMonsters = cpuPlayer.fieldCards.GetCanAdventMonsters();
        if(canAdventMonsters.Count > 0)
        {
            AdventMonster adventMonster = canAdventMonsters[ MatchManager.instance.cpuRandom.Range(0, canAdventMonsters.Count)];
            return new AdventButtonCommand(cpuPlayer, MatchManager.instance.GetCardId(adventMonster.monster.card));
        }
        else
        {
            //アタック/ディフェンスできるカードがあれば、ランダムにどちらかを実行
            int validCount = 0;//条件を満たしている個数カウント

            int attackIndex = cpuPlayer.fieldCards.GetCanAttackMonsters().Count > 0 ? validCount++ : -1;
            int defenceIndex = cpuPlayer.fieldCards.GetCanDefenceMonsters().Count > 0 ? validCount++ : -1;

            //満たした条件が1つ以上あれば、その中からランダムにインデックスを選択
            if (validCount > 0)
            {
                int chosenIndex = MatchManager.instance.cpuRandom.Range(0, validCount);

                if (chosenIndex == attackIndex)
                {
                    List<Card> canAttackMonsterCards = cpuPlayer.fieldCards.GetCanAttackMonsters().ToCards();
                    Card attackerCard = cpuCardSelector.SelectRandomCard(canAttackMonsterCards);
                    return new StartAttackCommand(cpuPlayer, MatchManager.instance.GetCardId(attackerCard));
                }
                else if (chosenIndex == defenceIndex)
                {
                    List<Card> canDefenceMonsterCards = cpuPlayer.fieldCards.GetCanDefenceMonsters().ToCards();
                    Card defencerCard = cpuCardSelector.SelectRandomCard(canDefenceMonsterCards);
                    return new DefenceCommand(cpuPlayer, MatchManager.instance.GetCardId(defencerCard));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }

    public override int SelectCardId(List<Card> cards, CardPointer.SelectType selectType)
    {
        switch (selectType)
        {
            case CardPointer.SelectType.none:
            case CardPointer.SelectType.ability:
            case CardPointer.SelectType.equip:
            case CardPointer.SelectType.noroi:
                return MatchManager.instance.GetCardId(cpuCardSelector.SelectRandomCard(cards));

            case CardPointer.SelectType.attack:
                if (MatchManager.instance.cpuRandom.Range(0, 2) == 0)
                {
                    //-1の場合SelectableTarget(相手プレイヤーへ直接攻撃）を表す
                    return -1;
                }
                else
                {
                    return MatchManager.instance.GetCardId(cpuCardSelector.SelectRandomCard(cards));
                }

            case CardPointer.SelectType.Advent:
                if (MatchManager.instance.cpuRandom.Range(0, 2) == 0)
                {
                    //-2の場合SelectableTarget(自分ライフコスト）を表す
                    return -2;
                }
                else
                {
                    return MatchManager.instance.GetCardId(cpuCardSelector.SelectRandomCard(cards));
                }

        }
        //-100は何にも該当しない
        return -100;
    }

    public override int SelectUseCardId(List<Card> cards)
    {
        return MatchManager.instance.GetCardId(cpuCardSelector.SelectRandomCard(cards));
    }

    public override bool DecideYesNo(Player cpuPlayer)
    {
        return true;
    }

    public override AEffectChoice DecideEffectChoice(List<AEffectChoice> effectChoices)
    {
        return effectChoices[MatchManager.instance.cpuRandom.Range(0, effectChoices.Count)];
    }
}
