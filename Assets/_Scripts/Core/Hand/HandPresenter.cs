using System.Collections.Generic;
using UnityEngine;

public class HandPresenter : MonoBehaviour
{
    [SerializeField] private CardPresenter m_cardPresenterPrefab;
    [SerializeField] private Transform m_deck;

    [Header("Fan Settings")]
    [SerializeField] private float m_maxFanAngle = 15f;
    [SerializeField] private float m_curveHeight = 40f;
    [SerializeField] private float m_cardSpacing = 150f;

    // Note: A hacky way to place cards on the Y axis
    [SerializeField] private float m_offsetY = 100f;

    [Header("Relayout Animation")]
    [SerializeField] private float m_relayoutDuration = 0.2f;

    private List<CardPresenter> m_cardsInHand;
    private HandModel m_handModel;
    private BoardModel m_boardModel;
    private ManaModel m_manaModel;

    private bool m_displayCardHover;
    private bool m_reactToMouseInput;
    private bool m_isHoverAnimationEnabled;

    public void Initialize(HandModel model, BoardModel boardModel, ManaModel manaModel, bool hideCards = false, bool reactToMouseInput = true, bool isHoverAnimationEnabled = false)
    {
        m_handModel = model;
        m_boardModel = boardModel;
        m_manaModel = manaModel;
        m_handModel.OnCardAdded += HandleOnCardAdded;
        m_cardsInHand = new List<CardPresenter>();

        m_displayCardHover = hideCards;
        m_reactToMouseInput = reactToMouseInput;
        m_isHoverAnimationEnabled = isHoverAnimationEnabled;
    }

    private void OnDisable()
    {
        m_handModel.OnCardAdded -= HandleOnCardAdded;
    }

    private void HandleOnCardAdded(object sender, CardModel model)
    {
        CardPresenter presenter = null;

        if (!CardPresenterRegistry.TryGet(model, out presenter))
        {
            presenter = Instantiate(m_cardPresenterPrefab, m_deck.position, Quaternion.identity, transform);
            presenter.Initialize(model, m_displayCardHover, m_reactToMouseInput, m_isHoverAnimationEnabled);
            CardPresenterRegistry.Register(model, presenter);
        }
        else
        {
            presenter.transform.SetParent(transform, true);

            if (m_isHoverAnimationEnabled)
                presenter.IsHoverAnimationEnabled = true;
        }

        m_cardsInHand.Add(presenter);
        presenter.OnCardClicked += HandleOnCardClicked;

        AnimateRelayout();
    }

    private void HandleOnCardClicked(object sender, CardPresenter card)
    {
        if (m_manaModel.CurrentMana < card.Model.CurrentCost)
            return;

        card.OnCardClicked -= HandleOnCardClicked;

        m_manaModel.CurrentMana -= card.Model.CurrentCost;

        m_cardsInHand.Remove(card);
        m_boardModel.Add(card.Model);

        AnimateRelayout();
    }

    private void GetFanTarget(int index, int count, out Vector3 pos, out Quaternion rot)
    {
        if (count <= 1)
        {
            pos = new Vector3(0f, m_offsetY, 0f);
            rot = Quaternion.identity;
            return;
        }

        float totalWidth = (count - 1) * m_cardSpacing;
        float startX = -totalWidth * 0.5f;

        float x = startX + index * m_cardSpacing;
        float t = Mathf.Lerp(-1f, 1f, (float)index / (count - 1));

        float angle = -t * m_maxFanAngle;
        float yOffset = -Mathf.Pow(Mathf.Abs(t), 2f) * m_curveHeight;

        pos = new Vector3(x, yOffset + m_offsetY, 0f);
        rot = Quaternion.Euler(0f, 0f, angle);
    }

    private void AnimateRelayout()
    {
        int count = m_cardsInHand.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            CardPresenter card = m_cardsInHand[i];
            Transform tr = card.transform;

            Vector3 startPos = tr.localPosition;
            Quaternion startRot = tr.localRotation;

            GetFanTarget(i, count, out Vector3 targetPos, out Quaternion targetRot);
            card.MoveCard(startPos, startRot, targetPos, targetRot, m_relayoutDuration, 0f);
        }
    }
}