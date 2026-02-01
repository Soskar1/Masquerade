using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;

public class BoardPresenter : MonoBehaviour
{
    [SerializeField] private RectTransform m_placeholderParent;
    [SerializeField] private CardPresenter m_ghostCardPrefab;
    [SerializeField] private float m_cardMovementDuration;

    [SerializeField] private float m_revealStagger = 0.05f;

    private Dictionary<CardPresenter, CardPresenter> m_cardToPlaceholder;

    private EntityModel m_entity;
    private BoardModel m_board;
    private HandModel m_hand;
    private ManaModel m_mana;
    private BattleModel m_battle;

    public BoardModel BoardModel => m_board;
    public List<CardPresenter> Cards => m_cardToPlaceholder.Keys.ToList();

    private bool m_canInteractWithCards = false;

    public void Initialize(EntityModel entity, BattleModel battle)
    {
        m_entity = entity;
        m_board = entity.Board;
        m_hand = entity.Hand;
        m_mana = entity.Mana;
        m_battle = battle;
        m_board.OnCardAdded += HandleOnCardAdded;
        m_board.OnCardRemoved += HandleOnCardRemoved;
        m_battle.OnTurnStarted += HandleOnTurnStarted;
        m_battle.OnTurnEnded += HandleOnTurnEnded;

        m_cardToPlaceholder = new Dictionary<CardPresenter, CardPresenter>();
    }

    private void HandleOnTurnStarted(object sender, System.EventArgs e)
    {
        m_canInteractWithCards = true;
    }

    private async void HandleOnTurnEnded(object sender, System.EventArgs e)
    {
        m_canInteractWithCards = false;

        if (!m_entity.IsPlayer)
            await RevealCardsAsync();
    }

    private void OnDisable()
    {
        Disable();
    }

    public void Disable()
    {
        m_board.OnCardAdded -= HandleOnCardAdded;
        m_board.OnCardRemoved -= HandleOnCardRemoved;
        m_battle.OnTurnStarted -= HandleOnTurnStarted;
        m_battle.OnTurnEnded -= HandleOnTurnEnded;
    }

    private void HandleOnCardAdded(object sender, CardModel card)
    {
        CardPresenterRegistry.TryGet(card, out CardPresenter presenter);

        presenter.IsHoverAnimationEnabled = false;

        CardPresenter placeholderInstace = Instantiate(m_ghostCardPrefab, m_placeholderParent);

        m_cardToPlaceholder.Add(presenter, placeholderInstace);
        presenter.transform.SetParent(transform, true);
        presenter.transform.localScale = presenter.BaseLocalScale;

        RealignCards();

        presenter.OnCardClicked += HandleOnCardClicked;
    }

    private void HandleOnCardRemoved(object sender, OnCardRemovedEventArgs args)
    {
        CardPresenterRegistry.TryGet(args.CardModel, out CardPresenter presenter);

        CardPresenter placeholder = m_cardToPlaceholder[presenter];
        m_cardToPlaceholder.Remove(presenter);
        GameObject.Destroy(placeholder.gameObject);

        if (args.DeleteFromGame)
        {
            CardPresenterRegistry.Unregister(args.CardModel);
            GameObject.Destroy(presenter.gameObject);
        }

        RealignCards();
    }

    private void HandleOnCardClicked(object sender, CardPresenter presenter)
    {
        if (!m_canInteractWithCards)
            return;

        presenter.OnCardClicked -= HandleOnCardClicked;

        m_mana.CurrentMana += presenter.Model.CurrentCost;

        m_board.Remove(presenter.Model);
        m_hand.Add(presenter.Model);
    }

    private void RealignCards()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_placeholderParent);

        foreach (var cardAndPlaceholder in m_cardToPlaceholder)
        {
            CardPresenter actualCard = cardAndPlaceholder.Key;
            CardPresenter placeholder = cardAndPlaceholder.Value;

            actualCard.MoveCard(actualCard.transform.localPosition, actualCard.transform.rotation, placeholder.transform.localPosition, placeholder.transform.rotation, m_cardMovementDuration, 0);
        }
    }

    public async Task RevealCardsAsync()
    {
        var cards = m_cardToPlaceholder.Keys.ToList();
        var tasks = new List<Task>(cards.Count);

        for (int i = 0; i < cards.Count; i++)
        {
            tasks.Add(cards[i].RevealAsync());
        }

        await Task.WhenAll(tasks);
    }

    public async Task BuffBoard()
    {
        var cards = m_cardToPlaceholder.Keys.ToList();

        for (int i = 0; i < cards.Count ; i++)
        {
            CardPresenter card = cards[i];

            if (card.Model.Modifiers.Count > 0)
            {
                foreach (var modifier in card.Model.Modifiers)
                {
                    await BuffColorCards(modifier);
                }
            }
        }
    }

    private async Task BuffColorCards(ModifierModel modifier)
    {
        var cards = m_cardToPlaceholder.Keys.ToList();
        List<Task> tasks = new List<Task>(cards.Count);

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].Model.CardColor == modifier.CardColor)
            {
                Task task = cards[i].StartBuffAnimation(modifier);
                tasks.Add(task);
                await Task.Delay(50);
            }
        }

        await Task.WhenAll(tasks);
    }
}
