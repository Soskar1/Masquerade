using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckModel
{
    private readonly List<CardData> m_cards;

    public DeckModel(List<CardData> cards) => m_cards = cards;

    public CardModel DrawCard()
    {
        CardData cardData = m_cards[Random.Range(0, m_cards.Count)];

        int value = Random.Range(0, System.Enum.GetValues(typeof(CardColor)).Length);
        CardColor color = (CardColor) value;

        List<ModifierModel> modifiers = GenerateRandomModifiers(
            allowNone: true,
            maxModifiers: 3,
            allowSameColorAsCard: false,
            cardColor: color
        );

        return new CardModel(cardData, color, modifiers);
    }

    public void Add(CardData cardData) => m_cards.Add(cardData);

    public static List<ModifierModel> GenerateRandomModifiers(
    bool allowNone,
    int maxModifiers,
    bool allowSameColorAsCard,
    CardColor cardColor)
    {
        // Decide if the card has modifiers at all
        if (allowNone)
        {
            // 50% chance to have no modifiers (tweak this probability as you like)
            if (UnityEngine.Random.value < 0.65f)
                return new List<ModifierModel>();
        }

        // Build candidate colors
        List<CardColor> availableColors = new List<CardColor>(
            (CardColor[])Enum.GetValues(typeof(CardColor))
        );

        if (!allowSameColorAsCard)
            availableColors.Remove(cardColor);

        // Clamp maxModifiers to how many colors we can pick from
        int maxPossible = Mathf.Min(maxModifiers, availableColors.Count);

        // Pick how many modifiers this card will get (1..maxPossible)
        int count = UnityEngine.Random.Range(1, maxPossible + 1);

        // Pick unique colors without duplicates
        var modifiers = new List<ModifierModel>(count);

        for (int i = 0; i < count; i++)
        {
            int idx = UnityEngine.Random.Range(0, availableColors.Count);
            CardColor modColor = availableColors[idx];
            availableColors.RemoveAt(idx); // ensures uniqueness

            int percentage = UnityEngine.Random.Range(1, 21) * 5;

            modifiers.Add(new ModifierModel(modColor, percentage));
        }

        return modifiers;
    }
}
