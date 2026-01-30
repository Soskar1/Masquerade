using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour
{
    [SerializeField] private Image m_cardImage;
    [SerializeField] private TextMeshProUGUI m_scoreText;

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
    }

    private void HandleOnScoreChanged(object sender, int e)
    {
        m_scoreText.text = e.ToString();
    }
}
