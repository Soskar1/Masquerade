using UnityEngine;
using UnityEngine.UI;

public class ScrollTexture : MonoBehaviour
{
    [SerializeField] private Vector2 speed;
    [SerializeField] private SpriteRenderer m_sprite;

    void Start()
    {
        m_sprite.material = new Material(m_sprite.material);
    }

    void Update()
    {
        m_sprite.material.mainTextureOffset += speed * Time.deltaTime;
    }
}
