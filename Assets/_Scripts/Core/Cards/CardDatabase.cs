using System.Collections.Generic;
using UnityEngine;

public class CardDatabase
{
    private List<CardData> m_cards;

    public CardDatabase(List<CardData> cards)
    {
        m_cards = cards;
    }

    public CardData GetRandomCard()
    {
        return m_cards[Random.Range(0, m_cards.Count)];
    }
}
