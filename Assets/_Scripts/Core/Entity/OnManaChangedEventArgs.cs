using System;

public class OnManaChangedEventArgs : EventArgs
{
    public int CurrentMana { get; private set; }
    public int MaxMana { get; private set; }

    public OnManaChangedEventArgs(int currentMana, int maxMana)
    {
        CurrentMana = currentMana;
        MaxMana = maxMana;
    }
}
