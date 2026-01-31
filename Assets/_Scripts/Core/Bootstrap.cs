using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private HandPresenter m_handPresenter;
    [SerializeField] private List<CardData> m_cardPool;
    [SerializeField] private DeckModel m_deckModel;

    public void Awake()
    {
        DeckModel deck = new DeckModel(m_cardPool);

        HandModel handModel = new HandModel(deck, 5);
        m_handPresenter.Initialize(handModel);

        handModel.DrawCards();
    }
}
