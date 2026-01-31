using System;

public class OnCardRemovedEventArgs : EventArgs
{
    public CardModel CardModel { get; private set; }
    public bool DeleteFromGame { get; private set; }

    public OnCardRemovedEventArgs(CardModel cardModel, bool deleteFromGame)
    {
        this.CardModel = cardModel;
        this.DeleteFromGame = deleteFromGame;
    }
}
