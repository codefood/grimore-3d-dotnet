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
	public Player Player => 
		GetChildren().OfType<Player>().First();
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
		TurnManager.OnTurnStart += Interface.UpdateCurrentTurn;
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
				new MoveSuccess(Vector2.Up, "North"),
				new MoveSuccess(Vector2.Down, "South"),
				new MoveSuccess(Vector2.Left, "East"),
				new MoveSuccess(Vector2.Right, "West"),
				new InteractionSuccess("Collect", "Key 1"),
				new InteractionSuccess("Open", "Door 3")
			);	
			Interface.UpdateQuest(Quest);

			Player.Position = new Vector3(TileSize, 0, 0);
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
}
public class MoveSuccess(Vector2 direction, string directionName) : Requirement("Move", directionName), ICheckable<Move>
{
	public bool Check(Move against) => 
		against.Direction == direction;
}

public class InteractionSuccess(string verb, string interactionName) : Requirement(verb, interactionName), ICheckable<IInteractable>
{
	public bool Check(IInteractable against) => 
		InteractionName == ((Node)against).Name;
}

public interface ICheckable<in T>
{
	bool Check(T against);
	bool Met { get; set; }
}
public abstract class Requirement(string verb, string interactionName)
{
	public string InteractionName { get; } = interactionName;
	public string Verb { get; } = verb;
	public bool Met { get; set; }
	public string Display => $"[{(Met ? "X" : " ")}] - {Verb} {InteractionName}";

}