using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private HandPresenter m_handPresenter;
    [SerializeField] private CardData m_data;

    public void Awake()
    {
        HandModel handModel = new HandModel(5);
        m_handPresenter.Initialize(handModel);

        CardModel card = new CardModel(m_data);
        handModel.DrawCards(new List<CardModel>() { card, card, card, card, card });
    }
}
