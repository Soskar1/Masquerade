using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [SerializeField] private Sprite m_maskSprite;
    [SerializeField] private int m_score;

    public Sprite MaskSprite => m_maskSprite;
    public int Score => m_score;
}