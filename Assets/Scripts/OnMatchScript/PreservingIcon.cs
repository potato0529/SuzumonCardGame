using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreservingIcon : StatusIcon
{
    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (status != null)//pin status
        {
            EnableIcon();
        }
        else
        {
            DisableIcon();
        }
    }
}
