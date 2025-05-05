using Godot;

namespace Grimore;

public class Summon(IActor actor, Spell instance) : Command(actor)
{
    public Spell Instance { get; } = instance;
}
public class Move(IActor actor, Vector2 direction) : Command(actor)
{
    public Vector2 Direction { get; } = direction;
}
public abstract class Command(IActor actor)
{
    public IActor Actor { get; } = actor;
}