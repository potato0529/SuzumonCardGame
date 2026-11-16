using System;
public class TriggerInfo
{
    public bool IsSurvive
    {
        get;
    }

    public Action Action
    {
        get;
    }
    
    public TriggerInfo(Action action)
    {
        Action = action;
    }

    public TriggerInfo(Action action, bool isSurvive) : this(action)
    {
        IsSurvive = isSurvive;
    }
}
