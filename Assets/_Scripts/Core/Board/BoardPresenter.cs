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

    public void Initialize(BoardModel model)
    {
        m_model = model;
        model.OnCardAdded += HandleOnCardAdded;
        model.OnCardRemoved += HandleOnCardRemoved;

        m_cardToPlaceholder = new Dictionary<CardPresenter, CardPresenter>();
    }

    private void OnDisable()
    {
        m_model.OnCardAdded -= HandleOnCardAdded;
        m_model.OnCardRemoved -= HandleOnCardRemoved;
    }

    private void HandleOnCardAdded(object sender, CardPresenter card)
    {
        card.ReactToMouseInput = false;

        CardPresenter placeholderInstace = Instantiate(m_ghostCardPrefab, m_placeholderParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_placeholderParent);

        m_cardToPlaceholder.Add(card, placeholderInstace);
        card.transform.SetParent(transform, true);
        card.transform.localScale = card.BaseLocalScale;

        foreach (var cardAndPlaceholder in m_cardToPlaceholder)
        {
            CardPresenter actualCard = cardAndPlaceholder.Key;
            CardPresenter placeholder = cardAndPlaceholder.Value;

            actualCard.MoveCard(actualCard.transform.localPosition, actualCard.transform.rotation, placeholder.transform.localPosition, placeholder.transform.rotation, m_cardMovementDuration, 0);
        }
    }

    private void HandleOnCardRemoved(object sender, CardPresenter card)
    {
        // TODO
    }
}
