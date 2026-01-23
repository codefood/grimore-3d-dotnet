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
    private Timer _resetTimer;
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
            .ForEach(collision => Interact(collision, actor));
    }

    private void Interact(GodotObject collision, PhysicsBody3D actor)
    {
        switch (collision)
        {
            case IInteractable interactor when actor is Player p:
                GD.Print($"{((Node)interactor).Name} collided with {actor.Name}");
                if (!interactor.Interact(p))
                {
                    States.Playing.Command!.Cancel();
                    GoBackwards();
                }
                else
                {
                    OnInteractionSuccess!.Invoke(interactor);
                }
                break;
            case Player p when actor is IInteractable interactor:
                GD.Print($"{actor} collided with player");
                if (!interactor.Interact(p))
                {
                    States.Playing.Command!.Cancel();
                    GoBackwards();
                }

                break;
            case IInteractable when actor is IInteractable thing:
                GD.Print($"{actor.Name} hit a thing and is being moved back to {_initial}");
                thing.Interact(null);
                States.Playing.Command!.Cancel();
                GoBackwards();
                break;
            default:
                States.Playing.Command!.Cancel();
                GoBackwards();
                break;
        }

        _processed.Add(collision);
    }

    private void GoBackwards()
    {
        var elapsed = Mathf.Max(_timer.WaitTime - _timer.TimeLeft, 0.01f);
        GD.Print($"resetting timer, WaitTime: {_timer.WaitTime}, TImeLeft: {_timer.TimeLeft} == {elapsed}");
        _timer.Stop();
        
        _resetTimer = new Timer()
        {
            Autostart = true,
            WaitTime = elapsed,
            OneShot = true,
            Name = $"reset timer",
        };
        _resetTimer.Timeout += TurnTimer;
        AddChild(_resetTimer);
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
        if (_resetTimer != null)
        {
            _resetTimer.Timeout -= TurnTimer;
            _resetTimer.QueueFree();
            _resetTimer = null;
        }
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
