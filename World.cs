using System.Linq;
using Godot;

public partial class World : Node3D
{
	[Export]
	private PackedScene _tileScene = GD.Load<PackedScene>("res://tile.tscn");

	private int _width;
	private int _height;
	private int _offsetRows;
	private int _offsetCols;
	private const int TileSize = 2;

	private void Load(string level)
	{
		var levelLines = level.Split('\n');
		
		_width = levelLines.Max(l => l.Length);
		_height = levelLines.Length;
		_offsetRows = 0 - _width / 2;
		_offsetCols = 0 - _height / 2;
		
		for (var row = 0; row < _height; row++)
		for (var col = 0; col < _width; col++)
		{
			var element = levelLines[row].ElementAtOrDefault(col);
			if (char.IsLetter(element))
				AddTile(row, col);
		}
	}

	private void Clear()
	{
		foreach (var tile in GetChildren().OfType<Tile>()) tile.QueueFree();
	}

	private void AddTile(int row, int col)
	{
		var tile = _tileScene.Instantiate() as Tile;
		tile!.Position = new Vector3((col + _offsetCols) * TileSize, 0, (row + _offsetRows) * TileSize);
		GD.Print($"instantiating tile at {tile.Position}");
		AddChild(tile);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Clear();
		Load(Levels.One);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}