using System;

public class ManaModel
{
    private int m_currentMana;
    private int m_maxMana;

    public int CurrentMana
    {
        get => m_currentMana;
        set
        {
            if (value < 0) value = 0;
            if (value > m_maxMana) value = m_maxMana;

            m_currentMana = value;
            
            OnManaChangedEventArgs args = new OnManaChangedEventArgs(m_currentMana, m_maxMana);
            OnManaChanged?.Invoke(this, args);
        }
    }

    public int MaxMana
    {
        get => m_maxMana;
        set
        {
            if (value < 0) value = 0;

            m_maxMana = value;
            OnManaChangedEventArgs args = new OnManaChangedEventArgs(m_currentMana, m_maxMana);
            OnManaChanged?.Invoke(this, args);
        }
    }

    public event EventHandler<OnManaChangedEventArgs> OnManaChanged;

    public ManaModel(int mana)
    {
        m_maxMana = mana;
        m_currentMana = mana;
    }
}
