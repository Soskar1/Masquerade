using System.Collections.Generic;
using UnityEngine;

public class DeckModel
{
    private readonly List<CardData> m_cards;

    public DeckModel(List<CardData> cards) => m_cards = cards;

    public CardModel DrawCard()
    {
        CardData cardData = m_cards[Random.Range(0, m_cards.Count)];

        int value = Random.Range(0, System.Enum.GetValues(typeof(CardColor)).Length);
        CardColor color = (CardColor) value;

        return new CardModel(cardData, color);
    }

    public void Add(CardData cardData) => m_cards.Add(cardData);
}
