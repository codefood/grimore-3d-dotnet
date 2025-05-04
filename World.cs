using System.Linq;
using Godot;

namespace grimore3ddotnet;

public partial class World : Node3D
{
	private readonly WorldManager _worldManager = new();
	public readonly TurnManager TurnManager = new();

	public Player Player;
	
	public override void _Input(InputEvent ev)
	{
		if (!TurnManager.IsCurrentTurn(Player)) return;
		
		var directionsPressed = Actions.Directions
			.Where(k => ev.IsActionPressed(k.Key))
			.ToList();
		
		if (directionsPressed.Count != 0)
		{
			var direction = directionsPressed.First().Value;
			Player.Position += new Vector3(direction.X, 0, direction.Y);
			ProcessTurn();
		}
		if (ev.IsActionPressed(Actions.Act))
		{
			Player.CastSpell(this);
			ProcessTurn();
		}

		if (ev.IsActionPressed(Actions.Clear))
		{
			_worldManager.ClearThingsFrom(this);
			ProcessTurn();
		}
	}

	private void ProcessTurn()
	{
		if (TurnManager.StartNextTurn() is not Enemy enemy) return;
		
		var timer = new Timer()
		{
			Autostart = true,
			WaitTime = 0.5f,
			OneShot = true,
			Name = $"enemy {enemy.Name} timer",
		};
		timer.Timeout += ProcessTurn;
		AddChild(timer);
		enemy.Position += new Vector3(0, 0, 1);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetChildren()
			.OfType<Player>()
			.First();
		_worldManager.LoadLevel(this, Levels.One);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}