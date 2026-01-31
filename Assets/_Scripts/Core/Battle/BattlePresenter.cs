using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BattlePresenter : MonoBehaviour
{
    [SerializeField] private Button m_endTurnButton;
    [SerializeField] private BoardPresenter m_enemyBoardPresenter;

    private BattleModel m_battleModel;

    public void Initialize(BattleModel model)
    {
        m_battleModel = model;
        m_battleModel.OnTurnStarted += HandleOnTurnStarted;

        m_battleModel.RevealBoardsAsync = RevealBoardsAsync;
    }

    private void OnDisable() => m_battleModel.OnTurnStarted -= HandleOnTurnStarted;

    private void HandleOnTurnStarted(object sender, System.EventArgs e) => m_endTurnButton.interactable = true;

    public async void EndTurn()
    {
        m_endTurnButton.interactable = false;
        await m_battleModel.EndTurn();
    }

    private async Task RevealBoardsAsync()
    {
        await Task.WhenAll(m_enemyBoardPresenter.RevealCardsAsync());
    }
}
