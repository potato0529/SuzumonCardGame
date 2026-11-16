using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffectButton : MonoBehaviour
{
    [Header("ÉJÅ[Éh")] public Card card;

    public void Clicked()
    {
        card.EffectButton();
    }
}
