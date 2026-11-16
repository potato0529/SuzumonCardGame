using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : CardButton
{
    public override void Clicked()
    {
        card.CloseButton();
    }
}
