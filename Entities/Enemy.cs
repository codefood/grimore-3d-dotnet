using System.Linq;
using Godot;

namespace Grimore.Entities;

public partial class Enemy : AnimatableBody3D, IActor
{
    public void TakeDamage()
    {
        GD.Print("enemy taking damage");
        Dying!.Invoke(this);
    }

    public event IActor.OnActing Acting;
    public event IActor.OnDying Dying;

    public void StartTurn()
    {
        var direction = Actions.Directions
            .ElementAt(GD.RandRange(0, Actions.Directions.Count - 1))
            .Value;

        Acting!.Invoke(new Move(this, direction));
    }
}