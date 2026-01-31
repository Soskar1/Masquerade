using System;
using System.Collections.Generic;

public class BoardModel
{
    private List<CardPresenter> m_selectedCards;

    public event EventHandler<CardPresenter> OnCardAdded;
    public event EventHandler<CardPresenter> OnCardRemoved;

    public BoardModel()
    {
        m_selectedCards = new List<CardPresenter>();
    }

    public void Add(CardPresenter card)
    {
        m_selectedCards.Add(card);
        OnCardAdded?.Invoke(this, card);
    }

    public void Remove(CardPresenter card)
    {
        m_selectedCards.Remove(card);
        OnCardRemoved?.Invoke(this, card);
    }
}
