using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private CardPresenter m_test;
    [SerializeField] private CardData m_data;

    public void Awake()
    {
        m_test.Initialize(new CardModel(m_data));
    }
}
