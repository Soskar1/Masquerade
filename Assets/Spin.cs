using UnityEngine;

public class Spin : MonoBehaviour
{
    float speed;

    void Start()
    {
        speed = 15.0f;
    }

    void Update()
    {
        transform.Rotate(0, 0, 1 * speed * Time.deltaTime);
    }
}
