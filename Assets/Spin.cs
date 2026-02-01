using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float m_speed = 15.0f;

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(0, 0, 1 * m_speed * Time.deltaTime);
    }
}
