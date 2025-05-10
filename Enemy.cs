using System.Linq;
using Godot;

namespace Grimore;

public partial class Enemy : AnimatableBody3D, IActor
{
    private Timer _timer;

    public override void _Ready()
    {
        _timer = new Timer()
        {
            Autostart = false,
            WaitTime = 0.2f,
            OneShot = false,
            Name = $"enemy timer",
        };
        _timer.Timeout += TurnWaitCallback;
        AddChild(_timer);
    }

    
    private void TurnWaitCallback()
    {
        _timer.Stop();
        var direction = Actions.Directions
            .ElementAt(GD.RandRange(0, Actions.Directions.Count - 1))
            .Value;

        Acting.Invoke(new Move(this, direction));
    }

    public void TakeDamage() => 
        QueueFree();

    public event IActor.OnActing Acting;
    public void StartTurn()
    {
        _timer.Start();
    }
}