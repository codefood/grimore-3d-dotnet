using System.Linq;
using Godot;
using Grimore.Entities;
using Grimore.Loaders;
using static Grimore.Quest;

namespace Grimore;

public partial class World : Node3D
{
	public const int TileSize = 2;
	public const float HalfTileSize = 1f;

	public TurnManager Turner =>
		GetNode<TurnManager>("TurnInstance");
	
	private Timer _timer;

	public Ui Interface =>
		GetNode<Ui>("UI");
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

		Player.HealthChanged += Interface.UpdateHealth;
		
		Interface.ToggleCamera += () =>
		{
			Camera.SetMode(Camera.CurrentMode == Camera.Mode.Isometric
				? Camera.Mode.ThirdPerson
				: Camera.Mode.Isometric);
		};

		OnUpdate += () => Interface.UpdateQuest(Quest);
		
		States.Playing.OnEnter += () =>
		{
			LevelLoader.Load(this, 1);
			QuestLoader.Load(this);
			TurnLoader.Load(this);
			
			Interface.UpdateQuest(Quest);
			
			Player.Keys = 0;
			Player.Health = 3;
			
			Interface.UpdateHealth(Player.Health);
			Turner.StartNextTurn();
		};
		
		Camera.SetMode(Camera.Mode.Isometric);
		GameState.Start();
	}

}