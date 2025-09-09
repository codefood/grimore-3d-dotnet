using System.Linq;
using Godot;

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
		
		GameState.State.Started.OnEnter += () => LevelLoader.Load(this);

		Camera.SetMode(Camera.Mode.isometric);
		GameState.Start();
	}
}