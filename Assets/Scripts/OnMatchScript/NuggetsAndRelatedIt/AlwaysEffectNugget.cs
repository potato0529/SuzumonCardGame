using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysEffectNugget : MonoBehaviour, IUpdatedGameStatus,IInEffectText
{
    [Header("効果(ステータス)")] public List<Effect> effects = new List<Effect>(); //AddStatusの効果でなきゃあいけない
    [Header("ターゲットの決め方")] public TotalTargetDecideNugget decideNugget;
    [Header("適用条件")] public Condition condition;
    [Header("発動場所")] public Card.CardState invokeCardState = Card.CardState.Field;

    public Card card { get; set; }
    private List<Card> givedStatusCards = new List<Card>(); //この効果によってステータスを与えられたカード

    private void Start()
    {
        card.SetChangedCardStateCB(ChangedCardState);
        MatchManager.instance.UpdatedGameNoticeReceiver.Add(this);
        card.alwaysEffectNugget = this;
    }

    /// <summary>
    /// ゲームの状態が更新されたとき、チェックをして効果を適用する
    /// </summary>
    public void UpdatedGameStatus()
    {
        foreach (Card target in givedStatusCards)
        {
            if (GameManager.instance.logController.statusUpdateInfo)
                UtilityLog.CardLog($"{target} に付与された永続付与ステータスをリセットします", card, UtilityLog.LogCategory.none);
            foreach (Effect effect in effects)
            {
                //エラー
                target.cardStatuses.statusesDictionary[effect.statusType].ChangeGivedStatus(effect,0);
            }
        }

        givedStatusCards.Clear();

        float tKey = Input.GetAxisRaw("TKey");

        //発動場所が正しく、発動条件を満たしているか
        if (card.cardState == invokeCardState && condition.FulfillConditions(card) && !(tKey > 0))
        {
            //全てのEffectsをforeachで回し、それぞれのAddStatusの追加プロパティでEnumかStatusの派生クラスアタッチし、そこから変更するステータスを判断し、AutoTargetで対象を判断し、実行する
            givedStatusCards = decideNugget.SearchTotalTargets(card);
            foreach (Card target in givedStatusCards)
            {
                if(GameManager.instance.logController.statusUpdateInfo)
                    UtilityLog.CardLog($"{target} にステータスを付与(再計算)します", card, UtilityLog.LogCategory.none);
                foreach (Effect effect in effects)
                {
                    target.cardStatuses.SetGivedStatus(effect);
                }
            }
        }
        else
        {
            //UtilityLog.CardLog("常時ナゲットの効果適用条件を満たしません", card, UtilityLog.LogCategory.none);
        }
    }

    public void ChangedCardState(Card.CardState cardState)
    {
        if(invokeCardState == cardState)
        {
            UtilityLog.CardLog($"{card} 正しい発動場所に変更されました", card, UtilityLog.LogCategory.none);
            MatchManager.instance.UpdatedGameNoticeReceiver.Add(this);
        }
        else
        {
            UtilityLog.CardLog($"{card} 正しくない発動場所に変更されました", card, UtilityLog.LogCategory.none);
            MatchManager.instance.UpdatedGameNoticeReceiver.Remove(this);
            foreach(Card target in givedStatusCards)
            {
                foreach (Effect effect in effects)
                {
                    if (target.cardStatuses.statusesDictionary.ContainsKey(effect.statusType)) //warn:即席のエラー対策なので今後対策を考える もしかしたらこれでよい可能性もあり
                        target.cardStatuses.statusesDictionary[effect.statusType].RemoveGivedStatus(effect);
                }
            }
        }
    }
}
