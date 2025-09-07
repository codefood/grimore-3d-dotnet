using System.Linq;
using Godot;

namespace Grimore.Entities;

public partial class Enemy : AnimatableBody3D, IActor, IInteractable
{
    private Vector2 Direction { get; set; }

    private Vector2 RandomDirection() => 
        Actions.Directions
            .Values
            .Except([Direction])
            .ElementAt(GD.RandRange(0, Actions.Directions
                .Values
                .Except([Direction])
                .Count() - 1));

    public bool PlayerInteraction(Player player)
    {
        if(player == null) Direction = RandomDirection();
        player?.TakeDamage();
        return false;
    }

    public void StartTurn()
    {
        if(Direction == Vector2.Zero) Direction = RandomDirection();
        IActor.InvokeActing(new Move(this, Direction));
    }
}