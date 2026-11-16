using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareCardCondition : TargetOtherCondition
{
    [Header("参照するカード")] public TotalTargetDecideNuggetMB referenceCardCondition;
    [Header("対象と同じか異なるか")] public bool equalCondition;
    [Header("比較する要素")] public CompareElement compareElement;

    public enum CompareElement
    {
        Attribute,
        Name,
        Category,
        CardType,
        DetailCardType
    }



    public override bool DoesTargetFulfillConditions(Card userCard ,Card compareCard)
    {
        foreach (Card card in referenceCardCondition.decideNugget.SearchTotalTargets(userCard))
        {
            bool isEqual = false;
            switch (compareElement)
            {
                case CompareElement.Attribute:
                    isEqual = compareCard.attribute == card.attribute;
                    break;

                case CompareElement.Name:
                    isEqual = compareCard.cardData.cardName == card.cardData.cardName;
                    break;

                case CompareElement.Category:
                    foreach (Card.Category category in card.categories)
                    {
                        if (compareCard.DoCategoriesMeetCondition(category))
                        {
                            isEqual = true;
                        }
                    }
                    break;

                case CompareElement.DetailCardType://未テスト
                    isEqual = compareCard.cardData.GetDetailCardType() == card.cardData.GetDetailCardType();
                    break;
            }

            if(isEqual != equalCondition)
            {
                return false;
            }
        }
        return true;
    }
}
