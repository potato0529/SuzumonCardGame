using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkIcon : StatusIcon
{
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    public override void CheckEnableIcon()
    {
        if (status == null || (status.StatusValue == 1 && status.IsDefault())) //pin !status
        {
            DisableIcon();
        }
        else
        {
            EnableIcon();
        }
    }
}
