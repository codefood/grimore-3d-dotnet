using System.Linq;
using Godot;
using Grimore.Entities;
using Grimore.Entities.Quests;
using Grimore.Loaders;
using static Grimore.Entities.Quests.Quest;

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

	public Camera Camera => 
		FindChildren("Camera").OfType<Camera>().First();
	public Quest Quest { get; set; }

	public override void _Ready()
	{
		Player.HealthChanged += Interface.UpdateHealth;
		
		Interface.ToggleCamera += () =>
		{
			Camera.SetMode(Camera.CurrentMode == Camera.Mode.Isometric
				? Camera.Mode.ThirdPerson
				: Camera.Mode.Isometric);
		};

		OnUpdate += () =>
		{
			Interface.UpdateQuest(Quest);
			if (Quest.Complete)
			{
				GameState.GameOver(true);
			}
		};
		
	}

}