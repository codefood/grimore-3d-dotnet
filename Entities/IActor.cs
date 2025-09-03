using Godot;

namespace Grimore.Entities;

public interface IActor
{
    StringName Name { get; }
    delegate void OnActing(Command action);
    static event OnActing Acting;
    
    static void InvokeActing(Command action) => Acting!.Invoke(action);
    static void InvokeDying(IActor actor) => Dying!.Invoke(actor);

    delegate void OnDying(IActor actor);
    static event OnDying Dying;
    void StartTurn();
}