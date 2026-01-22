using System;
using System.Linq;
using Godot;
using Grimore.Entities;

namespace Grimore;

public class Quest(params Quest.Requirement[] requirements)
{
    public readonly Requirement[] Requirements = requirements;

    public static event Action OnUpdate;

    public void Moved(Move move)
    {
        Requirements
            .OfType<ICheckable<Move>>()
            .Where(x => x.Check(move))
            .ForEach(x => x.Met = true);
		
        OnUpdate?.Invoke();
    }
	
    public void InteractionSuccess(IInteractable interactor)
    {
        Requirements
            .OfType<ICheckable<IInteractable>>()
            .Where(x => x.Check(interactor))
            .ForEach(x => x.Met = true);
		
        OnUpdate?.Invoke();
    }

    public bool Complete => 
        Requirements.All(x => x.Met);
	
    public class MoveSuccess(Vector2 direction, string directionName) : Requirement("Move", directionName), ICheckable<Move>
    {
        public bool Check(Move against) => 
            against.Direction == direction;
    }

    public class InteractionRequired(string verb, string interactionName) : Requirement(verb, interactionName), ICheckable<IInteractable>
    {
        public bool Check(IInteractable against) => 
            InteractionName == ((Node)against).Name;
    }

    private interface ICheckable<in T>
    {
        bool Check(T against);
        bool Met { get; set; }
    }
    public abstract class Requirement(string verb, string interactionName)
    {
        protected string InteractionName { get; } = interactionName;
        private string Verb { get; } = verb;
        public bool Met { get; set; }
        public string DisplayText => $"[{(Met ? "X" : " ")}] - {Verb} {InteractionName}";
    }
}