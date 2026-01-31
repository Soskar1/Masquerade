using System;

public class OnHealthChangedEventArgs : EventArgs
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    public OnHealthChangedEventArgs(int currentHealth, int maxHealth)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}