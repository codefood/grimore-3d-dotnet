using System.Collections.Generic;
using System.Linq;
using DialogueManagerRuntime;
using Godot;
using Grimore.Entities;

namespace Grimore;

public partial class TurnManager : Node
{
    private readonly Queue<IActor> _actors = new();
    private World _world;
    private Timer _timer;
    private Vector3 _direction;
    private PhysicsBody3D _actor;
    private readonly List<GodotObject> _processed = new();
    private Vector3? _initial;
    private const int Speed = 2;
    private bool _paused = false;

    public delegate void TurnStarted(IActor actor);
    public override void _Ready()
    {
        base._Ready();
        _timer = new Timer()
        {
            Autostart = false,
            WaitTime = 1f / Speed,
            OneShot = false,
            Name = $"turn timer",
        };
        _timer.Timeout += TurnTimer;
        AddChild(_timer);
        
        DialogueManager.DialogueStarted += _ =>
        {
            _timer.Paused = true;
            _paused = false;
        };
        DialogueManager.DialogueEnded += _ =>
        {
            _timer.Paused = false;
            _paused = false;
        };
        IActor.Acting += PerformAction;
        IActor.Dying += DieAndFree;
    }
    public event TurnStarted OnTurnStart;
    public void Enrol(IActor actor) => 
        _actors.Enqueue(actor);

    private void DieAndFree(IActor toDelete)
    {
        for (var i = 0; i < _actors.Count - 1; i++)
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
        if (!(!_paused && Current == action.Actor)) return;
		
        switch (action)
        {
            case Summon spell:
                _world.AddChild(spell.Instance);
                Enrol(spell.Instance);
                break;
            case Move move:
                _actor = (PhysicsBody3D)action.Actor;
                _initial = _actor.Position;
                _direction = new Vector3(move.Direction.X * World.TileSize, 0, move.Direction.Y * World.TileSize);
                break;
        }
        
        _timer.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if(_actor == null) return;
        
        var collisions = _actor.MoveAndCollide(_direction * (float)delta * Speed);

        foreach(var collision in collisions.EnumerateCollisions().Except(_processed))
        {
            switch (collision)
            {
                case IInteractable interactor when _actor is Player p:
                    GD.Print($"player collided with {((Node)interactor).Name}");
                    if (!interactor.PlayerInteraction(p))
                    {
                        _direction = Vector3.Zero;
                        _actor.Position = _initial!.Value;
                    }
                    break;
                case Player p when _actor is IInteractable interactor:
                    GD.Print($"{_actor.Name} collided with player");
                    if (!interactor.PlayerInteraction(p))
                    {
                        _direction = Vector3.Zero;
                        _actor.Position = _initial!.Value;
                    }
                    break;
                default:
                    _direction = Vector3.Zero;
                    _actor.Position = _initial!.Value;
                    break;
            }
            _processed.Add(collision);
        }
        
    }

    public void StartNextTurn()
    {
        Current = _actors.Dequeue();
        _actors.Enqueue(Current);
        //GD.Print($"StartNextTurn, Current: {((Node)Current).Name}, Queue of: {_actors.Count}");
        OnTurnStart!.Invoke(Current);
        Current.StartTurn();
    }

    private void TurnTimer()
    {
        _timer.Stop();
        _processed.Clear();
        _actor = null;
        _direction = Vector3.Zero;
        _initial = null;
        StartNextTurn();
    }
    
    private IActor Current { get; set; }
    public void Setup(World world) => _world = world;

    public void Clear() => 
        _actors.Clear();
}
