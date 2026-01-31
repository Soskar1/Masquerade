using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BattlePresenter : MonoBehaviour
{
    [SerializeField] private Button m_endTurnButton;
    [SerializeField] private BoardPresenter m_playerBoardPresenter;
    [SerializeField] private BoardPresenter m_enemyBoardPresenter;
    [SerializeField] private CurrentScore m_playerScore;
    [SerializeField] private CurrentScore m_enemyScore;

    private BattleModel m_battleModel;

    public void Initialize(BattleModel model)
    {
        m_battleModel = model;
        m_battleModel.OnTurnStarted += HandleOnTurnStarted;

        m_battleModel.RevealBoardsAsync = RevealBoardsAsync;
        m_battleModel.CalculatePointsAsync = AnimateScoreCalcualtionAsync;
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

    private async Task AnimateScoreCalcualtionAsync()
    {

        List<Task> textMovementTasks = new List<Task>();
        foreach (CardPresenter presenter in m_playerBoardPresenter.Cards)
        {
            ScoreMessage scoreMessage = presenter.SpawnScoreText();
            textMovementTasks.Add(scoreMessage.GetTask());
            scoreMessage.Target = m_playerScore;
        }

        foreach (CardPresenter presenter in m_enemyBoardPresenter.Cards)
        {
            ScoreMessage scoreMessage = presenter.SpawnScoreText();
            textMovementTasks.Add(scoreMessage.GetTask());
            scoreMessage.Target = m_enemyScore;
        }

        m_playerBoardPresenter.BoardModel.Clear();
        m_enemyBoardPresenter.BoardModel.Clear();
        await Task.Delay(2000);
        await Task.WhenAll(textMovementTasks);
        
    }
}
