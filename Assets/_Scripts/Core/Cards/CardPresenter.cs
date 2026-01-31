using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image m_maskImage;
    [SerializeField] private Image m_borderImage;
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private GameObject m_cardCover;

    [SerializeField] private Image m_scoreImage;
    [SerializeField] private TextMeshProUGUI m_costText;

    [Header("Hover Settings")]
    [SerializeField] private float m_hoverScaleMultiplier = 1.2f;
    [SerializeField] private float m_hoverOffset = 40f;
    [SerializeField] private float m_hoverDuration = 0.15f;
    [SerializeField] private float m_maxLocalY = 120f;

    [SerializeField] private List<CardColorBackgroundSprite> m_backgroundSprites;
    private Dictionary<CardColor, CardColorBackgroundSprite> m_backgroundSpritesDict;

    [SerializeField] private List<CardScoreSprite> m_scoreSprites;
    private Dictionary<CardScore, CardScoreSprite> m_scoreSpritesDict;

    private Vector3 m_baseLocalPosition;
    private Vector3 m_baseLocalScale;
    private bool m_hasBaseTransform = false;

    private Coroutine m_hoverRoutine;

    private CardModel m_model;

    private bool m_reactToMouseInput;

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

        m_scoreSpritesDict = new Dictionary<CardScore, CardScoreSprite>();

        foreach (CardScoreSprite bgSprite in m_scoreSprites)
            m_scoreSpritesDict.Add(bgSprite.Score, bgSprite);
    }

    public void Initialize(CardModel model, bool displayCardCover = false, bool reactToMouseInput = true)
    {
        m_model = model;
        m_reactToMouseInput = reactToMouseInput;
        
        m_maskImage.sprite = model.CardData.MaskSprite;
        m_borderImage.sprite = model.CardData.BorderSprite;
        m_backgroundImage.sprite = m_backgroundSpritesDict[model.CardColor].Sprite;
        m_scoreImage.sprite = m_scoreSpritesDict[(CardScore)model.CurrentScore].Sprite;

        m_costText.text = model.CurrentCost.ToString();

        m_cardCover.SetActive(displayCardCover);
        m_maskImage.enabled = !displayCardCover;
        m_backgroundImage.enabled = !displayCardCover;
        m_scoreImage.enabled = !displayCardCover;
        m_costText.enabled = !displayCardCover;

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

        // Optional: reset transform when disabled
        if (m_hasBaseTransform)
        {
            transform.localPosition = m_baseLocalPosition;
            transform.localScale = m_baseLocalScale;
        }
    }

    private void HandleOnScoreChanged(object sender, int score)
    {
        m_scoreImage.sprite = m_scoreSpritesDict[(CardScore)score].Sprite;
    }

    private void HandleOnCostChanged(object sender, int cost)
    {
        m_costText.text = cost.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_reactToMouseInput)
            return;

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
        if (!m_reactToMouseInput)
            return;

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
