using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Data")]
public class CardData : ScriptableObject
{
    [SerializeField] private Sprite m_maskSprite;
    [SerializeField] private int m_score;
    [SerializeField] private int m_cost;

    public Sprite MaskSprite => m_maskSprite;
    public int Score => m_score;
    public int Cost => m_cost;
}