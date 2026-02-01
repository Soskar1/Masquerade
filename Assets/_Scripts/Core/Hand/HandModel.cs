using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class HandModel
{
    private int m_maxSize;
    private List<CardModel> m_cards;
    private readonly DeckModel m_deck;

    public event EventHandler<CardModel> OnCardAdded;
    public event EventHandler<OnCardRemovedEventArgs> OnCardRemoved;

    public List<CardModel> Cards => m_cards;

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

    public async Task DrawCards()
    {
        for (int i = m_cards.Count; i < m_maxSize; ++i)
        {
            CardModel card = m_deck.DrawCard();
            Add(card);
            await Task.Delay(100);
        }
    }

    public void RemoveCard(CardModel card, bool deleteFromTheGame = false)
    {
        m_cards.Remove(card);
        OnCardRemovedEventArgs args = new OnCardRemovedEventArgs(card, deleteFromTheGame);
        OnCardRemoved?.Invoke(this, args);
    }

    public void Clear()
    {
        int size = m_cards.Count;
        for (int i = 0; i < size; ++i)
        {
            CardModel model = m_cards.First();
            RemoveCard(model, true);
        }
    }

    public void IncreaseMaxSize() => m_maxSize++;
}
