using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grimore;

public partial class TurnManager : Node
{
    private readonly Queue<IActor> _actors = new();
    private World _world;
    private Timer _timer;
    private Vector3 _velocity;
    private PhysicsBody3D _actor;
    private List<GodotObject> _processed = new();

    public delegate void TurnStarted(IActor actor);

    public event TurnStarted OnTurnStart;
    public void Enrol(IActor actor)
    {
        actor.Acting += PerformAction;
        actor.Dying += DieAndFree;
        _actors.Enqueue(actor);    
    }

    private void DieAndFree(IActor toDelete)
    {
        for (int i = 0; i < _actors.Count - 1; i++)
        {
            var c = _actors.Dequeue();
            if (c == toDelete)
            {
                ((Node)c).QueueFree();
                continue;
            }
            _actors.Enqueue(c);
        }
    }

    private void PerformAction(Command action)
    {
        if (!IsCurrentTurn(action.Actor)) return;
		
        switch (action)
        {
            case Summon spell:
                _world.AddChild(spell.Instance);
                Enrol(spell.Instance);
                break;
            case Move move:
                //ProcessMove(action);
                _actor = action.Actor as PhysicsBody3D;
                _velocity = new Vector3(move.Direction.X * World.TileSize, 0, move.Direction.Y * World.TileSize);
                break;
        }
        
        _timer.Start();
    }

    private void TurnTimer()
    {
        _timer.Stop();
        _processed.Clear();
        _actor = null;
        _velocity = Vector3.Zero;
        StartNextTurn();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if(_actor == null) return;
        
        var collisions = _actor.MoveAndCollide(_velocity * (float)delta);
        foreach(var collision in collisions.EnumerateCollisions().Except(_processed))
        {
            switch (collision)
            {
                case Wall wall:
                    // lets not do anything
                    
                    return;
                case Enemy enemy:
                    GD.Print($"Enemy {enemy.Name} took damage from {_actor.Name}");
                    enemy.TakeDamage();
                    break;
                case Door door:
                    door.Open();
                    break;
                case Player p:
                    GD.Print($"{_actor.Name} collided with player");
                    //p.TakeDamage();
                    //action.Actor.Position = original;
                    break;
                case Spell spell:
                    ((IActor)_actor).TakeDamage();
                    break;
            }
            _processed.Add(collision);
        }
        
    }
    private bool IsCurrentTurn(IActor actor) => 
        Current == actor;

    public void StartNextTurn()
    {
        Current = _actors.Dequeue();
        _actors.Enqueue(Current);
        //GD.Print($"StartNextTurn, Current: {((Node)Current).Name}, Queue of: {_actors.Count}");
        OnTurnStart!.Invoke(Current);
        Current.StartTurn();
        
    }

    public override void _Ready()
    {
        base._Ready();
        _timer = new Timer()
        {
            Autostart = false,
            WaitTime = 1f,
            OneShot = false,
            Name = $"turn timer",
        };
        _timer.Timeout += TurnTimer;
        AddChild(_timer);
    }

    private IActor Current { get; set; }
    public void Setup(World world) => _world = world;

    public void Clear() => 
        _actors.Clear();
}
