using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithstandIcon : StatusIcon
{
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// ”ğ‚¯‚é‚ğ’Ç‰Á‚·‚éŒø‰Ê‚ª‚ ‚Á‚½ê‡‚±‚±‚É“`‚¦‚é‚Ì•K{¦‚à‚Æ‚à‚Æ”ğ‚¯‚é“z‚ª‚»‚ÌŒø‰Êó‚¯‚½‚ç‚Ü‚ ‚È‚ñ‚â‚©‚ñ‚â
    /// </summary>
    public void AddWithstandScript(Withstand thisWithstand)
    {
        if (status == null)//pin !status
            status = thisWithstand;
    }

    public void RemoveWithstandScript()
    {
        status = null;
    }
}
