using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthPresenter : MonoBehaviour
{
    [SerializeField] private Image m_healthBar;
    [SerializeField] private TextMeshProUGUI m_text;
    private HealthModel m_healthModel;

    public void Register(HealthModel model)
    {
        if (m_healthModel != null)
            Unregister();

        m_healthModel = model;
        m_healthModel.OnHealthChanged += HandleOnHealthChangedEvent;

        UpdateHealthBar(m_healthModel.CurrentHealth, m_healthModel.MaxHealth);
    }

    private void OnDisable() => Unregister();
    public void Unregister()
    {
        if (m_healthModel == null)
            return;

        m_healthModel.OnHealthChanged -= HandleOnHealthChangedEvent;
        m_healthModel = null;
    }

    private void HandleOnHealthChangedEvent(object obj, OnHealthChangedEventArgs e) => UpdateHealthBar(e.CurrentHealth, e.MaxHealth);

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        m_healthBar.fillAmount = currentHealth / maxHealth;

        if (m_text != null)
            m_text.text = $"{currentHealth}/{maxHealth}";
    }
}