namespace Grimore.Entities.Quests;

public abstract class Requirement(string verb, string interactionName)
{
    protected string InteractionName { get; } = interactionName;
    private string Verb { get; } = verb;
    public bool Met { get; set; }
    public virtual string DisplayText => 
        $"[{(Met ? "X" : " ")}] - {Verb} {InteractionName}";
}