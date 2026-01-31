using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Data")]
public class CardData : ScriptableObject
{
    [SerializeField] private Sprite m_maskSprite;
    [SerializeField] private Sprite m_borderSprite;
    [SerializeField] private Sprite m_backgroundSprite;

    [SerializeField] private int m_score;
    [SerializeField] private int m_cost;

    public Sprite MaskSprite => m_maskSprite;
    public Sprite BorderSprite => m_borderSprite;
    public Sprite BackgroundSprite => m_backgroundSprite;

    public int Score => m_score;
    public int Cost => m_cost;
}