using Godot;

namespace Grimore.Entities;

public partial class Key : Node3D, IActor
{
    public event IActor.OnActing Acting;
    public event IActor.OnDying Dying;
    public void StartTurn()
    {
        
    }

    public void TakeDamage()
    {
        //collected!
    }
}