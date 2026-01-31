using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardPresenter : MonoBehaviour
{
    [SerializeField] private RectTransform m_placeholderParent;
    [SerializeField] private CardPresenter m_ghostCardPrefab;
    [SerializeField] private float m_cardMovementDuration;

    private Dictionary<CardPresenter, CardPresenter> m_cardToPlaceholder;

    private EntityModel m_entity;
    private BoardModel m_board;
    private HandModel m_hand;
    private ManaModel m_mana;
    private BattleModel m_battle;

    private bool m_canInteractWithCards = false;

    public void Initialize(EntityModel entity, BattleModel battle)
    {
        m_entity = entity;
        m_board = entity.Board;
        m_hand = entity.Hand;
        m_mana = entity.Mana;
        m_battle = battle;
        m_board.OnCardAdded += HandleOnCardAdded;
        m_board.OnCardRemoved += HandleOnCardRemoved;
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

        if (!m_entity.IsPlayer)
            Reveal();
    }

    private void OnDisable()
    {
        m_board.OnCardAdded -= HandleOnCardAdded;
        m_board.OnCardRemoved -= HandleOnCardRemoved;
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

    private void HandleOnCardRemoved(object sender, OnCardRemovedEventArgs args)
    {
        CardPresenterRegistry.TryGet(args.CardModel, out CardPresenter presenter);

        CardPresenter placeholder = m_cardToPlaceholder[presenter];
        m_cardToPlaceholder.Remove(presenter);
        GameObject.Destroy(placeholder.gameObject);

        if (args.DeleteFromGame)
        {
            CardPresenterRegistry.Unregister(args.CardModel);
            GameObject.Destroy(presenter.gameObject);
        }

        RealignCards();
    }

    private void HandleOnCardClicked(object sender, CardPresenter presenter)
    {
        if (!m_canInteractWithCards)
            return;

        presenter.OnCardClicked -= HandleOnCardClicked;

        m_mana.CurrentMana += presenter.Model.CurrentCost;

        m_board.Remove(presenter.Model);
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
