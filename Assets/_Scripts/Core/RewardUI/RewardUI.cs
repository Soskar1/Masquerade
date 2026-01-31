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

    public void Initialize(CardDatabase database)
    {
        m_cardDatabase = database;
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
        }

        await lastCardTask;

        m_getButton.gameObject.SetActive(true);
        m_headerText.gameObject.SetActive(true);
    }
}
