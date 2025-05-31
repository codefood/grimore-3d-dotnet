using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grimore;

public static class FooExt
{
    private static readonly Type[] PawnTypes = [typeof(Wall), typeof(Enemy), typeof(Spell)];

    public static IEnumerable<Node3D> GetPawns(this IEnumerable<Node3D> nodes) =>
        PawnTypes.SelectMany(p => nodes.Where(n => n.GetType() == p));
}
public class TurnManager(World world)
{
    private readonly Queue<IActor> _actors = new();

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
                world.AddChild(spell.Instance);
                Enrol(spell.Instance);
                break;
            case Move:
                ProcessMove(action);
                break;
        }

        StartNextTurn();
    }

    private void ProcessMove(Command action)
    { 
        var move = (Move)action;
      
        GD.Print($"{move.Actor.Name} is moving ({move.Direction.X}, {move.Direction.Y})");
        
        var original = action.Actor.Position;
        var target = new Vector3(move.Direction.X * World.TileSize, 0, move.Direction.Y * World.TileSize);
        
        //we move until we collide, then process those collisions, then continue
        var physicsObject = ((PhysicsBody3D)action.Actor);
        var collisions = physicsObject.MoveAndCollide(target);
        var count = collisions?.GetCollisionCount() ?? 0;
        
        for(int idx = 0; idx < count; idx++)
        {
            var collision = collisions!.GetCollider(idx);
            switch (collision)
            {
                case Wall wall:
                    // lets not do anything
                    action.Actor.Position = original;
                    return;
                case Enemy enemy:
                    GD.Print($"Enemy {enemy.Name} took damage from {action.Actor.Name}");
                    enemy.TakeDamage();
                    break;
                case Door door:
                    door.Open();
                    break;
                case Player p:
                    GD.Print($"{action.Actor.Name} collided with player");
                    //p.TakeDamage();
                    //action.Actor.Position = original;
                    break;
                case Spell spell:
                    action.Actor.TakeDamage();
                    break;
            }
        }
        //action.Actor.Position += collisions?.GetRemainder() ?? Vector3.Zero;
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

    private IActor Current { get; set; }


    public void Clear() => 
        _actors.Clear();
}