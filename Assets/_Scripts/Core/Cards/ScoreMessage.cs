using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ScoreMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private float m_minDistance = 0.5f;
    [SerializeField] private float m_speed;

    private CurrentScore m_target;
    public CurrentScore Target
    {
        get => m_target;
        set
        {
            m_target = value;
            m_directionToMove = (m_target.transform.position - transform.position).normalized;
        }
    }

    public int Score { get; private set; }

    private Vector3 m_directionToMove;

    TaskCompletionSource<bool> MovedToTarget;

    public Task GetTask()
    {
        MovedToTarget = new TaskCompletionSource<bool>();

        return MovedToTarget.Task;
    }

    public void Initialize(int score)
    {
        Score = score;
        m_scoreText.text = "+" + score.ToString();
    }

    public void Update()
    {
        if (m_target == null)
            return;

        transform.position += m_directionToMove * m_speed * Time.deltaTime;

        if (Vector2.Distance(transform.position, Target.transform.position) < m_minDistance)
        {
            m_target.Accept(this);
            m_target = null;
            MovedToTarget.SetResult(true);
        }
    }
}
