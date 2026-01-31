using UnityEngine;

public class EntityPresenter : MonoBehaviour
{
    [SerializeField] private HandPresenter m_handPresenter;
    [SerializeField] private HealthPresenter m_healthPresenter;
    [SerializeField] private ManaPresenter m_manaPresenter;
    [SerializeField] private BoardPresenter m_boardPresenter;

    public void Initialize(EntityModel entityModel, BattleModel battleModel)
    {
        m_boardPresenter.Initialize(entityModel, battleModel);
        m_handPresenter.Initialize(entityModel.Hand, entityModel.Board, entityModel.Mana, battleModel, !entityModel.IsPlayer, entityModel.IsPlayer, entityModel.IsPlayer);
        m_healthPresenter.Initialize(entityModel.Health);
        m_manaPresenter.Initialize(entityModel.Mana);
    }
}
