using System;
using System.Collections.Generic;
using System.Linq;
using DialogueManagerRuntime;
using Godot;
using Grimore.Entities;

namespace Grimore;

public partial class TurnManager : Node
{
    private Queue<IActor> _actors = new();
    private Timer _timer;
    private readonly List<GodotObject> _processed = new();
    private Vector3? _initial;
    private const int Speed = 2;
    public static event Action<Move> OnPlayerMove;
    public static event Action<IActor> OnTurnStart;
    public static event Action<IInteractable> OnInteractionSuccess;
    
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
            GameState.GameOver(false);
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
        if (States.Playing.Command != null) return;
		
        switch (action)
        {
            case CastSpell spell:
                AddChild(spell.Instance);
                InsertNextActor(spell.Instance);
                break;
            case Move move:
                States.Playing.Actor = action.Actor;
                if (action.Actor is Player) OnPlayerMove!.Invoke(move);
                _initial = ((PhysicsBody3D)action.Actor).Position;
                States.Playing.Command = move;
                break;
            case TargetSpell spell:
                States.Playing.Command = spell;
                return;
        }

        _timer.WaitTime = 1f / Speed;
        GD.Print($"starting timer, WaitTime: {_timer.WaitTime}");
        _timer.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (GameState.Current is not GameState.TurnState) return;
        if (States.Playing.Actor is not PhysicsBody3D actor) return;
        if (States.Playing.Command is not Move move) return;

        var direction = move.ToWorldDirection();
        
        var collisions = actor!.MoveAndCollide(direction * ((float)delta * Speed));

        EnumerateCollisions(collisions)
            .Except(_processed)
            .ForEach(collision => NewInteract(collision, actor));
    }

    private void NewInteract(GodotObject collision, PhysicsBody3D actor)
    {
        _processed.Add(collision);
        switch (collision)
        {
            case IInteractable interactable when actor is Player player:
            {
                InteractWith(collision, interactable, player);
                break;
            }
            case Player player when actor is IInteractable interactable:
            {
                InteractWith(collision, interactable, player);
                break;
            }
            default:
            {
                //this is the case where an enemy has hit a wall or a key or a door or something
                InteractWith(collision, actor as IInteractable, null);
                //GoBackwards();
                return;
            }
        }
    }

    private void InteractWith(GodotObject collision, IInteractable interactable, Player player)
    {
        GD.Print($"player interacted with {collision.GetType().Name}");
        if (interactable.Interact(player))
        {
            OnInteractionSuccess!.Invoke(interactable);
        }
        else
        {
            var actorNode = States.Playing.Actor as Node3D;
            actorNode!.Position = _initial!.Value;
            TurnTimer();
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
        States.Playing.Command = null;        
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
        States.Playing.Command = null;
        _initial = null;
    }

    public void Clear()
    {
        Reset();
        _actors.Clear();
    }
}
