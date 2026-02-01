using UnityEngine;

[System.Serializable]
public struct CardColorBackgroundSprite
{
    [SerializeField] private CardColor m_color;
    [SerializeField] private Texture m_texture;

    public CardColor Color => m_color;
    public Texture Texture => m_texture;
}