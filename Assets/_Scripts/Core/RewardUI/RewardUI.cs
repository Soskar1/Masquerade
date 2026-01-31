using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private Transform m_cardSpawnpoint;
    [SerializeField] private CardPresenter m_cardPresenterPrefab;
    [SerializeField] private List<Transform> m_cardPlaceholders;

    [SerializeField] private Button m_getButton;
    [SerializeField] private TextMeshProUGUI m_headerText;

    [SerializeField] private Animator m_animator;

    private CardDatabase m_cardDatabase;

    private EntityModel m_player;
    private CardData m_selectedCard;

    private List<CardPresenter> m_cards;

    TaskCompletionSource<bool> m_rewardPicked;

    public Task GetTask()
    {
        m_rewardPicked = new TaskCompletionSource<bool>();
        return m_rewardPicked.Task;
    }

    public void Initialize(CardDatabase database, EntityModel player)
    {
        m_cardDatabase = database;
        m_player = player;
        m_cards = new List<CardPresenter>();
    }

    public async Task DisplayCards()
    {
        m_animator.enabled = false;

        float initialDelay = 0;
        float time = 0.25f;
        Task lastCardTask = Task.CompletedTask;

        for (int i = 0; i < m_cardPlaceholders.Count; ++i)
        {
            Transform placeholder = m_cardPlaceholders[i];

            CardPresenter instance = Instantiate(m_cardPresenterPrefab, m_cardSpawnpoint.position, Quaternion.identity, transform);
            CardData data = m_cardDatabase.GetRandomCard();
            CardModel model = new CardModel(data, CardColor.Green);
            instance.Initialize(model, false, true, true);
            lastCardTask = instance.MoveCardAsync(instance.transform.localPosition, Quaternion.identity, placeholder.localPosition, Quaternion.identity, time, initialDelay);
            instance.transform.localScale *= 2;
            instance.BaseLocalScale = instance.transform.localScale;
            initialDelay += 0.15f;
            time -= 0.05f;

            instance.OnCardClicked += HandleOnCardClicked;
            m_cards.Add(instance);
        }

        await lastCardTask;

        m_getButton.gameObject.SetActive(true);
        m_getButton.interactable = false;
        m_headerText.gameObject.SetActive(true);
    }

    private void HandleOnCardClicked(object sender, CardPresenter card)
    {
        foreach (var presenter in m_cards)
        {
            if (presenter == card)
                continue;

            presenter.IsHoverAnimationEnabled = true;

            presenter.HoverToInitialPosition();
        }

        m_selectedCard = card.Model.CardData;

        m_getButton.interactable = true;
        card.IsHoverAnimationEnabled = false;
    }

    public void Get()
    {
        m_player.Deck.Add(m_selectedCard);

        foreach (var presenter in m_cards)
        {
            presenter.OnCardClicked -= HandleOnCardClicked;
            GameObject.Destroy(presenter.gameObject);
        }

        m_cards.Clear();

        m_selectedCard = null;
        gameObject.SetActive(false);

        if (m_rewardPicked != null)
            m_rewardPicked.SetResult(true);
    }
}
