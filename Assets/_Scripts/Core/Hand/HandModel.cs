using System;
using System.Collections.Generic;
using System.Linq;

public class HandModel
{
    private int m_maxSize;
    private List<CardModel> m_cards;

    public event EventHandler<List<CardModel>> OnHandChanged;

    public HandModel(int maxSize)
    {
        m_maxSize = maxSize;
        m_cards = new List<CardModel>();
    }

    public void DrawCards(List<CardModel> cards)
    {
        m_cards = cards.Take(m_maxSize).ToList();
        OnHandChanged?.Invoke(this, m_cards);
    }
}
