using System.Threading.Tasks;
using Godot;

namespace Grimore;

public interface IActor
{
    StringName Name { get; }
    delegate void OnActing(Command action);
    event OnActing Acting;

    delegate void OnDying(IActor actor);
    event OnDying Dying;
    void StartTurn();
    void TakeDamage();
}