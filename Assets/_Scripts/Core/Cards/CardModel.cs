using System;

public class CardModel
{
    public CardData CardData { get; private set; }

    private int m_currentScore;
    public int CurrentScore
    {
        get => m_currentScore;
        set
        {
            if (value < 0) value = 0;

            m_currentScore = value;
            OnScoreChanged?.Invoke(this, m_currentScore);
        }
    }

    public event EventHandler<int> OnScoreChanged;

    public CardModel(CardData data)
    {
        CardData = data;
        CurrentScore = data.Score;
    }
}