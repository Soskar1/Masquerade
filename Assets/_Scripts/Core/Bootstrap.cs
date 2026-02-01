using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private List<EntityData> m_playerData;
    [SerializeField] private EntityPresenter m_playerPresenter;

    [Header("Enemy")]
    [SerializeField] private List<EntityData> m_enemies;
    [SerializeField] private EntityPresenter m_enemyPresenter;

    [SerializeField] private BattlePresenter m_battlePresenter;

    [SerializeField] private RewardUI m_rewardUI;
    [SerializeField] private List<CardData> m_cardDatabase;

    [SerializeField] private EscapeMenu m_escapeMenu;

    private Controls m_controls;

    public async void Awake()
    {
        m_controls = new Controls();
        m_controls.Enable();
        m_controls.Player.Escape.performed += Escape_performed;

        EntityData randomData = m_playerData[Random.Range(0, m_playerData.Count)];
        EntityModel player = new EntityModel(randomData, true);
        BattleModel battle = new BattleModel(player, m_enemies);

        m_playerPresenter.Initialize(player, battle);
        m_enemyPresenter.Initialize(battle);
        m_rewardUI.Initialize(new CardDatabase(m_cardDatabase), player);
        m_battlePresenter.Initialize(battle);
        await battle.StartNewBattle();
    }

    public void OnDisable()
    {
        m_controls.Player.Escape.performed -= Escape_performed;
        m_controls.Disable();
        m_controls.Dispose();
    }

    private void Escape_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        m_escapeMenu.gameObject.SetActive(!m_escapeMenu.gameObject.activeSelf);
    }
}
