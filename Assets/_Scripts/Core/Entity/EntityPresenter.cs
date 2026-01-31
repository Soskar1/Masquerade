using UnityEngine;

public class EntityPresenter : MonoBehaviour
{
    [SerializeField] private HandPresenter m_handPresenter;
    [SerializeField] private HealthPresenter m_healthPresenter;
    [SerializeField] private ManaPresenter m_manaPresenter;
    [SerializeField] private BoardPresenter m_boardPresenter;
    [SerializeField] private CurrentScore m_score;

    private BattleModel m_battleModel;

    public void Initialize(EntityModel entityModel, BattleModel battleModel)
    {
        m_battleModel = battleModel;

        m_boardPresenter.Initialize(entityModel, battleModel);
        m_handPresenter.Initialize(entityModel.Hand, entityModel.Board, entityModel.Mana, battleModel, !entityModel.IsPlayer, entityModel.IsPlayer, entityModel.IsPlayer);
        m_healthPresenter.Initialize(entityModel.Health);
        m_manaPresenter.Initialize(entityModel.Mana);
    }

    public void Initialize(BattleModel battleModel)
    {
        m_battleModel = battleModel;

        m_battleModel.OnNewEnemy += HandleOnNewEnemy;
        m_battleModel.OnPlayerWon += HandleOnPlayerWon;
    }

    private void OnDisable()
    {
        m_battleModel.OnNewEnemy -= HandleOnNewEnemy;
        m_battleModel.OnPlayerWon -= HandleOnPlayerWon;
    }

    private void HandleOnPlayerWon(object sender, System.EventArgs e)
    {
        m_boardPresenter.Disable();
        m_handPresenter.Disable();
        m_healthPresenter.Unregister();
        m_manaPresenter.Disable();
    }

    private void HandleOnNewEnemy(object sender, EntityModel e)
    {
        m_boardPresenter.Initialize(e, m_battleModel);
        m_handPresenter.Initialize(e.Hand, e.Board, e.Mana, m_battleModel, !e.IsPlayer, e.IsPlayer, e.IsPlayer);
        m_healthPresenter.Initialize(e.Health);
        m_manaPresenter.Initialize(e.Mana);
    }
}
