using System;
using System.Collections.Generic;

public class BoardModel
{
    private List<CardModel> m_selectedCards;

    public event EventHandler<CardModel> OnCardAdded;

    public BoardModel()
    {
        m_selectedCards = new List<CardModel>();
    }

    public void Add(CardModel card)
    {
        m_selectedCards.Add(card);
        OnCardAdded?.Invoke(this, card);
    }

    public void Remove(CardModel card)
    {
        m_selectedCards.Remove(card);
    }
}
