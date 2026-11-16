using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Choices : Effect
{
    [Header("効果ナゲットの選択肢")] public List<EffectNugget> effectNuggetChoices = new List<EffectNugget>();
    [Header("選択するのが自分かどうか")] public bool doIChoose = true;

    private void Reset()
    {
        effectType = EffectTypes.ChooseEffect;
    }
}
