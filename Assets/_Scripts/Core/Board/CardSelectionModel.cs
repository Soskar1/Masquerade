using System.Collections.Generic;

public class CardSelectionModel
{
    private List<CardModel> m_selectedCards;

    public CardSelectionModel()
    {
        m_selectedCards = new List<CardModel>();
    }

    public void Add(CardModel card) => m_selectedCards.Add(card);
    public void Remove(CardModel card) => m_selectedCards.Remove(card);
}
