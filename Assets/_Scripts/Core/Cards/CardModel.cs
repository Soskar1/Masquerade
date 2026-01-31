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

    private int m_currentCost;
    public int CurrentCost
    {
        get => m_currentCost;
        set
        {
            if (value < 0) value = 0;

            m_currentCost = value;
            OnCostChanged?.Invoke(this, m_currentCost);
        }
    }

    public event EventHandler<int> OnScoreChanged;
    public event EventHandler<int> OnCostChanged;

    public CardModel(CardData data)
    {
        CardData = data;
        CurrentScore = data.Score;
        CurrentCost = data.Cost;
    }
}