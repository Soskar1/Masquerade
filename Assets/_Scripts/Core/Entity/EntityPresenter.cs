using UnityEngine;

public class EntityPresenter : MonoBehaviour
{
    [SerializeField] private HandPresenter m_handPresenter;
    [SerializeField] private HealthPresenter m_healthPresenter;
    [SerializeField] private ManaPresenter m_manaPresenter;
    [SerializeField] private BoardPresenter m_boardPresenter;

    public void Initialize(EntityModel entityModel)
    {
        m_boardPresenter.Initialize(entityModel.Board, entityModel.Hand);
        m_handPresenter.Initialize(entityModel.Hand, entityModel.Board, !entityModel.IsPlayer, entityModel.IsPlayer, entityModel.IsPlayer);
        m_healthPresenter.Initialize(entityModel.Health);
        m_manaPresenter.Initialize(entityModel.Mana);
    }
}
