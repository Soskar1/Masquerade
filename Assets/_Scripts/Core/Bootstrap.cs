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

    public async void Awake()
    {
        EntityModel player = new EntityModel(m_playerData, true);
        EntityModel enemy = new EntityModel(m_enemyData, false);
        BattleModel battle = new BattleModel(player, enemy);

        m_playerPresenter.Initialize(player, battle);
        m_enemyPresenter.Initialize(enemy, battle);

        m_battlePresenter.Initialize(battle);
        await battle.StartTurn();
    }
}
