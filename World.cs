using System.Linq;
using Godot;

namespace grimore3ddotnet;

public partial class World : Node3D
{
	private readonly WorldManager _worldManager = new();
	private readonly TurnManager _turnManager = new();
	
	private Player _player;
	private const float Speed = 2;
	

	public override void _Input(InputEvent ev)
	{
		if (!_turnManager.IsCurrentTurn(_player)) return;
		var directionsPressed = Actions.Directions
			.Where(k => ev.IsActionPressed(k.Key))
			.ToList();
		
		if (directionsPressed.Count != 0)
		{
			var direction = directionsPressed.First().Value;
			_player.MoveAndCollide(new Vector3(direction.X, 0, direction.Y));
		}
		if (ev.IsActionPressed(Actions.Act))
		{
			_player.CastSpell(this);
		}

		if (ev.IsActionPressed(Actions.Clear))
		{
			_worldManager.ClearThingsFrom(this);
		}
	}

	private void MovePlayer(Vector3 totalDirection)
	{
		var player = _player;

		GD.Print(totalDirection.ToString());
		player.SetVelocity(totalDirection * Speed);
		if (!player.IsOnFloor())
		{
			GD.Print("Falling");
		}
		player.MoveAndSlide();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_worldManager.LoadLevel(this, Levels.One);
		_player = GetChildren()
			.OfType<Player>()
			.First();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var directionVectors = Actions.Directions.Select(action =>
				Input.IsActionPressed(action.Key)
					? action.Value
					: Vector2.Zero)
			.ToList();
		var totalDirection = new Vector3(directionVectors.Sum(v => v.X) * _worldManager.TileSize, 0, directionVectors.Sum(v => v.Y) * _worldManager.TileSize);
		if (totalDirection != Vector3.Zero)
		{
			MovePlayer(totalDirection);
		}
	}
}