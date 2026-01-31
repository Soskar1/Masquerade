using UnityEngine;
using UnityEngine.UI;

public class BattlePresenter : MonoBehaviour
{
    [SerializeField] private Button m_endTurnButton;
    [SerializeField] private BoardPresenter m_enemyBoard;

    private BattleModel m_battleModel;

    public void Initialize(BattleModel model)
    {
        m_battleModel = model;
        m_battleModel.OnTurnStarted += HandleOnTurnStarted;
    }

    private void OnDisable() => m_battleModel.OnTurnStarted -= HandleOnTurnStarted;

    private void HandleOnTurnStarted(object sender, System.EventArgs e) => m_endTurnButton.interactable = true;

    public void EndTurn()
    {
        m_endTurnButton.interactable = false;
        m_enemyBoard.Reveal();

        m_battleModel.EndTurn();
    }
}
