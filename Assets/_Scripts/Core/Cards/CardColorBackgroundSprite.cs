using UnityEngine;

[System.Serializable]
public struct CardColorBackgroundSprite
{
    [SerializeField] private CardColor m_color;
    [SerializeField] private Sprite m_sprite;

    public CardColor Color => m_color;
    public Sprite Sprite => m_sprite;
}