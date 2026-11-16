using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalysisIcon : StatusIcon
{
    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (statusIconsManager.card.cardStatuses.statusesDictionary.ContainsKey(StatusData.StatusType.Paralysis))//pin status
        {
            EnableIcon();
        }
        else
        {
            DisableIcon();
        }
    }
}
