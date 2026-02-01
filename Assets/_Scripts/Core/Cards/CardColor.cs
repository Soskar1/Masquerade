using UnityEngine;

public enum CardColor
{
    Red,
    Green,
    Blue,
    Yellow
}

public static class CardColorExtensions
{
    public static Color GetColorCode(this CardColor color)
    {
        switch (color)
        {
            case CardColor.Red: return new Color(0.918f, 0.31f, 0.212f);
            case CardColor.Green: return new Color(0.569f, 0.859f, 0.412f);
            case CardColor.Yellow: return new Color(0.976f, 0.761f, 0.169f);
            case CardColor.Blue: return new Color(0.561f, 0.827f, 1);
            default: return Color.black;
        }
    }
}