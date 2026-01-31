using System;
using System.Collections.Generic;

public class HandModel
{
    private int m_maxSize;
    private List<CardModel> m_cards;
    private readonly DeckModel m_deck;

    public event EventHandler<CardModel> OnCardAdded;
    public event EventHandler<CardModel> OnCardRemoved;

    public HandModel(DeckModel deck, int maxSize)
    {
        m_maxSize = maxSize;
        m_cards = new List<CardModel>(m_maxSize);
        m_deck = deck;
    }

    public void Add(CardModel card)
    {
        m_cards.Add(card);
        OnCardAdded?.Invoke(this, card);
    }

    public void DrawCards()
    {
        for (int i = 0; i < m_maxSize; ++i)
        {
            CardModel card = m_deck.DrawCard();
            Add(card);
        }
    }

    public void RemoveCard(CardModel card)
    {
        m_cards.Remove(card);
        OnCardRemoved?.Invoke(this, card);
    }
}
