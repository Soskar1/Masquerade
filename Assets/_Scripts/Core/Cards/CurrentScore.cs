using TMPro;
using UnityEngine;

public class CurrentScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;
    private int m_currentScore;

    public void Accept(ScoreMessage message)
    {
        m_currentScore += message.Score;
        m_text.text = m_currentScore.ToString();
        GameObject.Destroy(message.gameObject);
    }
}
