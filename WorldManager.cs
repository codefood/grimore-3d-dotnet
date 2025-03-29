using System.Collections.Generic;
using System.Linq;
using Godot;

namespace grimore3ddotnet;

public class WorldManager
{
    public int TileSize => 2;
    private int _offsetRows;
    private int _offsetCols;
    
    static readonly Dictionary<PackedScene, char[]> Things = new()
    {
        { ResourceLoader.Load<PackedScene>("res://tile.tscn"), [' '] },
        { ResourceLoader.Load<PackedScene>("res://wall.tscn"), ['W', 'w'] }
    };

    public void LoadLevel(World world, string level)
    {
        ClearThingsFrom(world);
        var levelLines = level.Split('\n');

        var width = levelLines.Max(l => l.Length);
        var height = levelLines.Length;
        _offsetRows = 0 - width / 2;
        _offsetCols = 0 - height / 2;

        for (var row = 0; row < height; row++)
        for (var col = 0; col < width; col++)
        {
            var thing = Things
                .FirstOrDefault(t =>
                    t.Value.Any(c => c == levelLines[row].ElementAtOrDefault(col)))
                .Key;

            if (thing == null) continue;
            
            var instance = thing.Instantiate() as Node3D;
            instance!.Position = new Vector3((col + _offsetCols) * TileSize, 0, (row + _offsetRows) * TileSize);
            world.AddChild(instance);
        }
    }

    public void ClearThingsFrom(World world)
    {
        world.GetChildren().OfType<Tile>().ForEach(x => x.QueueFree());
        world.GetChildren().OfType<Wall>().ForEach(x => x.QueueFree());
    }
}