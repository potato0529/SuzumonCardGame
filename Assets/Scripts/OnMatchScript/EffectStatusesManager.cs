using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStatusesManager : MonoBehaviour
{
    public static EffectStatusesManager instance = null;
    [Header("スズモンに存在するアイコン(StatusTypeEnum)")] public StatusIcon[] effectStatusIcons;

    private void Awake()
    {
        instance = this;
    }
}
