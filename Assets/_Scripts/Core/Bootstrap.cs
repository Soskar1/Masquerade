using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private EntityData m_playerData;
    [SerializeField] private EntityPresenter m_playerPresenter;

    [Header("Enemy")]
    [SerializeField] private EntityData m_enemyData;
    [SerializeField] private EntityPresenter m_enemyPresenter;

    [SerializeField] private BattlePresenter m_battlePresenter;

    [SerializeField] private RewardUI m_rewardUI;
    [SerializeField] private List<CardData> m_cardDatabase;

    public async void Awake()
    {
        EntityModel player = new EntityModel(m_playerData, true);
        EntityModel enemy = new EntityModel(m_enemyData, false);
        BattleModel battle = new BattleModel(player, enemy);

        m_playerPresenter.Initialize(player, battle);
        m_enemyPresenter.Initialize(enemy, battle);
        m_rewardUI.Initialize(new CardDatabase(m_cardDatabase));
        m_battlePresenter.Initialize(battle);
        await battle.StartTurn();
    }
}
