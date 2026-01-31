using System;

public class HealthModel
{
    private int m_currentHealth;
    private int m_maxHealth;

    public int CurrentHealth
    {
        get => m_currentHealth;
        set
        {
            m_currentHealth = value;

            OnHealthChangedEventArgs args = new OnHealthChangedEventArgs(m_currentHealth, m_maxHealth);
            OnHealthChanged?.Invoke(this, args);
        }
    }

    public int MaxHealth
    {
        get => m_maxHealth;
        set
        {
            m_maxHealth = value;

            OnHealthChangedEventArgs args = new OnHealthChangedEventArgs(m_currentHealth, m_maxHealth);
            OnHealthChanged?.Invoke(this, args);
        }
    }

    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;

    public HealthModel(int currentHealth, int maxHealth)
    {
        m_currentHealth = currentHealth;
        m_maxHealth = maxHealth;
    }
}
