using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifierPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_percentageText;
    [SerializeField] private Image m_bg;

    public void Initialize(ModifierModel modifier)
    {
        m_percentageText.text = modifier.Percentage.ToString();
        m_bg.color = modifier.CardColor.GetColorCode();
    }
}
