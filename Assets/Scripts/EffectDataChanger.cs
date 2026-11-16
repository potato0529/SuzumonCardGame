using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDataChanger : MonoBehaviour
{
    [Header("変換したいカード")] public List<CardData> changeCards = new List<CardData>();

    [ContextMenu(nameof(ChangeData))]
    public void ChangeData()
    {
        foreach(CardData card in changeCards)
        {
            //for (int loops = 0; card.effectNuggetsParent.transform.childCount > loops; loops++)
            //{
            //    Debug.Log("おっちゃん参戦");
            //    GameObject childObj = card.effectNuggetsParent.transform.GetChild(loops).gameObject;
            //    GenericEffectNugget childsEffectNugget = childObj.GetComponent<GenericEffectNugget>();
            //    if (childsEffectNugget != null)
            //    {
            //        Debug.Log("おっちゃんサイなら");
            //        foreach(Effect effect in childsEffectNugget.effects)
            //        {
            //            Debug.Log("やっぱりさみしい");
            //            effect.tempValueGenerator.operatorUnits = effect.mainValue.valueGenerator.operatorUnits;
            //            effect.tempValueGenerator.firstValue = effect.mainValue.valueGenerator.firstValue;
            //        }
            //    }
            //}
        }
    }
}
