public class GivedStatus
{
    public Effect sourceEffect;
    public int givedStatusValue;
    public int decreasedValue;

    public GivedStatus(Effect effect,Card card)
    {
        sourceEffect = effect;
        givedStatusValue = effect.mainValue.GetValue(card);
    }
}
