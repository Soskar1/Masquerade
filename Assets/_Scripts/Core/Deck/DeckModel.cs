using System.Collections.Generic;
using UnityEngine;

public class DeckModel
{
    private readonly List<CardModel> m_cards;

    public DeckModel(List<CardModel> cards)
    {
        m_cards = cards;
    }

    public CardModel DrawCard() => m_cards[Random.Range(0, m_cards.Count - 1)];
}
