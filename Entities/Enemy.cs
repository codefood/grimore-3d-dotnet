using System.Linq;
using Godot;

namespace Grimore.Entities;

public partial class Enemy : AnimatableBody3D, IActor, IInteractable
{
    public Vector2 Direction { get; set; } =
        Actions.Directions.ElementAt(GD.RandRange(0, Actions.Directions.Count - 1)).Value;
    
    public bool PlayerInteraction(Player player)
    {
        player?.TakeDamage();
        return false;
    }

    public void StartTurn()
    {
        IActor.InvokeActing(new Move(this, Direction));
    }
}