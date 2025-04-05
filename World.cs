using System.Linq;
using Godot;

namespace grimore3ddotnet;

public partial class World : Node3D
{
	private readonly WorldManager _manager = new();
	private Player _player;
	private const float Speed = 2;

	public override void _Input(InputEvent ev)
	{
		if (ev.IsActionPressed(Actions.Act))
		{
			_player.CastSpell(this);
		}

		if (ev.IsActionPressed(Actions.Clear))
		{
			_manager.ClearThingsFrom(this);
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
		_manager.LoadLevel(this, Levels.One);
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
		var totalDirection = new Vector3(directionVectors.Sum(v => v.X) * _manager.TileSize, 0, directionVectors.Sum(v => v.Y) * _manager.TileSize);
		if (totalDirection != Vector3.Zero)
		{
			MovePlayer(totalDirection);
		}
	}
}