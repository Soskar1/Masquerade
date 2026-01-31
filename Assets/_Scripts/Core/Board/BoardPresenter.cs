using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardPresenter : MonoBehaviour
{
    [SerializeField] private RectTransform m_placeholderParent;
    [SerializeField] private CardPresenter m_ghostCardPrefab;
    [SerializeField] private float m_cardMovementDuration;

    private Dictionary<CardPresenter, CardPresenter> m_cardToPlaceholder;

    private BoardModel m_model;
    private HandModel m_hand;
    private ManaModel m_mana;
    private BattleModel m_battle;

    private bool m_canInteractWithCards = false;

    public void Initialize(BoardModel model, HandModel hand, ManaModel mana, BattleModel battle)
    {
        m_model = model;
        m_hand = hand;
        m_mana = mana;
        m_battle = battle;
        model.OnCardAdded += HandleOnCardAdded;
        model.OnCardRemoved += HandleOnCardRemoved;
        m_battle.OnTurnStarted += HandleOnTurnStarted;
        m_battle.OnTurnEnded += HandleOnTurnEnded;

        m_cardToPlaceholder = new Dictionary<CardPresenter, CardPresenter>();
    }

    private void HandleOnTurnStarted(object sender, System.EventArgs e)
    {
        m_canInteractWithCards = true;
    }

    private void HandleOnTurnEnded(object sender, System.EventArgs e)
    {
        m_canInteractWithCards = false;
    }

    private void OnDisable()
    {
        m_model.OnCardAdded -= HandleOnCardAdded;
        m_model.OnCardRemoved -= HandleOnCardRemoved;
        m_battle.OnTurnStarted -= HandleOnTurnStarted;
        m_battle.OnTurnEnded -= HandleOnTurnEnded;
    }

    private void HandleOnCardAdded(object sender, CardModel card)
    {
        CardPresenterRegistry.TryGet(card, out CardPresenter presenter);

        presenter.IsHoverAnimationEnabled = false;

        CardPresenter placeholderInstace = Instantiate(m_ghostCardPrefab, m_placeholderParent);

        m_cardToPlaceholder.Add(presenter, placeholderInstace);
        presenter.transform.SetParent(transform, true);
        presenter.transform.localScale = presenter.BaseLocalScale;

        RealignCards();

        presenter.OnCardClicked += HandleOnCardClicked;
    }

    private void HandleOnCardRemoved(object sender, CardModel card)
    {
        CardPresenterRegistry.TryGet(card, out CardPresenter presenter);

        CardPresenter placeholder = m_cardToPlaceholder[presenter];
        m_cardToPlaceholder.Remove(presenter);
        GameObject.Destroy(placeholder.gameObject);

        RealignCards();
    }

    private void HandleOnCardClicked(object sender, CardPresenter presenter)
    {
        if (!m_canInteractWithCards)
            return;

        presenter.OnCardClicked -= HandleOnCardClicked;

        m_mana.CurrentMana += presenter.Model.CurrentCost;

        m_model.Remove(presenter.Model);
        m_hand.Add(presenter.Model);
    }

    private void RealignCards()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_placeholderParent);

        foreach (var cardAndPlaceholder in m_cardToPlaceholder)
        {
            CardPresenter actualCard = cardAndPlaceholder.Key;
            CardPresenter placeholder = cardAndPlaceholder.Value;

            actualCard.MoveCard(actualCard.transform.localPosition, actualCard.transform.rotation, placeholder.transform.localPosition, placeholder.transform.rotation, m_cardMovementDuration, 0);
        }
    }

    public void Reveal()
    {
        foreach (CardPresenter card in m_cardToPlaceholder.Keys)
            card.Reveal();
    }
}
