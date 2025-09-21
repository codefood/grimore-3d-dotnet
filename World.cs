using System.Linq;
using Godot;
using Grimore.Entities;

namespace Grimore;

public partial class World : Node3D
{
	public const int TileSize = 2;
	public const float HalfTileSize = 1f;

	public TurnManager Turner =>
		GetNode<TurnManager>("TurnInstance");
	
	private Timer _timer;

	private Ui Interface =>
		GetNode<Ui>("UI");
	public Player Player => 
		GetChildren().OfType<Player>().First();
	private Camera Camera => 
		FindChildren("Camera").OfType<Camera>().First();
	public Quest Quest { get; private set; }

	public override void _Ready()
	{
		if (Turner == null)
		{
			GD.Print("No turnmanager in scene");
			return;
		}

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
			LevelLoader.Load(this, 1);
			Quest = new Quest(
				new Quest.MoveSuccess(Vector2.Up, "North"),
				new Quest.MoveSuccess(Vector2.Down, "South"),
				new Quest.MoveSuccess(Vector2.Left, "East"),
				new Quest.MoveSuccess(Vector2.Right, "West"),
				new Quest.InteractionRequired("Collect", "Key 1"),
				new Quest.InteractionRequired("Open", "Door 3")
			);
			
			TurnManager.OnPlayerMove += Quest.Moved;
			TurnManager.OnInteractionSuccess += Quest.InteractionSuccess;
			TurnManager.OnTurnStart += Interface.UpdateCurrentTurn;
			
			Interface.UpdateQuest(Quest);
			
			Player.Keys = 0;
			Player.Health = 3;
			
			Interface.UpdateHealth(Player.Health);
			Turner.StartNextTurn();
		};
		
		Camera.SetMode(Camera.Mode.isometric);
		GameState.Start();
	}

}