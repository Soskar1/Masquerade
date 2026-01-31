using System.Collections;
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

    [Header("Draw Animation")]
    [SerializeField] private float m_drawDuration = 0.3f;
    [SerializeField] private float m_drawDelayBetweenCards = 0.05f;

    [Header("Relayout Animation")]
    [SerializeField] private float m_relayoutDuration = 0.2f;

    private List<CardPresenter> m_cardsInHand;
    private HandModel m_handModel;
    private BoardModel m_boardModel;

    private bool m_displayCardHover;
    private bool m_reactToMouseInput;

    public void Initialize(HandModel model, BoardModel boardModel, bool hideCards = false, bool reactToMouseInput = true)
    {
        m_handModel = model;
        m_boardModel = boardModel;
        m_handModel.OnHandChanged += HandleOnHandChanged;
        m_cardsInHand = new List<CardPresenter>();

        m_displayCardHover = hideCards;
        m_reactToMouseInput = reactToMouseInput;
    }

    private void HandleOnHandChanged(object sender, List<CardModel> models)
    {
        // NOTE:
        // This code assumes OnHandChanged gives you only *new* cards to add.
        // If it's the full hand every time, you'll want to Clear & rebuild instead.

        foreach (CardModel model in models)
        {
            CardPresenter instance = Instantiate(m_cardPresenterPrefab, transform);
            instance.Initialize(model, m_displayCardHover, m_reactToMouseInput);

            m_cardsInHand.Add(instance);
            instance.OnCardClicked += HandleOnCardClicked;
        }

        ApplyFanLayoutFromDeck();
    }

    private void HandleOnCardClicked(object sender, CardPresenter card)
    {
        card.OnCardClicked -= HandleOnCardClicked;
        m_cardsInHand.Remove(card);

        m_boardModel.Add(card);

        AnimateRelayout();
    }

    private void ApplyFanLayoutFromDeck()
    {
        int count = m_cardsInHand.Count;
        if (count == 0) return;

        Transform parent = transform;

        Vector3 deckLocalPos = parent.InverseTransformPoint(m_deck.position);
        Quaternion deckLocalRot = Quaternion.Inverse(parent.rotation) * m_deck.rotation;

        for (int i = 0; i < count; i++)
        {
            CardPresenter card = m_cardsInHand[i];
            Transform tr = card.transform;

            if (m_reactToMouseInput)
                card.ReactToMouseInput = false;

            GetFanTarget(i, count, out Vector3 targetPos, out Quaternion targetRot);
            card.BaseLocalPosition = targetPos;

            tr.localPosition = deckLocalPos;
            tr.localRotation = deckLocalRot;

            float delay = i * m_drawDelayBetweenCards;
            StartCoroutine(AnimateCard(card, deckLocalPos, deckLocalRot, targetPos, targetRot, m_drawDuration, delay));
        }
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
            card.BaseLocalPosition = targetPos;
            StartCoroutine(AnimateCard(card, startPos, startRot, targetPos, targetRot, m_relayoutDuration, 0f));
        }
    }

    private IEnumerator AnimateCard(CardPresenter card, Vector3 startPos, Quaternion startRot, Vector3 targetPos, Quaternion targetRot, float duration, float delay)
    {
        Transform cardTransform = card.transform;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float time = 0f;

        while (time < duration)
        {
            if (cardTransform == null)
                yield break;

            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            cardTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            cardTransform.localRotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        if (cardTransform != null)
        {
            cardTransform.localPosition = targetPos;
            cardTransform.localRotation = targetRot;
        }

        if (m_reactToMouseInput)
            card.ReactToMouseInput = true;
    }
}