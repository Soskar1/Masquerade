using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private HandPresenter m_playerHand;
    [SerializeField] private HandPresenter m_enemyHand;

    [SerializeField] private List<CardData> m_cardPool;
    [SerializeField] private DeckModel m_deckModel;

    public void Awake()
    {
        DeckModel deck = new DeckModel(m_cardPool);

        HandModel playerHandModel = new HandModel(deck, 5);
        m_playerHand.Initialize(playerHandModel);

        HandModel enemyHandModel = new HandModel(deck, 5);
        m_enemyHand.Initialize(enemyHandModel, hideCards: true, reactToMouseInput: false);

        playerHandModel.DrawCards();
        enemyHandModel.DrawCards();
    }
}
