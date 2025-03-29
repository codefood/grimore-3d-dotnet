using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace grimore3ddotnet;

public partial class World : Node3D
{
	[Export]
	private PackedScene _tileScene = ResourceLoader.Load<PackedScene>("res://tile.tscn");

	[Export] 
	private PackedScene _wallScene = ResourceLoader.Load<PackedScene>("res://wall.tscn");
	private const int TileSize = 2;
	private int _offsetRows;
	private int _offsetCols;


	public Dictionary<PackedScene, char[]> GetThings() => new()
	{
		{ _tileScene, ['X', 'x', '.'] },
		{ _wallScene, ['W', 'w'] }
	};
	
	private void Load(string level)
	{
		var levelLines = level.Split('\n');
		var things = GetThings();
		
		var width = levelLines.Max(l => l.Length);
		var height = levelLines.Length;
		_offsetRows = 0 - width / 2;
		_offsetCols = 0 - height / 2;
		
		for (var row = 0; row < height; row++)
		for (var col = 0; col < width; col++)
		{
			var thing = things
				.FirstOrDefault(t =>
					t.Value.Any(c => c == levelLines[row].ElementAtOrDefault(col)))
				.Key;
			
			if (thing != null)
			{
				AddThing(row, col, thing);
			}
		}
			
	}
	
	private void AddThing(int row, int col, PackedScene scene)
	{
		var thing = scene.Instantiate() as Node3D;
		thing!.Position = new Vector3((col + _offsetCols) * TileSize, 0, (row + _offsetRows) * TileSize);
		AddChild(thing);
	}

	private void ClearThings()
	{
		GetChildren().OfType<Tile>().ForEach(x => x.QueueFree());
		GetChildren().OfType<Wall>().ForEach(x => x.QueueFree());
	}
	
	private const float Speed = 1;

	public override void _Input(InputEvent ev)
	{
		if (ev.IsActionPressed(Actions.Act))
		{
			//todo: an action
		}

		if (ev.IsActionPressed(Actions.Clear))
		{
			ClearThings();
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
	public override void _Ready()
	{
		ClearThings();
		Load(Levels.Three);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var directionVectors = Actions.Directions.Select(action =>
				Input.IsActionPressed(action.Key)
					? action.Value
					: Vector2.Zero)
			.ToList();
		var totalDirection = new Vector3(directionVectors.Sum(v => v.X) * TileSize, 0, directionVectors.Sum(v => v.Y) * TileSize);
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