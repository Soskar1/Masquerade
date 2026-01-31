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

    private List<CardPresenter> m_cardsInHand;
    private HandModel m_handModel;

    private bool m_displayCardHover;
    private bool m_reactToMouseInput;

    public void Initialize(HandModel model, bool hideCards = false, bool reactToMouseInput = true)
    {
        m_handModel = model;
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
        }

        ApplyFanLayoutFromDeck();
    }

    private void ApplyFanLayoutFromDeck()
    {
        int count = m_cardsInHand.Count;
        if (count == 0)
            return;

        // If deck is not assigned, just place instantly like before
        if (m_deck == null)
        {
            ApplyFanLayoutInstant();
            return;
        }

        // We want to animate each card from the deck to its final position
        float totalWidth = (count - 1) * m_cardSpacing;
        float startX = -totalWidth * 0.5f;

        // Parent for local space
        Transform parent = transform;

        for (int i = 0; i < count; i++)
        {
            CardPresenter card = m_cardsInHand[i];
            Transform tr = card.transform;

            // Calculate final position & rotation for this card in hand
            Vector3 targetPos;
            Quaternion targetRot;

            if (count == 1)
            {
                targetPos = new Vector3(0f, m_offsetY, 0f);
                targetRot = Quaternion.identity;
            }
            else
            {
                float x = startX + i * m_cardSpacing;
                float t = Mathf.Lerp(-1f, 1f, (float)i / (count - 1));

                float angle = -t * m_maxFanAngle;
                float yOffset = -Mathf.Pow(Mathf.Abs(t), 2f) * m_curveHeight;

                targetPos = new Vector3(x, yOffset + m_offsetY, 0f);
                targetRot = Quaternion.Euler(0f, 0f, angle);
            }

            // Start at the deck position/rotation
            Vector3 deckLocalPos = parent.InverseTransformPoint(m_deck.position);
            Quaternion deckLocalRot = Quaternion.Inverse(parent.rotation) * m_deck.rotation;

            tr.localPosition = deckLocalPos;
            tr.localRotation = deckLocalRot;

            // Animate to target
            float delay = i * m_drawDelayBetweenCards;
            StartCoroutine(AnimateCardToHand(tr, deckLocalPos, deckLocalRot, targetPos, targetRot, delay));
        }
    }

    private IEnumerator AnimateCardToHand(
        Transform cardTransform,
        Vector3 startPos,
        Quaternion startRot,
        Vector3 targetPos,
        Quaternion targetRot,
        float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float time = 0f;

        while (time < m_drawDuration)
        {
            // Card might be destroyed while animating
            if (cardTransform == null)
                yield break;

            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / m_drawDuration);

            cardTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            cardTransform.localRotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        if (cardTransform != null)
        {
            cardTransform.localPosition = targetPos;
            cardTransform.localRotation = targetRot;
        }
    }

    // Fallback / non-animated layout
    private void ApplyFanLayoutInstant()
    {
        int count = m_cardsInHand.Count;
        if (count == 0) return;

        float totalWidth = (count - 1) * m_cardSpacing;
        float startX = -totalWidth * 0.5f;

        if (count == 1)
        {
            m_cardsInHand[0].transform.localRotation = Quaternion.identity;
            m_cardsInHand[0].transform.localPosition = new Vector3(0f, m_offsetY, 0f);
            return;
        }

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * m_cardSpacing;
            float t = Mathf.Lerp(-1f, 1f, (float)i / (count - 1));

            float angle = -t * m_maxFanAngle;
            float yOffset = -Mathf.Pow(Mathf.Abs(t), 2f) * m_curveHeight;

            Transform tr = m_cardsInHand[i].transform;
            tr.localPosition = new Vector3(x, yOffset + m_offsetY, 0f);
            tr.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}