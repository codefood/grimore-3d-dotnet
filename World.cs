using System.Linq;
using Godot;

namespace grimore3ddotnet;

public partial class World : Node3D
{
	[Export]
	private PackedScene _tileScene = GD.Load<PackedScene>("res://tile.tscn");
	
	private int _offsetRows;
	private int _offsetCols;
	private const int TileSize = 2;

	private void Load(string level)
	{
		var levelLines = level.Split('\n');
		
		var width = levelLines.Max(l => l.Length);
		var height = levelLines.Length;
		_offsetRows = 0 - width / 2;
		_offsetCols = 0 - height / 2;
		
		for (var row = 0; row < height; row++)
		for (var col = 0; col < width; col++)
		{
			if (char.IsLetter(levelLines[row].ElementAtOrDefault(col)))
				AddTile(row, col);
		}
	}
	
	private void AddTile(int row, int col)
	{
		var tile = _tileScene.Instantiate() as Tile;
		tile!.Position = new Vector3((col + _offsetCols) * TileSize, 0, (row + _offsetRows) * TileSize);
		GD.Print($"instantiating tile at {tile.Position}");
		AddChild(tile);
	}

	private void ClearTiles()
	{
		foreach (var tile in GetChildren().OfType<Tile>()) 
			tile.QueueFree();
	}
	
	public override void _Input(InputEvent ev)
	{
		var directionVectors = Actions.Directions.Select(action =>
			ev.IsActionPressed(action.Key)
				? action.Value
				: Vector2.Zero)
			.ToList();
		var totalDirection = new Vector3(directionVectors.Sum(v => v.X) * TileSize, 0, directionVectors.Sum(v => v.Y) * TileSize);
		if (totalDirection != Vector3.Zero)
		{
			MovePlayer(totalDirection);
		}

		if (ev.IsActionPressed(Actions.Act))
		{
			MovePlayer(Vector3.Up);
		}

		if (ev.IsActionPressed(Actions.Clear))
		{
			ClearTiles();
		}
	}

	private void MovePlayer(Vector3 totalDirection)
	{
		var player = GetChildren()
			.OfType<Player>()
			.First();

		GD.Print(totalDirection.ToString());
		player.SetPosition(player.Position + totalDirection);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ClearTiles();
		Load(Levels.Three);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}