using System;
using System.Linq;
using Godot;

namespace Grimore.Entities.Quests;

public partial class Quest(params Requirement[] requirements) : Node
{
    public readonly Requirement[] Requirements = requirements;

    public static event Action OnUpdate;

    public void Check<T>(T action)
    {
        Requirements
            .OfType<ICheckable<T>>()
            .Where(x => x.Check(action))
            .ForEach(x => x.Met = true);

        OnUpdate?.Invoke();
    }
    public bool Complete => 
        Requirements.All(x => x.Met);
	
    public class MoveSuccess(Vector2 direction, string directionName) : 
        Requirement("Move", directionName), 
        ICheckable<Move>
    {
        public bool Check(Move against) => 
            against.Direction == direction;
    }
}