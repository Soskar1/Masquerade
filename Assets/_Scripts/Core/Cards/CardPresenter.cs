using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image m_cardImage;
    [SerializeField] private TextMeshProUGUI m_scoreText;

    [Header("Hover Settings")]
    [SerializeField] private float m_hoverScaleMultiplier = 1.2f;
    [SerializeField] private float m_hoverOffset = 40f;        // how far to move along local "up"
    [SerializeField] private float m_hoverDuration = 0.15f;    // seconds
    [SerializeField] private float m_maxLocalY = 120f; // upper limit in parent local space

    private Vector3 m_baseLocalPosition;
    private Vector3 m_baseLocalScale;
    private bool m_hasBaseTransform = false;

    private Coroutine m_hoverRoutine;

    private CardModel m_model;

    public void Initialize(CardModel model)
    {
        m_model = model;
        m_cardImage.sprite = model.CardData.MaskSprite;
        m_scoreText.text = model.CurrentScore.ToString();

        m_model.OnScoreChanged += HandleOnScoreChanged;
    }

    private void OnDisable()
    {
        if (m_model != null)
            m_model.OnScoreChanged -= HandleOnScoreChanged;

        if (m_hoverRoutine != null)
            StopCoroutine(m_hoverRoutine);

        // Optional: reset transform when disabled
        if (m_hasBaseTransform)
        {
            transform.localPosition = m_baseLocalPosition;
            transform.localScale = m_baseLocalScale;
        }
    }

    private void HandleOnScoreChanged(object sender, int e)
    {
        m_scoreText.text = e.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Store the base transform on first hover
        if (!m_hasBaseTransform)
        {
            m_baseLocalPosition = transform.localPosition;
            m_baseLocalScale = transform.localScale;
            m_hasBaseTransform = true;
        }

        Vector3 targetScale = m_baseLocalScale * m_hoverScaleMultiplier;
        Vector3 targetPos = CalculateHoverPosition(m_hoverOffset);

        StartHoverTween(targetPos, targetScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!m_hasBaseTransform)
            return;

        StartHoverTween(m_baseLocalPosition, m_baseLocalScale);
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
}
