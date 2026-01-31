using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "EntityData")]
public class EntityData : ScriptableObject
{
    [SerializeField] private List<CardData> m_cardPool;
    [SerializeField] private int m_health;
    [SerializeField] private int m_maxMana;
    [SerializeField] private int m_handSize;

    public List<CardData> CardPool => m_cardPool;
    public int Health => m_health;
    public int MaxMana => m_maxMana;
    public int HandSize => m_handSize;
}