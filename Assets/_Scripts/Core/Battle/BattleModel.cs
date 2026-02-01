using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleModel
{
    private EntityModel m_player;
    private EntityModel m_currentEnemy;
    private List<EntityData> m_enemies;
    private int m_currentEnemyIndex = 0;

    public event EventHandler OnTurnStarted;
    public event EventHandler OnTurnEnded;
    public event EventHandler OnPlayerWon;
    public event EventHandler OnEnemyWon;
    public event EventHandler<EntityModel> OnNewEnemy;

    public Func<Task> RevealBoardsAsync { get; set; }
    public Func<Task> CalculatePointsAsync { get; set; }
    public Func<Task> WaitForReward { get; set; }
    public Func<Task> BuffCards { get; set; }

    public BattleModel(EntityModel player, List<EntityData> enemies)
    {
        m_player = player;
        m_enemies = enemies;
    }

    public async Task StartNewBattle()
    {
        if (m_currentEnemy != null)
        {
            m_currentEnemy.Board.Clear();
            m_currentEnemy.Hand.Clear();
        }

        EntityData data = m_enemies[m_currentEnemyIndex];
        m_currentEnemy = new EntityModel(data, false);

        ++m_currentEnemyIndex;
        if (m_currentEnemyIndex >= m_enemies.Count)
        {
            m_currentEnemyIndex = m_enemies.Count - 1;
        }
        else
        {
            m_player.Hand.IncreaseMaxSize();
        }

        OnNewEnemy?.Invoke(this, m_currentEnemy);
        await StartTurn();
    }

    public async Task StartTurn()
    {
        m_player.Mana.Restore();
        m_currentEnemy.Mana.Restore();

        m_player.Board.Clear();
        m_currentEnemy.Board.Clear();

        await m_player.Hand.DrawCards();
        await m_currentEnemy.Hand.DrawCards();

        List<CardModel> enemyCards = EnemyPickCards();

        foreach (CardModel card in enemyCards)
        {
            await Task.Run(() => Task.Delay(150));
            m_currentEnemy.Hand.RemoveCard(card);
            m_currentEnemy.Board.Add(card);
        }

        OnTurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public List<CardModel> EnemyPickCards()
    {
        bool found = false;
        List<CardModel> cards = new List<CardModel>();

        do
        {
            found = false;
            foreach (CardModel card in m_currentEnemy.Hand.Cards)
            {
                if (card.CurrentCost <= m_currentEnemy.Mana.CurrentMana && !cards.Contains(card))
                {
                    m_currentEnemy.Mana.CurrentMana -= card.CurrentCost;
                    found = true;

                    cards.Add(card);
                }
            }
        } while (found);

        return cards;
    }

    public async Task EndTurn()
    {
        if (RevealBoardsAsync != null)
            await RevealBoardsAsync();

        await BuffCards?.Invoke();

        int playerScore = CalculateScore(m_player.Board);
        int enemyScore = CalculateScore(m_currentEnemy.Board);

        await CalculatePointsAsync?.Invoke();

        int diff = Mathf.Abs(playerScore - enemyScore);
        if (diff > 0)
        {
            if (playerScore > enemyScore)
                m_currentEnemy.Health.CurrentHealth -= diff;
            else
                m_player.Health.CurrentHealth -= diff;
        }

        OnTurnEnded?.Invoke(this, EventArgs.Empty);

        if (m_currentEnemy.Health.CurrentHealth <= 0)
        {
            m_currentEnemy.Hand.Clear();

            OnPlayerWon?.Invoke(this, EventArgs.Empty);
            await WaitForReward();
            await StartNewBattle();
        }
        else if (m_player.Health.CurrentHealth <= 0)
        {
            OnEnemyWon?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            // 4) Start next turn (optional delay if you want)
            await StartTurn();
        }
    }

    private int CalculateScore(BoardModel board)
    {
        int score = 0;

        foreach (CardModel card in board.SelectedCards)
            score += card.CurrentScore;

        return score;
    }
}
