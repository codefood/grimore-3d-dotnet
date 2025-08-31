using Godot;

namespace Grimore.Entities;

public interface IActor
{
    StringName Name { get; }
    delegate void OnActing(Command action);
    event OnActing Acting;

    delegate void OnDying(IActor actor);
    event OnDying Dying;
    void StartTurn();
}