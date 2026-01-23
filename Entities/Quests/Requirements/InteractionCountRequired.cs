namespace Grimore.Entities.Quests;

public class InteractionCountRequired<TInteractable>(string verb, string interactionName, int count) : 
    Requirement(verb, interactionName), 
    ICheckable<IInteractable>
    where TInteractable : IInteractable
{
    private int _total;

    public bool Check(IInteractable against)
    {
        var matches = against is TInteractable;
        if (matches) _total++;
        return _total >= count;
    }

    public override string DisplayText => 
        base.DisplayText + $" ({_total} / {count})";

}