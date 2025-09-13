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
    private readonly List<GodotObject> _processed = new();
    private Vector3? _initial;
    private const int Speed = 2;

    public delegate void TurnStarted(IActor actor);
    public event TurnStarted OnTurnStart;
    
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
        
        States.Paused.OnEnter += () => _timer.Paused = true;
        States.Playing.OnEnter += () => _timer.Paused = false;
        States.Ended.OnEnter += () => _timer.Paused = true;
        
        DialogueManager.DialogueStarted += _ => GameState.Pause();
        DialogueManager.DialogueEnded += _ => GameState.Start();
        
        IActor.Acting += PerformAction;
        IActor.Dying += DieAndFree;
    }
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
        if (toDelete == States.Playing.Actor)
        {
            StartNextTurn();
        }

        ((Node)toDelete).QueueFree();
    }

    private void PerformAction(Command action)
    {
        if (GameState.Current is not GameState.TurnState) return;
        if (States.Playing.Actor != action.Actor) return;
		
        switch (action)
        {
            case CastSpell spell:
                _world.AddChild(spell.Instance);
                InsertNextActor(spell.Instance);
                break;
            case Move move:
                States.Playing.Actor = action.Actor;
                _initial = ((PhysicsBody3D)action.Actor).Position;
                _direction = new Vector3(move.Direction.X * World.TileSize, 0, move.Direction.Y * World.TileSize);
                break;
        }
        
        _timer.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (GameState.Current is not GameState.TurnState) return;
        
        if (States.Playing.Actor == null) return;

        if (States.Playing.Actor is Spell)
        {
            _direction += new Vector3(0, 0.1f, 0);
        }
        var actor = States.Playing.Actor as PhysicsBody3D;
        
        var collisions = actor.MoveAndCollide(_direction * (float)delta * Speed);

        foreach(var collision in EnumerateCollisions(collisions).Except(_processed))
        {
            switch (collision)
            {
                case IInteractable interactor when actor is Player p:
                    GD.Print($"{((Node)interactor).Name} collided with {actor.Name}");
                    if (!interactor.Interact(p))
                    {
                        _direction = Vector3.Zero;
                        actor.Position = _initial!.Value;
                    }
                    else 
                    {
                        _world.Quest.InteractionSuccess(interactor);    
                    }
                    break;
                case Player p when actor is IInteractable interactor:
                    GD.Print($"{actor} collided with player");
                    if (!interactor.Interact(p))
                    {
                        _direction = Vector3.Zero;
                        actor.Position = _initial!.Value;
                    }
                    break;
                case IInteractable when actor is IInteractable thing:
                    GD.Print($"{actor.Name} hit a thing and is being moved back to {_initial}");
                    thing.Interact(null);
                    _direction = Vector3.Zero;
                    actor.Position = _initial!.Value;
                    break;
                default:
                    _direction = Vector3.Zero;
                    actor.Position = _initial!.Value;
                    break;
            }
            _processed.Add(collision);
        }
        
    }
    
    static IEnumerable<GodotObject> EnumerateCollisions(KinematicCollision3D collisions)
    {
        var count = collisions?.GetCollisionCount() ?? 0;
        for (var idx = 0; idx < count; idx++)
        {
            yield return collisions!.GetCollider(idx);
        }
    }

    public void StartNextTurn()
    {
        States.Playing.Actor = _actors.Dequeue();
        _actors.Enqueue(States.Playing.Actor);
        OnTurnStart!.Invoke(States.Playing.Actor);
        States.Playing.Actor.StartTurn();
    }

    private void TurnTimer()
    {
        Reset();
        StartNextTurn();
    }

    private void Reset()
    {
        _timer.Stop();
        _processed.Clear();
        _direction = Vector3.Zero;
        _initial = null;
    }
    
    public void Setup(World world) => _world = world;

    public void Clear()
    {
        Reset();
        _actors.Clear();
    }
}
