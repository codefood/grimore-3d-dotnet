using System.Linq;
using Godot;

namespace Grimore;

public partial class World : Node3D
{
	public const int TileSize = 2;
	private readonly LevelLoader _levelLoader = new();
	public TurnManager Turner => FindChildren("TurnInstance").First() as TurnManager;
	
	private Timer _timer;
	private Ui Interface => FindChild("UI") as Ui;
	private Entities.Player Player => GetChildren().OfType<Entities.Player>().First();
	private Camera Camera => FindChildren("Camera").OfType<Camera>().First();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Turner == null)
		{
			GD.Print("No turnmanager in scene");
			return;
		}
		Turner.Setup(this);
		Turner.OnTurnStart += Interface.UpdateCurrentTurn;
		
		Interface.ChangeSpellColour += colour => Player.SpellColor = colour;
		Interface.ToggleCamera += () =>
		{
			Camera.SetMode(Camera.CurrentMode == Camera.Mode.isometric
				? Camera.Mode.thirdPerson
				: Camera.Mode.isometric);
		};
		Interface.Damage += () =>
		{
			Player.TakeDamage();
		};
		
		_levelLoader.Load(this, @"""
WWDWWWWWWWWWWWW
W       E     W
W       WW    W
D             D
W   WW E      W
W             W
W       D     W
W             W
WW           WW
W       n     W
""");

		Camera.SetMode(Camera.Mode.isometric);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}