using System;
using System.Collections.Generic;
using System.Linq;

public class BoardModel
{
    private List<CardModel> m_selectedCards;

    public List<CardModel> SelectedCards => m_selectedCards;

    public event EventHandler<CardModel> OnCardAdded;
    public event EventHandler<OnCardRemovedEventArgs> OnCardRemoved;

    public BoardModel()
    {
        m_selectedCards = new List<CardModel>();
    }

    public void Add(CardModel card)
    {
        m_selectedCards.Add(card);
        OnCardAdded?.Invoke(this, card);
    }

    public void Remove(CardModel card, bool deleteFromTheGame = false)
    {
        m_selectedCards.Remove(card);

        OnCardRemovedEventArgs args = new OnCardRemovedEventArgs(card, deleteFromTheGame);
        OnCardRemoved?.Invoke(this, args);
    }

    public void Clear()
    {
        int size = m_selectedCards.Count;
        for (int i = 0; i < size; ++i)
        {
            CardModel model = m_selectedCards.First();
            Remove(model, true);
        }
    }
}
