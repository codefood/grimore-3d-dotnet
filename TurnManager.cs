using System.Collections.Generic;
using System.Linq;
using DialogueManagerRuntime;
using Godot;
using Grimore.Entities;

namespace Grimore;

public partial class TurnManager : Node
{
    private Queue<IActor> _actors = new();
    private World _world;
    private Timer _timer;
    private Vector3 _direction;
    private PhysicsBody3D _actor;
    private readonly List<GodotObject> _processed = new();
    private Vector3? _initial;
    private const int Speed = 2;

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
        
        GameState.State.Paused.OnEnter += () => _timer.Paused = true;
        GameState.State.Started.OnEnter += () => _timer.Paused = false;
        GameState.State.Ended.OnEnter += () => _timer.Paused = true;
        
        DialogueManager.DialogueStarted += _ => GameState.Pause();
        DialogueManager.DialogueEnded += _ => GameState.Start();
        
        IActor.Acting += PerformAction;
        IActor.Dying += DieAndFree;
    }
    public event TurnStarted OnTurnStart;
    public void Enrol(IActor actor) =>
        _actors.Enqueue(actor);

    private void InsertNextActor(IActor actor) => 
        _actors = new Queue<IActor>([actor, .._actors.ToList()]);

    private void DieAndFree(IActor toDelete)
    {
        _actors = new Queue<IActor>(_actors.ToList().Except([toDelete]));
        if (toDelete is Player)
        {
            GameState.GameOver();
            return;
        }
        if (toDelete == Current)
        {
            StartNextTurn();
        }

        ((Node)toDelete).QueueFree();
    }

    private void PerformAction(Command action)
    {
        if (GameState.Current != GameState.State.Started) return;
        if (Current != action.Actor) return;
		
        switch (action)
        {
            case CastSpell spell:
                _world.AddChild(spell.Instance);
                InsertNextActor(spell.Instance);
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
        
        if (GameState.Current != GameState.State.Started) return;
        
        if(_actor == null) return;

        if (_actor is Spell)
        {
            _direction += new Vector3(0, 0.1f, 0);
        }
        
        var collisions = _actor.MoveAndCollide(_direction * (float)delta * Speed);

        foreach(var collision in collisions.EnumerateCollisions().Except(_processed))
        {
            switch (collision)
            {
                case IInteractable when _actor is Spell spell:
                    GD.Print("Spell hit a thing");
                    spell.PlayerInteraction(null);
                    _direction = Vector3.Zero;
                    _actor.Position = _initial!.Value;
                    break;
                case IInteractable interactor when _actor is Player p:
                    GD.Print($"{((Node)interactor).Name} collided with {_actor.Name}");
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
