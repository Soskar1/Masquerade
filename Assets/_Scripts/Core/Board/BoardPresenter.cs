using UnityEngine;

public class BoardPresenter : MonoBehaviour
{
    [SerializeField] private Transform m_parent;

    private BoardModel m_model;

    public void Initialize(BoardModel model)
    {
        m_model = model;
        model.OnCardAdded += HandleOnCardAdded;
        model.OnCardRemoved += HandleOnCardRemoved;
    }

    private void OnDisable()
    {
        m_model.OnCardAdded -= HandleOnCardAdded;
        m_model.OnCardRemoved -= HandleOnCardRemoved;
    }

    private void HandleOnCardAdded(object sender, CardPresenter card)
    {
        card.transform.SetParent(m_parent, false);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = card.BaseLocalScale;
        card.ReactToMouseInput = false;
    }

    private void HandleOnCardRemoved(object sender, CardPresenter card)
    {
        // TODO
    }
}
