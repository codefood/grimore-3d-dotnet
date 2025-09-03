using System.Linq;
using Godot;

namespace Grimore.Entities;

public partial class Enemy : AnimatableBody3D, IActor, IInteractable
{
    public bool PlayerInteraction()
    {
        GD.Print("enemy taking damage");
        IActor.InvokeDying(this);
        return false;
    }

    public void StartTurn()
    {
        var direction = Actions.Directions
            .ElementAt(GD.RandRange(0, Actions.Directions.Count - 1))
            .Value;

        IActor.InvokeActing(new Move(this, direction));
    }
}