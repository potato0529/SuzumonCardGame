using UnityEngine;
using Unity.Services.Analytics;

public class ExpEvent : Unity.Services.Analytics.Event
{
    public ExpEvent() : base("gainExp")
    {

    }

    public int Exp
    {
        set => SetParameter("exp", value);
    }
}
