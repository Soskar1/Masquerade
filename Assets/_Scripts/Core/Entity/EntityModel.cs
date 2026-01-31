public class EntityModel
{
    public HealthModel Health { get; private set; }
    public HandModel Hand { get; private set; }
    public DeckModel Deck { get; private set; }
    public ManaModel Mana { get; private set; }
    public BoardModel Board { get; private set; }
    public bool IsPlayer { get; private set; }

    public EntityModel(EntityData data, bool isPlayer)
    {
        Health = new HealthModel(data.Health);
        Deck = new DeckModel(data.CardPool);
        Hand = new HandModel(Deck, data.HandSize);
        Mana = new ManaModel(data.MaxMana);
        Board = new BoardModel();
        IsPlayer = isPlayer;
    }
}
