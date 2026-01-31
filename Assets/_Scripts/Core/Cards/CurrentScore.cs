using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CurrentScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;

    [Header("Scaling")]
    [SerializeField] private float m_baseScale = 1f;
    [SerializeField] private float m_maxScale = 1.6f;

    [Header("Color")]
    [SerializeField] private Color m_baseColor = Color.white;
    [SerializeField] private Color m_maxColor = Color.red;

    [Header("Score Range")]
    [SerializeField] private int m_minScore = 0;
    [SerializeField] private int m_maxScore = 50;

    [Header("Animation")]
    [SerializeField] private float m_lerpSpeed = 10f;

    [SerializeField] private float m_speed = 150;
    [SerializeField] private float m_minDistance = 0.5f;

    private int m_currentScore;
    private Vector3 m_targetScale;
    private Color m_targetColor;

    private Vector3 m_target;
    public Vector3 Target
    {
        get => m_target;
        set
        {
            m_target = value;
            m_directionToMove = (m_target - transform.position).normalized;
        }
    }

    public float Speed
    {
        get => m_speed;
        set
        {
            m_speed = value;
        }
    }

    public bool rotate;

    public Task GetTask()
    {
        MovedToTarget = new TaskCompletionSource<bool>();

        return MovedToTarget.Task;
    }

    private Vector3 m_directionToMove;
    TaskCompletionSource<bool> MovedToTarget;

    public int Value => m_currentScore;

    public void SetValue(int value)
    {
        m_currentScore = value;
        m_text.text = (m_currentScore == 0) ? "" : m_currentScore.ToString();

        UpdateTargets();
    }

    private void Awake()
    {
        ResetVisuals();
    }

    public void Accept(ScoreMessage message)
    {
        m_currentScore += message.Score;
        m_text.text = m_currentScore.ToString();

        UpdateTargets();
        Destroy(message.gameObject);
    }

    public void Clear()
    {
        m_currentScore = 0;
        m_text.text = "";
        ResetVisuals();
    }

    private void Update()
    {
        // Smooth interpolation
        m_text.transform.localScale =
            Vector3.Lerp(m_text.transform.localScale, m_targetScale, Time.deltaTime * m_lerpSpeed);

        m_text.color =
            Color.Lerp(m_text.color, m_targetColor, Time.deltaTime * m_lerpSpeed);

        if (rotate)
        {
            transform.Rotate(0, 0, Time.deltaTime * m_speed);
        }

        if (MovedToTarget == null)
            return;

        transform.position += m_directionToMove * m_speed * Time.deltaTime;

        if (Vector2.Distance(transform.position, Target) < m_minDistance)
        {
            MovedToTarget.SetResult(true);
            MovedToTarget = null;
        }
    }

    private void UpdateTargets()
    {
        float t = Mathf.InverseLerp(m_minScore, m_maxScore, m_currentScore);

        float scale = Mathf.Lerp(m_baseScale, m_maxScale, t);
        m_targetScale = Vector3.one * scale;

        m_targetColor = Color.Lerp(m_baseColor, m_maxColor, t);
    }

    private void ResetVisuals()
    {
        m_targetScale = Vector3.one * m_baseScale;
        m_targetColor = m_baseColor;

        m_text.transform.localScale = m_targetScale;
        m_text.color = m_targetColor;
    }
}
