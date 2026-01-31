using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private EntityData m_playerData;
    [SerializeField] private EntityPresenter m_playerPresenter;

    [Header("Enemy")]
    [SerializeField] private List<EntityData> m_enemies;
    [SerializeField] private EntityPresenter m_enemyPresenter;

    [SerializeField] private BattlePresenter m_battlePresenter;

    [SerializeField] private RewardUI m_rewardUI;
    [SerializeField] private List<CardData> m_cardDatabase;

    public async void Awake()
    {
        EntityModel player = new EntityModel(m_playerData, true);
        BattleModel battle = new BattleModel(player, m_enemies);

        m_playerPresenter.Initialize(player, battle);
        m_enemyPresenter.Initialize(battle);
        m_rewardUI.Initialize(new CardDatabase(m_cardDatabase), player);
        m_battlePresenter.Initialize(battle);
        await battle.StartNewBattle();
    }
}
