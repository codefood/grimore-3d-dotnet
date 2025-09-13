using System;
using System.Linq;
using Godot;
using Grimore.Entities;

namespace Grimore;

public partial class World : Node3D
{
	public const int TileSize = 2;
	public const float HalfTileSize = 1f;
	public TurnManager Turner => 
		FindChildren("TurnInstance").First() as TurnManager;
	
	private Timer _timer;
	private Ui Interface => 
		FindChild("UI") as Ui;
	public Entities.Player Player => 
		GetChildren().OfType<Entities.Player>().First();
	private Camera Camera => 
		FindChildren("Camera").OfType<Camera>().First();
	public Quest Quest { get; set; }

	public override void _Ready()
	{
		if (Turner == null)
		{
			GD.Print("No turnmanager in scene");
			return;
		}
		Turner.Setup(this);
		Turner.OnTurnStart += Interface.UpdateCurrentTurn;
		Player.HealthChanged += Interface.UpdateHealth;
		
		Interface.ToggleCamera += () =>
		{
			Camera.SetMode(Camera.CurrentMode == Camera.Mode.isometric
				? Camera.Mode.thirdPerson
				: Camera.Mode.isometric);
		};
		

		Quest.OnUpdate += () => Interface.UpdateQuest(Quest);
		
		States.Playing.OnEnter += () =>
		{
			LevelLoader.Load(this);
			Quest = new Quest(
				new InteractionSuccess("Collect", "Key 1"),
				new InteractionSuccess("Open", "Door 3")
			);	
			Interface.UpdateQuest(Quest);
			
			Player.Position = Vector3.Zero;
			Player.Keys = 0;
			Player.Health = 3;
			
			Interface.UpdateHealth(Player.Health);
			Turner.StartNextTurn();
		};
		
		Camera.SetMode(Camera.Mode.isometric);
		GameState.Start();
	}

}

public class Quest(params Requirement[] requirements)
{
	public readonly Requirement[] Requirements = requirements;

	public static event Action OnUpdate;

	public void InteractionSuccess(IInteractable interactor)
	{
		foreach (var requirement in Requirements
			         .OfType<InteractionSuccess>()
			         .Where(x => x.InteractionName == ((Node)interactor).Name))
		{
			requirement.Met = true;
		}

		OnUpdate?.Invoke();
	}
}
public class InteractionSuccess(string verb, string interactionName) : Requirement(verb)
{
	public string InteractionName { get; } = interactionName;
}
public abstract class Requirement(string verb)
{
	public string Verb { get; } = verb;
	public bool Met { get; set; }
}