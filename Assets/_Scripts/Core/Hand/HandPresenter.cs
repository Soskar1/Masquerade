using System.Collections.Generic;
using UnityEngine;

public class HandPresenter : MonoBehaviour
{
    [SerializeField] private CardPresenter m_cardPresenterPrefab;
    [SerializeField] private Transform m_handParent;

    private HandModel m_handModel;

    public void Initialize(HandModel model)
    {
        m_handModel = model;
        m_handModel.OnHandChanged += HandleOnHandChanged;
    }

    private void HandleOnHandChanged(object sender, List<CardModel> e)
    {
        foreach (CardModel model in e)
        {
            CardPresenter instance = Instantiate(m_cardPresenterPrefab, m_handParent);
            instance.Initialize(model);
        }
    }
}