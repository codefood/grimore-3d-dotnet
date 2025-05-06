using System.Collections.Generic;
using Godot;

namespace Grimore;

public class TurnManager(World world)
{
    private readonly Queue<IActor> _actors = new();

    public delegate void TurnStarted(IActor actor);

    public event TurnStarted OnTurnStart;
    public void Enrol(IActor actor)
    {
        actor.Acting += PerformAction;
        _actors.Enqueue(actor);    
    }
    
    private void PerformAction(Command action)
    {
        if (!IsCurrentTurn(action.Actor)) return;
		
        switch (action)
        {
            case Summon spell:
                world.AddChild(spell.Instance);
                Enrol(spell.Instance);
                break;
            case Move move:
                action.Actor.Position += new Vector3(move.Direction.X * World.TileSize, 0, move.Direction.Y * World.TileSize);
                break;
        }

        StartNextTurn();
    }

    private bool IsCurrentTurn(IActor actor) => 
        Current == actor;

    public void StartNextTurn()
    {
        Current = _actors.Dequeue();
        _actors.Enqueue(Current);
        GD.Print($"StartNextTurn, Current: {((Node)Current).Name}, Queue of: {_actors.Count}");
        OnTurnStart.Invoke(Current);
        Current.StartTurn();
        
    }

    private IActor Current { get; set; }


    public void Clear() => 
        _actors.Clear();
}