using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grimore;

public class LevelLoader
{
    private int _offsetRows;
    private int _offsetCols;
    static readonly PackedScene EnemyScene = ResourceLoader.Load<PackedScene>("res://enemy.tscn");
    private static readonly PackedScene TileScene = ResourceLoader.Load<PackedScene>("res://tile.tscn");
    
    static readonly Dictionary<PackedScene, char[]> Things = new()
    {
        { TileScene, [' '] },
        { ResourceLoader.Load<PackedScene>("res://wall.tscn"), ['W', 'w'] },
        { ResourceLoader.Load<PackedScene>("res://door.tscn"), ['D', 'd'] }
    };
    
    

    public void Load(World world, string level)
    {
        ClearThingsFrom(world);
        
        world.TurnManager.Enrol(world.GetChildren()
            .OfType<Player>()
            .First());
        
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
            instance!.Position = new Vector3((col + _offsetCols) * World.TileSize, 0, (row + _offsetRows) * World.TileSize);

            if (instance is Door)
            {
                //special case to put doors on tiles
                var tileForDoor = TileScene.Instantiate() as Node3D;
                tileForDoor!.Position = new Vector3((col + _offsetCols) * World.TileSize, 0, (row + _offsetRows) * World.TileSize);
                world.AddChild(tileForDoor);
            }
            
            world.AddChild(instance);

            if (instance is Tile && GD.Randi() % 40 == 0)
            {
                var enemy = EnemyScene.Instantiate() as Enemy;
                enemy!.Position = new Vector3((col + _offsetCols) * World.TileSize, 0.5f, (row + _offsetRows) * World.TileSize);
                enemy.Name = $"Enemy {world.GetChildren().OfType<Enemy>().Count() + 1}";
                world.AddChild(enemy);
                world.TurnManager.Enrol(enemy);
            }
            
        }

        world.TurnManager.StartNextTurn();
    }

    private void ClearThingsFrom(World world)
    {
        world.GetChildren().OfType<Tile>().ForEach(x => x.QueueFree());
        world.GetChildren().OfType<Wall>().ForEach(x => x.QueueFree());
        world.GetChildren().OfType<Door>().ForEach(x => x.QueueFree());
        world.TurnManager.Clear();
    }
}