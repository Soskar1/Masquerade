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

    public void Initialize(BoardModel model, HandModel hand, ManaModel mana)
    {
        m_model = model;
        m_hand = hand;
        m_mana = mana;
        model.OnCardAdded += HandleOnCardAdded;

        m_cardToPlaceholder = new Dictionary<CardPresenter, CardPresenter>();
    }

    private void OnDisable()
    {
        m_model.OnCardAdded -= HandleOnCardAdded;
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

    private void HandleOnCardClicked(object sender, CardPresenter presenter)
    {
        presenter.OnCardClicked -= HandleOnCardClicked;

        m_mana.CurrentMana += presenter.Model.CurrentCost;

        CardPresenter placeholder = m_cardToPlaceholder[presenter];
        m_cardToPlaceholder.Remove(presenter);
        GameObject.Destroy(placeholder.gameObject);

        RealignCards();

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
}
