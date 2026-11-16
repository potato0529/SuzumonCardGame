using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetValueCondition : TargetOtherCondition
{
    [Header("カードのどの値をとるか")] public ConditionType conditionType;
    [Header("ステータスタイプ")] public StatusData.StatusType statusType; //conditionTypeがStatusのときに使用
    [Header("比較")] public ConditionCommon.EqualSigns equalSigns;
    [Header("条件値")] public VariableInt value;
    

    public enum ConditionType
    {
        Rank,
        Status,
        LifeBlocker
    }

    public override bool DoesTargetFulfillConditions(Card userCard,Card target)
    {
        int targetValue = 0;
        switch (conditionType)
        {
            case ConditionType.Rank:
                targetValue = target.rank;
                break;

            case ConditionType.Status:
                if (target.cardStatuses.statusesDictionary.ContainsKey(statusType))
                {
                    targetValue = target.cardStatuses.statusesDictionary[statusType].StatusValue;
                }
                break;

            case ConditionType.LifeBlocker:
                if(target.effectNuggets[(byte)EffectNugget.InvokeTiming.LifeBlock])
                    targetValue = target.effectNuggets[(byte)EffectNugget.InvokeTiming.LifeBlock].GetComponent<LifeBlocker>().lifeBlockerValue;
                break;
        }
        //このままだとrankとかstatusをValueGeneratorで掛けたりして計算できないけどゆうてほぼ使う機会なさそうだし恐らく左辺（こっち）に5掛けたいときは右辺を5で割ればうまくいくみたいな感じで多分何とかなる　でそのときにでてくるのが小数点切り捨て/切り上げして本来差があったものがイコールになってしまう可能性がある問題だけどそれはあとで何とかすることは可能　トリマ困ったら改良するか
        return ConditionCommon.ComparisonValues(equalSigns, targetValue, value.GetValue(userCard));
    }
}
