using System.Collections.Generic;
using UnityEngine;

public class MenuCard : MonoBehaviour
{

    public List<CardData> cards;
    private CardDatabase database;

    [SerializeField] private CardPresenter presenter;

    public void Awake()
    {
        database = new CardDatabase(cards);
    }


    public void ChangeCard()
    {
        presenter.Disable();

        CardData data = database.GetRandomCard();
        int value = Random.Range(0, System.Enum.GetValues(typeof(CardColor)).Length);
        CardColor color = (CardColor)value;

        var modifiers = DeckModel.GenerateRandomModifiers(true, 2, false, color);
        CardModel model = new CardModel(data, color, modifiers);

        
        presenter.Initialize(model);
    }
}
