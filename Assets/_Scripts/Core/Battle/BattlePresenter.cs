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
    [SerializeField] private RewardUI m_rewardUI;

    private Vector3 m_playerScoreInitialPosition;
    private Vector3 m_enemyScoreInitialPosition;

    [SerializeField] private RectTransform m_playerOutOfBounds;
    [SerializeField] private RectTransform m_enemyOutOfBounds;

    [SerializeField] private HealthPresenter m_playerHealthPresenter;
    [SerializeField] private HealthPresenter m_enemyHealthPresenter;

    private BattleModel m_battleModel;

    public void Initialize(BattleModel model)
    {
        m_battleModel = model;
        m_battleModel.OnTurnStarted += HandleOnTurnStarted;
        m_battleModel.OnPlayerWon += HandleOnPlayerWon;

        m_battleModel.RevealBoardsAsync = RevealBoardsAsync;
        m_battleModel.CalculatePointsAsync = AnimateScoreCalcualtionAsync;
        m_battleModel.WaitForReward = WaitForReward;

        m_playerScoreInitialPosition = m_playerScore.transform.localPosition;
        m_enemyScoreInitialPosition = m_enemyScore.transform.localPosition;
    }

    private void HandleOnPlayerWon(object sender, System.EventArgs e)
    {
        m_rewardUI.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        m_battleModel.OnTurnStarted -= HandleOnTurnStarted;
        m_battleModel.OnPlayerWon -= HandleOnPlayerWon;
    }

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
        m_playerScore.transform.localPosition = m_playerScoreInitialPosition;
        m_enemyScore.transform.localPosition = m_enemyScoreInitialPosition;
        m_playerScore.rotate = false;
        m_enemyScore.rotate = false;
        m_playerScore.transform.rotation = Quaternion.identity;
        m_enemyScore.transform.rotation = Quaternion.identity;

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
        await Task.WhenAll(textMovementTasks);
        await AnimateScoreClashAsync();

        m_playerScore.Clear();
        m_enemyScore.Clear();
    }

    private async Task AnimateScoreClashAsync()
    {
        await Task.Delay(500);
        int player = m_playerScore.Value;
        int enemy = m_enemyScore.Value;

        // Nothing to clash (tie or both zero)
        if (player == enemy || (player == 0 && enemy == 0))
        {
            return;
        }

        CurrentScore bigger = player > enemy ? m_playerScore : m_enemyScore;
        CurrentScore smaller = player > enemy ? m_enemyScore : m_playerScore;

        // Big moves onto small
        RectTransform outOfBounds = player > enemy ? m_enemyOutOfBounds : m_playerOutOfBounds;

        bigger.Target = smaller.transform.position;
        bigger.Speed = 500;
        await bigger.GetTask();

        bigger.SetValue(bigger.Value - smaller.Value);

        smaller.Target = outOfBounds.transform.position;
        smaller.Speed = 1000;
        smaller.rotate = true;
        await smaller.GetTask();

        bigger.Target = player > enemy ? m_enemyHealthPresenter.transform.position : m_playerHealthPresenter.transform.position;
        bigger.Speed = 1000;
        await bigger.GetTask();
    }

    private async Task WaitForReward() => await m_rewardUI.GetTask();
}
