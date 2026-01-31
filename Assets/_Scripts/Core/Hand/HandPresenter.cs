using System.Collections.Generic;
using UnityEngine;

public class HandPresenter : MonoBehaviour
{
    [SerializeField] private CardPresenter m_cardPresenterPrefab;

    [Header("Fan Settings")]
    [SerializeField] private float m_maxFanAngle = 15f;
    [SerializeField] private float m_curveHeight = 40f;
    [SerializeField] private float m_cardSpacing = 150f;

    // Note: A hacky way to place cards on the Y axis
    [SerializeField] private float m_offsetY = 100f;

    private List<CardPresenter> m_cardsInHand;
    private HandModel m_handModel;

    public void Initialize(HandModel model)
    {
        m_handModel = model;
        m_handModel.OnHandChanged += HandleOnHandChanged;
        m_cardsInHand = new List<CardPresenter>();
    }

    private void HandleOnHandChanged(object sender, List<CardModel> e)
    {
        foreach (CardModel model in e)
        {
            CardPresenter instance = Instantiate(m_cardPresenterPrefab, transform);
            instance.Initialize(model);

            m_cardsInHand.Add(instance);
        }

        ApplyFanLayout();
    }

    private void ApplyFanLayout()
    {
        int count = m_cardsInHand.Count;
        if (count == 0) return;

        float totalWidth = (count - 1) * m_cardSpacing;
        float startX = -totalWidth * 0.5f;

        if (count == 1)
        {
            m_cardsInHand[0].transform.localRotation = Quaternion.identity;
            m_cardsInHand[0].transform.localPosition = Vector3.zero;
            return;
        }

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * m_cardSpacing;
            float t = Mathf.Lerp(-1f, 1f, (float)i / (count - 1));

            float angle = -t * m_maxFanAngle;
            float yOffset = -Mathf.Pow(Mathf.Abs(t), 2f) * m_curveHeight;

            Transform tr = m_cardsInHand[i].transform;
            tr.localPosition = new Vector3(x, yOffset + m_offsetY, 0f);
            tr.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}