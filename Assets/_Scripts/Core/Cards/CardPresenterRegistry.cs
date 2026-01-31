using System.Collections.Generic;

public static class CardPresenterRegistry
{
    private static readonly Dictionary<CardModel, CardPresenter> m_lookup = new Dictionary<CardModel, CardPresenter>();

    public static void Register(CardModel model, CardPresenter presenter)
    {
        m_lookup[model] = presenter;
    }

    public static void Unregister(CardModel model)
    {
        m_lookup.Remove(model);
    }

    public static bool TryGet(CardModel model, out CardPresenter presenter) => m_lookup.TryGetValue(model, out presenter);
}