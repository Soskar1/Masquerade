using UnityEngine;

[System.Serializable]
public struct CardCostSprite
{
    [SerializeField] private CardCost m_cost;
    [SerializeField] private Sprite m_sprite;

    public CardCost Cost => m_cost;
    public Sprite Sprite => m_sprite;
}