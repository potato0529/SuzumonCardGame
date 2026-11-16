using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CPUAgentから思考要請を受け、状況と戦略方針をもとに行動を選択する
/// </summary>
public abstract class CpuStrategy
{
    protected CpuCardSelector cpuCardSelector = new CpuCardSelector();

    public abstract PlayerCommand DecideCommand(Player cpuPlayer);

    public bool HasAvailableCommands(Player cpuPlayer)
    {
        return cpuPlayer.handScr.GetAvailableHandCard().Count > 0 || cpuPlayer.fieldCards.GetCanAttackMonsters().Count > 0 || cpuPlayer.fieldCards.GetCanDefenceMonsters().Count > 0 || cpuPlayer.fieldCards.GetCanAdventMonsters().Count > 0; 
    }

    /// <summary>
    /// 与えられた中からStrategyに基づいて選ぶべきカードのIDを返す。※ここでは実際のカード選択処理はせず、単に選ぶべきものを返すだけ）
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="selectType"></param>
    /// <returns></returns>
    public abstract int SelectCardId(List<Card> cards, CardPointer.SelectType selectType);

    /// <summary>
    /// 与えられた中からStrategyに基づいて使用すべきカードのIDを返す。af:現在「使用」はCardPointerにおける「Select」に該当しないのでSelectTypeがなく、SelectCardId()に含めることはできなかったが、ほぼ同じ処理で他のSelectTypeが増えたような感覚。この周辺は疎結合なのでここだけルートが変わってもほとんど影響はなさそうだが、どちらに含めるのが適切かは悩む
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public abstract int SelectUseCardId(List<Card> cards);
    
    /// <summary>
    /// YesNoどちらかを状況を判断して決める。※ここでは実際のYesNo決定処理はせず、単に選ぶべきものを返すだけ
    /// </summary>
    /// <param name="cpuPlayer"></param>
    /// <returns></returns>
    public abstract bool DecideYesNo(Player cpuPlayer);

    /// <summary>
    /// 与えた選択効果の選択肢の中から、効果の内容と状況を加味して選択効果を1つ決める。※ここでは実際の選択処理はせず、単に選ぶべきものを返すだけ）
    /// </summary>
    /// <param name="choiceNum"></param>
    /// <returns></returns>
    public abstract AEffectChoice DecideEffectChoice(List<AEffectChoice> effectChoices);
}
