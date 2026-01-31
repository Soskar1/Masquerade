using System;
using System.Collections.Generic;

public class HandModel
{
    private int m_maxSize;
    private List<CardModel> m_cards;
    private readonly DeckModel m_deck;

    public event EventHandler<List<CardModel>> OnHandChanged;

    public HandModel(DeckModel deck, int maxSize)
    {
        m_maxSize = maxSize;
        m_cards = new List<CardModel>(m_maxSize);
        m_deck = deck;
    }

    public void DrawCards()
    {
        List<CardModel> cards = new List<CardModel>();

        for (int i = 0; i < m_maxSize; ++i)
        {
            CardModel card = m_deck.DrawCard();
            cards.Add(card);
        }

        m_cards = cards;
        OnHandChanged?.Invoke(this, m_cards);
    }
}
