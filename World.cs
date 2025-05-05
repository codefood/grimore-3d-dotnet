using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grimore;

public partial class World : Node3D
{
	private readonly WorldManager _worldManager = new();
	public readonly TurnManager TurnManager = new();

	public Player Player;
	private Timer _timer;
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
			Player.Move(direction);
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

		var direction = Actions.Directions
			.ElementAt(GD.RandRange(0, Actions.Directions.Count - 1))
			.Value;

		enemy.Move(direction);
		_timer.Start();
	}

	private void EnemyTurnEnded()
	{
		_timer.Stop();
		ProcessTurn();
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
		
		
		_timer = new Timer()
		{
			Autostart = false,
			WaitTime = 0.5f,
			OneShot = false,
			Name = $"enemy timer",
		};
		_timer.Timeout += EnemyTurnEnded;
		AddChild(_timer);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}