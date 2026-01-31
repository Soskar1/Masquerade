using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "EntityData")]
public class EntityData : ScriptableObject
{
    [SerializeField] private List<CardData> m_cardPool;
    [SerializeField] private int m_health;
    [SerializeField] private int m_mana;
    [SerializeField] private int m_handSize;

    public List<CardData> CardPool => m_cardPool;
    public int Health => m_health;
    public int Mana => m_mana;
    public int HandSize => m_handSize;
}