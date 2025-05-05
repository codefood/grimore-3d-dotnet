using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grimore;

public partial class World : Node3D
{
	private readonly WorldManager _worldManager = new();
	public readonly TurnManager TurnManager = new();

	public Player Player;
	private Node Interface => GetChildren().First(x => x.Name == "UI");

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
		AddChild(timer); //so how many of these do we end up with?
		enemy.Position += new Vector3(0, 0, 1);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TurnManager.OnTurnStart += actor => 
			Interface
					.GetChildren()
					.OfType<Label>()
					.Single()
					.Text = $"Current Turn: {actor.Name}";
		
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