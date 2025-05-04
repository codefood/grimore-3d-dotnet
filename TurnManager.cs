using System.Collections.Generic;
using Godot;

namespace grimore3ddotnet;

public class TurnManager
{
    private readonly Queue<IActor> _actors = new();
    
    public void Enrol(IActor actor)
    {
        _actors.Enqueue(actor);    
    }

    public bool IsCurrentTurn(IActor actor) => 
        Current == actor;

    public IActor StartNextTurn()
    {
        GD.Print($"StartNextTurn, Current: {Current}, Queue of: {_actors.Count}");
        Current = _actors.Dequeue();
        _actors.Enqueue(Current);
        return Current;
    }

    private IActor Current { get; set; }


    public void Clear() => 
        _actors.Clear();
}