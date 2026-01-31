using System.Collections.Generic;
using UnityEngine;

public class ManaPresenter : MonoBehaviour
{
    [SerializeField] private GameObject m_manaPrefab;
    private List<GameObject> m_currentMana;

    private ManaModel m_manaModel;

    public void Initialize(ManaModel model)
    {
        m_currentMana = new List<GameObject>();
        m_manaModel = model;
        DisplayMana(m_manaModel.CurrentMana);

        m_manaModel.OnManaChanged += HandleOnManaChanged;
    }

    private void OnDisable()
    {
        m_manaModel.OnManaChanged -= HandleOnManaChanged;
    }

    private void HandleOnManaChanged(object sender, OnManaChangedEventArgs e)
    {
        DisplayMana(e.CurrentMana);
    }

    private void DisplayMana(int currentMana)
    {
        for (int i = 0; i < currentMana; ++i)
        {
            GameObject instance = Instantiate(m_manaPrefab, transform);
            m_currentMana.Add(instance);
        }
    }
}