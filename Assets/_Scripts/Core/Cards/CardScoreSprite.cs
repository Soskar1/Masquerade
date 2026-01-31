using UnityEngine;

[System.Serializable]
public struct CardScoreSprite
{
    [SerializeField] private CardScore m_score;
    [SerializeField] private Sprite m_sprite;

    public CardScore Score => m_score;
    public Sprite Sprite => m_sprite;
}