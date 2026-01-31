using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image m_maskImage;
    [SerializeField] private Image m_borderImage;
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private TextMeshProUGUI m_costText;
    [SerializeField] private GameObject m_cardCover;

    [Header("Hover Settings")]
    [SerializeField] private float m_hoverScaleMultiplier = 1.2f;
    [SerializeField] private float m_hoverOffset = 40f;
    [SerializeField] private float m_hoverDuration = 0.15f;
    [SerializeField] private float m_maxLocalY = 120f;

    [SerializeField] private List<CardColorBackgroundSprite> m_backgroundSprites;
    private Dictionary<CardColor, CardColorBackgroundSprite> m_backgroundSpritesDict;

    private Vector3 m_baseLocalPosition;
    private Vector3 m_baseLocalScale;
    public Vector3 BaseLocalScale => m_baseLocalScale;

    private Coroutine m_hoverRoutine;
    private Coroutine m_moveRoutine;

    private CardModel m_model;

    private bool m_reactToMouseInput;

    public event EventHandler<CardPresenter> OnCardClicked;

    public bool ReactToMouseInput
    {
        get => m_reactToMouseInput;
        set => m_reactToMouseInput = value;
    }

    private void Awake()
    {
        m_backgroundSpritesDict = new Dictionary<CardColor, CardColorBackgroundSprite>();
        foreach (CardColorBackgroundSprite bgSprite in m_backgroundSprites)
            m_backgroundSpritesDict.Add(bgSprite.Color, bgSprite);
    }

    public void Initialize(CardModel model, bool displayCardCover = false, bool reactToMouseInput = true)
    {
        m_model = model;
        m_reactToMouseInput = reactToMouseInput;
        
        m_maskImage.sprite = model.CardData.MaskSprite;
        m_borderImage.sprite = model.CardData.BorderSprite;
        m_backgroundImage.sprite = m_backgroundSpritesDict[model.CardColor].Sprite;

        m_scoreText.text = model.CurrentScore.ToString();
        m_costText.text = model.CurrentCost.ToString();

        m_cardCover.SetActive(displayCardCover);

        m_baseLocalScale = transform.localScale;

        m_model.OnScoreChanged += HandleOnScoreChanged;
        m_model.OnCostChanged += HandleOnCostChanged;
    }

    private void OnDisable()
    {
        if (m_model != null)
        {
            m_model.OnScoreChanged -= HandleOnScoreChanged;
            m_model.OnCostChanged -= HandleOnCostChanged;
        }

        if (m_hoverRoutine != null)
            StopCoroutine(m_hoverRoutine);

        transform.localPosition = m_baseLocalPosition;
        transform.localScale = m_baseLocalScale;
    }

    private void HandleOnScoreChanged(object sender, int score)
    {
        m_scoreText.text = score.ToString();
    }

    private void HandleOnCostChanged(object sender, int cost)
    {
        m_costText.text = cost.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_reactToMouseInput)
            return;

        Vector3 targetScale = m_baseLocalScale * m_hoverScaleMultiplier;
        Vector3 targetPos = CalculateHoverPosition(m_hoverOffset);

        StartHoverTween(targetPos, targetScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!m_reactToMouseInput)
            return;

        StartHoverTween(m_baseLocalPosition, m_baseLocalScale);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_reactToMouseInput)
            return;

        if (m_hoverRoutine != null)
            StopCoroutine(m_hoverRoutine);

        if (m_moveRoutine != null)
            StopCoroutine(m_moveRoutine);

        OnCardClicked?.Invoke(this, this);
    }

    private void StartHoverTween(Vector3 targetPos, Vector3 targetScale)
    {
        if (m_hoverRoutine != null)
            StopCoroutine(m_hoverRoutine);

        m_hoverRoutine = StartCoroutine(HoverTweenCoroutine(targetPos, targetScale));
    }

    private IEnumerator HoverTweenCoroutine(Vector3 targetPos, Vector3 targetScale)
    {
        RectTransform rect = transform as RectTransform;

        Vector3 startPos = rect.localPosition;
        Vector3 startScale = rect.localScale;

        float time = 0f;

        while (time < m_hoverDuration)
        {
            time += Time.unscaledDeltaTime; // unscaled so it still feels good if you use timeScale
            float t = Mathf.Clamp01(time / m_hoverDuration);

            rect.localPosition = Vector3.Lerp(startPos, targetPos, t);
            rect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        rect.localPosition = targetPos;
        rect.localScale = targetScale;
        m_hoverRoutine = null;
    }

    /// <summary>
    /// Calculates the hover position by moving the card along its own "up" direction.
    /// Works even if the card is rotated in the hand.
    /// </summary>
    private Vector3 CalculateHoverPosition(float offset)
    {
        RectTransform rect = transform as RectTransform;
        RectTransform parentRect = rect.parent as RectTransform;

        // Move along local "up" direction in world space
        Vector3 worldTarget = rect.position + rect.up * offset;

        // Convert to parent local space
        Vector3 localTarget = parentRect.InverseTransformPoint(worldTarget);

        // Clamp Y so it never goes above the upper bound
        localTarget.y = Mathf.Min(localTarget.y, m_maxLocalY);

        return localTarget;
    }

    public void MoveCard(Vector3 startPos, Quaternion startRot, Vector3 targetPos, Quaternion targetRot, float duration, float delay)
    {
        m_baseLocalPosition = targetPos;
        m_moveRoutine = StartCoroutine(MoveCardCoroutine(startPos, startRot, targetPos, targetRot, duration, delay));
    }

    private IEnumerator MoveCardCoroutine(Vector3 startPos, Quaternion startRot, Vector3 targetPos, Quaternion targetRot, float duration, float delay)
    {
        Transform cardTransform = transform;

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
    }
}
