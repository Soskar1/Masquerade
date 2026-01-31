using System.Collections.Generic;
using UnityEngine;

public class DeckModel
{
    private readonly List<CardModel> m_cards;

    public DeckModel(List<CardData> cards)
    {
        m_cards = new List<CardModel>();
        foreach (CardData card in cards)
            m_cards.Add(new CardModel(card));
    }

    public CardModel DrawCard() => m_cards[Random.Range(0, m_cards.Count)];
}
