using System;
public interface IUseEffect
{
    void UseEffectOrder();
    void UseEffectOrder(Action callback);
    void AddTriggerQueue();
    void CanUseCheck();
}
