using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage m_img;
    [SerializeField] private float m_x;
    [SerializeField] private float m_y;

    public void Update()
    {
        m_img.uvRect = new Rect(m_img.uvRect.position + new Vector2(m_x, m_y) * Time.deltaTime, m_img.uvRect.size);
    }
}
