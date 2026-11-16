using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EvolutionNugget : MonoBehaviour,IPassTarget,IInEffectText
{
    [Header("必要進化素材")] public TargetCardConditionsNugget[] materialConditionsNuggets;

    private Card[] decidedMaterials;

    private int nowMaterialNum;

    public Card card { get; set; }

    private Action callbackAction;

    private void Start()
    {
        card.evolutionNugget = this;
        decidedMaterials = new Card[materialConditionsNuggets.Length];
    }

    /// <summary>
    /// コールバックしたいとき用のUseEffectOrder
    /// </summary>
    /// <param name="caller">ICompProcessでコールバックするオブジェクト</param>
    public void EvolutionOrder(Action action)
    {
        callbackAction = action;
        if (CanEvolutionCheck())
        {
            StartCoroutine(ChooseEvoMaterial());
        }
        else
        {
            UtilityLog.CardLog("進化素材が足りません", card, UtilityLog.LogCategory.none);
        }
    }
    
    /// <summary>
    /// 進化素材がそろっているかチェックする
    /// </summary>
    /// <returns></returns>
    public bool CanEvolutionCheck()
    {
        //それぞれの素材モンスターの条件を満たしたモンスターがいるか調べる
        byte materialsNum = 0;
        //この仕組みは、ある進化素材の条件の範囲をA、その前に選ぶ素材の条件をBとしたときB⊃Aでなければならない。これを変えたければ頑張るしかない。またはその条件をなくし重複可にする
        List<Card> meetConditionCards = new List<Card>();
        foreach(TargetCardConditionsNugget materialConditionNugget in materialConditionsNuggets)
        {
            materialsNum++;
            //進化なのでコストにできないモンスターはカウントしないようにする（今後もっと汎用的にこの処理を適用する可能性はある）ExistTargetで正しく検知するためにselectInfoで条件追加してるのに実際の進化処理のSelectCardのselectType:costを指定して結局条件がかぶってるからやっぱり処理きもいな
            if (materialConditionNugget.GetMeetConditionsCards(card, (card) => { return !card.cantCost; }, (card) => { return !meetConditionCards.Contains(card); }).Count > 0)
            {
                meetConditionCards.Add(materialConditionNugget.GetMeetConditionsCards(card, (card) => { return !card.cantCost; }, (card) => { return !meetConditionCards.Contains(card); })[0]);
                UtilityLog.CardLog("素材" + materialsNum + "の条件を満たしたモンスター:", card, UtilityLog.LogCategory.none);
                continue;
            }
            Debug.Log("素材" + materialsNum + "の条件を満たしたモンスターが見つからなかった為、進化できません");
            return false;
        }
        UtilityLog.CardLog("進化が可能です！", card, UtilityLog.LogCategory.none);
        return true;
    }

    public IEnumerator ChooseEvoMaterial()
    {
        UtilityLog.CardLog("進化素材を選びます", card, UtilityLog.LogCategory.none);

        nowMaterialNum = 0;
        //そもそも進化処理が複数の選択処理をまたいでいるせいで既に素材とされているカードが選択できてしまう問題は後回し
        foreach (TargetCardConditionsNugget conditionNugget in materialConditionsNuggets)
        {
            
            Debug.Log((nowMaterialNum + 1) + "個目の素材を選択中...");
            List<Card> selectableMaterials = conditionNugget.GetMeetConditionsCards(card, (card) => { return !card.cantCost; });

            //新しい選択処理案の解釈を拡張してここでも無理やり同じような処理をしても大丈夫なことになっています。既に選ばれている素材を選択済みとする
            foreach(Card targetedMaterial in decidedMaterials)
            {
                if (targetedMaterial != null && selectableMaterials.Contains(targetedMaterial))
                    selectableMaterials.Remove(targetedMaterial);
            }

            card.controlerPlayer.cardPointer.SelectCard(1,false, selectableMaterials, CardPointer.SelectType.none,PassTargets,nowMaterialNum + 1 + "枚目の進化素材"); //warn:たまたまコルーチンで1fのディレイがあるから大丈夫だったけどPassTargetが来た時と同じ処理タイミングで次の素材の選択が始まってしまうと処理の重複が起きてエラーになってしまう
            //指定された枠の素材が決定されて埋まるまで待つ

            while (!decidedMaterials[nowMaterialNum])
            {
                yield return null;
            }
            Debug.Log((nowMaterialNum + 1) + "個目の素材が決定");
            nowMaterialNum++;
        }

        //evolutionMonsterにdecidedMaterialsを重ねる処理的な
        foreach(Card material in decidedMaterials)
        {
            //仮
            material.selectableTarget.targeted = false;
            Debug.Log("素材「" + material + "」を重ねます");
            card.cardLayer.PileUnderThis(material);
            
        }

        decidedMaterials = new Card[materialConditionsNuggets.Length];

        callbackAction.Invoke();

        //List<List<Card>> sourceEachConditionsSelectableMaterials = new List<List<Card>>();
        
        //for(int i=0;i<sourceEachConditionsSelectableMaterials.Count;i++)
        //{
        //    sourceEachConditionsSelectableMaterials[i] = materialConditionsNuggets[i].GetMeetConditionsCards(card, (card) => { return !card.cantCost; });
        //}

        //List<List<Card>> eachConditionsSelectableMaterials = new List<List<Card>>(sourceEachConditionsSelectableMaterials);

        ////全条件のターゲットを合計し、重複は取り除く。
        //List<Card> allSelectableTargets = new List<Card>();
        //foreach(List<Card> cards in eachConditionsSelectableMaterials)
        //{
        //    allSelectableTargets.AddRange(cards);
        //}

        ////それぞれのカードがいくつの条件に当てはまっているのか管理
        //Dictionary<Card, int> allTargetsMeetConditionValue = new Dictionary<Card, int>();
        //foreach(Card selectableTarget in allSelectableTargets)
        //{
        //    if (allTargetsMeetConditionValue.ContainsKey(selectableTarget))
        //    {
        //        allTargetsMeetConditionValue[selectableTarget] += 1;
        //    }
        //    else
        //    {
        //        allTargetsMeetConditionValue.Add(selectableTarget, 1);
        //    }
        //}

        ////条件に当てはまる数が最小のターゲットを取得
        //List<Card> minMeetConditionTargets = new List<Card>();
        //int minValue = allTargetsMeetConditionValue.Values.Min();
        //foreach(KeyValuePair<Card,int> selectableTarget in allTargetsMeetConditionValue)
        //{
        //    if(selectableTarget.Value == minValue)
        //    {
        //        minMeetConditionTargets.Add(selectableTarget.Key);
        //    }
        //}

        ////条件に当てはまった数が少ないターゲットを取り除く（同数でも）
        //foreach(List<Card> meetConditionCards in eachConditionsSelectableMaterials)
        //{
        //    foreach (Card minMeetConditionCard in minMeetConditionTargets)
        //    {
        //        meetConditionCards.Remove(minMeetConditionCard);
        //    }
        //}

        //残ったなかで確定させる
        //同数なら、分岐させたのち確定させる
    }

    public void PassTargets(List<Card> targets)
    {
        //ターゲットリストのindex0を追加する　※PassTargets()は共通インターフェースの為Listでしか受け取れないが、進化素材は1つずつ選ぶ処理の為index0のみでよい
        decidedMaterials[nowMaterialNum] = targets[0];//era card[]
        UtilityLog.CardLog("進化素材'" + targets[0] + "'を受け取りました。素材:", targets[0], UtilityLog.LogCategory.none);
    }
}
