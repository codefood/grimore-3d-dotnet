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
        { ResourceLoader.Load<PackedScene>("res://door.tscn"), ['D', 'd'] },
        { EnemyScene, ['E', 'e'] },
        { ResourceLoader.Load<PackedScene>("res://npc.tscn"), ['N', 'n'] }
    };

    public void Load(World world, string level)
    {
        ClearThingsFrom(world);
        
        world.Turner.Enrol(world.GetChildren()
            .OfType<Entities.Player>()
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
            
            var instance = (Node3D)thing.Instantiate();
            instance.Position = new Vector3((col + _offsetCols) * World.TileSize, 0, (row + _offsetRows) * World.TileSize);
            
            var instanceType = instance.GetType();
            instance.Name = $"{instanceType.Name} {world.GetChildren().Count(x => x.GetType() == instanceType)}";
            
            if (instance is Entities.Door or Entities.Enemy or Entities.Npc)
            {
                var tileForEntity = (Node3D)TileScene.Instantiate();
                tileForEntity.Position = new Vector3((col + _offsetCols) * World.TileSize, 0, (row + _offsetRows) * World.TileSize);
                world.AddChild(tileForEntity);
                instance.Position += new Vector3(0, World.TileSize / 2, 0); 
                
                if(instance is Entities.Enemy en)
                    world.Turner.Enrol(en);
                
            }
            
            world.AddChild(instance);

        }

        world.Turner.StartNextTurn();
    }

    private void ClearThingsFrom(World world)
    {
        world.GetChildren().OfType<Entities.Tile>().ForEach(x => x.QueueFree());
        world.GetChildren().OfType<Entities.Wall>().ForEach(x => x.QueueFree());
        world.GetChildren().OfType<Entities.Door>().ForEach(x => x.QueueFree());
        world.GetChildren().OfType<Entities.Enemy>().ForEach(x => x.QueueFree());
        world.Turner.Clear();
    }
}