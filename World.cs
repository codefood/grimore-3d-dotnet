using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace grimore3ddotnet;

public partial class World : Node3D
{
	private readonly WorldManager _manager = new();
	private const float Speed = 1;

	public override void _Input(InputEvent ev)
	{
		if (ev.IsActionPressed(Actions.Act))
		{
			//todo: an action
		}

		if (ev.IsActionPressed(Actions.Clear))
		{
			_manager.ClearThingsFrom(this);
		}
	}

	private void MovePlayer(Vector3 totalDirection)
	{
		var player = GetChildren()
			.OfType<Player>()
			.First();

		GD.Print(totalDirection.ToString());
		player.SetVelocity(totalDirection * Speed);
		if (!player.IsOnFloor())
		{
			GD.Print("Falling");
		}
		player.MoveAndSlide();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() => 
		_manager.LoadLevel(this, Levels.One);

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

public static class Fn
{
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> input, Action<T> action)
	{
		var enumerable = input as T[] ?? input.ToArray();
		foreach (var element in enumerable)
		{
			action(element);
		}

		return enumerable;
	}
}