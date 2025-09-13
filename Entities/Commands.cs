using Godot;

namespace Grimore.Entities;

public class CastSpell(IActor actor, Spell instance) : Command(actor)
{
    public Spell Instance { get; } = instance;
}
public class Move(IActor actor, Vector2 direction) : Command(actor)
{
    private Vector2 Direction { get; set; } = direction;
    public Vector3 ToWorldDirection() => 
        new(Direction.X * World.TileSize, 0, Direction.Y * World.TileSize);

    public void Cancel() => Direction = -Direction;

}
public abstract class Command(IActor actor)
{
    public IActor Actor { get; } = actor;
}