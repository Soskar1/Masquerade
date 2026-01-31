using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleModel
{
    private EntityModel m_player;
    private EntityModel m_enemy;

    public event EventHandler OnTurnStarted;
    public event EventHandler OnTurnEnded;

    public BattleModel(EntityModel player, EntityModel enemy)
    {
        m_player = player;
        m_enemy = enemy;
    }

    public async Task StartTurn()
    {
        m_player.Mana.Restore();
        m_enemy.Mana.Restore();

        m_player.Board.Clear();
        m_enemy.Board.Clear();

        m_player.Hand.DrawCards();
        m_enemy.Hand.DrawCards();

        List<CardModel> enemyCards = EnemyPickCards();

        foreach (CardModel card in enemyCards)
        {
            await Task.Run(() => Task.Delay(150));
            m_enemy.Hand.RemoveCard(card);
            m_enemy.Board.Add(card);
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
            foreach (CardModel card in m_enemy.Hand.Cards)
            {
                if (card.CurrentCost <= m_enemy.Mana.CurrentMana && !cards.Contains(card))
                {
                    m_enemy.Mana.CurrentMana -= card.CurrentCost;
                    found = true;

                    cards.Add(card);
                }
            }
        } while (found);

        return cards;
    }

    public async Task EndTurn()
    {
        OnTurnEnded?.Invoke(this, EventArgs.Empty);

        int playerScore = CalculateScore(m_player.Board);
        int enemyScore = CalculateScore(m_enemy.Board);

        int diff = Mathf.Abs(playerScore - enemyScore);

        if (playerScore > enemyScore)
            m_enemy.Health.CurrentHealth -= diff;
        else
            m_player.Health.CurrentHealth -= diff;

        await StartTurn();
    }

    private int CalculateScore(BoardModel board)
    {
        int score = 0;

        foreach (CardModel card in board.SelectedCards)
            score += card.CurrentScore;

        return score;
    }
}
