using Godot;

namespace Grimore.Entities.Quests;

public class InteractionRequired(string verb, string interactionName) : 
    Requirement(verb, interactionName), 
    ICheckable<IInteractable>
{
    public bool Check(IInteractable against) => 
        InteractionName == ((Node)against).Name;
}