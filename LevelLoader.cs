using System.Collections.Generic;
using System.Linq;
using Godot;
using Grimore.Entities;

namespace Grimore;

public class LevelLoader
{
    private static readonly PackedScene TileScene = ResourceLoader.Load<PackedScene>("res://tile.tscn");
    
    static readonly Dictionary<PackedScene, char[]> Things = new()
    {
        { TileScene, [' '] },
        { ResourceLoader.Load<PackedScene>("res://enemy.tscn"), ['E', 'e'] },
        { ResourceLoader.Load<PackedScene>("res://wall.tscn"), ['W', 'w'] },
        { ResourceLoader.Load<PackedScene>("res://door.tscn"), ['D', 'd'] },
        { ResourceLoader.Load<PackedScene>("res://npc.tscn"), ['N', 'n'] },
        { ResourceLoader.Load<PackedScene>("res://key.tscn"), ['K', 'k'] }
    };

    public static void Load(World world)
    {
        ClearThingsFrom(world);
        
        world.Turner.Enrol(world.GetChildren()
            .OfType<Player>()
            .First());
        
        var levelLines = """
            WWDWWWWWWWWWWWW
            W       E     W
            W       WW    W
            D             D
            W   WW E      W
            W             W
            W             W
            W      kDd    W
            WW           WW
            W       n     D
            """
            .Split('\n')
            .Select(l => l.Trim())
            .ToArray();

        var width = levelLines.Max(l => l.Length);
        var height = levelLines.Length;
        var offsetRows = 0 - width / 2;
        var offsetCols = 0 - height / 2;

        for (var row = 0; row < height; row++)
        for (var col = 0; col < width; col++)
        {
            var thing = Things
                .FirstOrDefault(t =>
                    t.Value.Any(c => c == levelLines[row].ElementAtOrDefault(col)))
                .Key;

            if (thing == null) continue;
            
            var instance = (Node3D)thing.Instantiate();
            var instanceType = instance.GetType();
            
            instance.Position = new Vector3(
                (col + offsetCols) * World.TileSize, 
                0, 
                (row + offsetRows) * World.TileSize);
            
            instance.Name = $"{instanceType.Name} {world.GetChildren().Count(x => x.GetType() == instanceType)}";
 
            world.AddChild(instance);

            if (instance is Wall or Tile) continue;
            
            instance.Position += new Vector3(0, World.HalfTileSize, 0);
            
            var tileForEntity = (Node3D)TileScene.Instantiate();
            tileForEntity.Position = new Vector3((col + offsetCols) * World.TileSize, 0, (row + offsetRows) * World.TileSize);
            world.AddChild(tileForEntity);
                
            if (instance is IActor actor) world.Turner.Enrol(actor);
        }

        world.Player.Position = Vector3.Zero;
        world.Turner.StartNextTurn();
    }

    private static void ClearThingsFrom(World world)
    {
        world.GetChildren().OfType<IActor>().Except([world.Player]).ForEach(x => ((Node)x).QueueFree());
        world.GetChildren().OfType<IInteractable>().ForEach(x => ((Node)x).QueueFree());
        world.Turner.Clear();
    }
}