using UnityEngine;

public class ManaPresenter : MonoBehaviour
{
    [SerializeField] private GameObject m_manaPrefab;

    private ManaModel m_manaModel;

    public void Initialize(ManaModel model)
    {
        m_manaModel = model;

        m_manaModel.OnManaChanged += HandleOnManaChanged;
    }
   

    private void OnDisable()
    {
        m_manaModel.OnManaChanged -= HandleOnManaChanged;
    }

    private void HandleOnManaChanged(object sender, OnManaChangedEventArgs e)
    {
        Debug.Log("TODO");
    }
}