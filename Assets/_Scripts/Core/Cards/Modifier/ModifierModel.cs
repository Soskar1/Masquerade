public class ModifierModel
{
    public CardColor CardColor { get; private set; }
    public int Percentage { get; private set; }

    public ModifierModel(CardColor cardColor, int percentage)
    {
        CardColor = cardColor;
        Percentage = percentage;
    }
}
