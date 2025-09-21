using System.Collections.Generic;
using System.Linq;
using Godot;
using Grimore.Entities;
using static Godot.ResourceLoader;

namespace Grimore;

public static class LevelLoader
{
    private static readonly PackedScene TileScene = Load<PackedScene>("res://tile.tscn");
    private static readonly PackedScene DoorScene = Load<PackedScene>("res://door.tscn");

    private static readonly Dictionary<PackedScene, char[]> Things = new()
    {
        { TileScene, [' '] },
        { DoorScene , ['D', 'd'] },
        { Load<PackedScene>("res://enemy.tscn"), ['E', 'e'] },
        { Load<PackedScene>("res://wall.tscn"), ['W', 'w'] },
        { Load<PackedScene>("res://npc.tscn"), ['N', 'n'] },
        { Load<PackedScene>("res://key.tscn"), ['K', 'k'] }
    };
    
    // ReSharper disable once StringLiteralTypo
    private static readonly Dictionary<int, string> Levels = new()
    {
        {
            0,
            """
            WWWWW1WWWWW
            W   k        
            W            
            W            
            """
        },
        {
            1,
            """
            WW2WW
            W   W
            w   W
            W   W
            W   W
            WW1WW
            """
        }
    };

    public static void Load(World world, int level)
    {
        ClearThingsFrom(world);
        
        world.Turner.Enrol(world.GetChildren()
            .OfType<Player>()
            .First());

        var levelLines = Levels[level]
            .Split('\n');
  
        var width = levelLines.Max(l => l.Length);
        var height = levelLines.Length;
        var offsetRows = 0 - width / 2;
        var offsetCols = 0 - height / 2;

        Vector3? startPosition = null;
        
        for (var row = 0; row < height; row++)
        for (var col = 0; col < width; col++)
        {
            var character = levelLines[row].ElementAtOrDefault(col);

            PackedScene thing;
            if (int.TryParse([character], out var doorIndex))
            {
                thing = DoorScene;
            }
            else
            {
                thing = Things
                    .FirstOrDefault(t =>
                        t.Value.Any(c => c == character))
                    .Key;
            }

            if (thing == null) continue;
            
            var instance = (Node3D)thing.Instantiate();
            var instanceType = instance.GetType();
            
            instance.Name = $"{instanceType.Name} {world.GetChildren().Count(x => x.GetType() == instanceType)}";
            
            instance.Position = new Vector3(
                (col + offsetCols) * World.TileSize, 
                0, 
                (row + offsetRows) * World.TileSize);
            
            world.AddChild(instance);

            if (instance is Wall wall && (col == width - 1 || row == height - 1)) wall.SemiTransparent = true; 

            if (instance is Wall or Tile) continue;

            instance.Position += new Vector3(0, World.HalfTileSize, 0);
            
            if (instance is Door d)
            {
                d.DoorIndex = doorIndex;
                if(col == 0 || col == width -1) d.RotateMe();
                
                d.LoadLevel += lvl => Load(world, lvl);
                if (doorIndex == level) startPosition = instance.Position;
            }
            
            
            var tileForEntity = (Node3D)TileScene.Instantiate();
            tileForEntity.Position = new Vector3((col + offsetCols) * World.TileSize, 0, (row + offsetRows) * World.TileSize);
            world.AddChild(tileForEntity);
                
            if (instance is IActor actor) world.Turner.Enrol(actor);
        }

        world.Player.Position =
            startPosition ??
            world.GetChildren().OfType<Tile>()
                .OrderBy(_ => GD.Randi())
                .First()
                .Position + new Vector3(0, 0, 0.1f);
    }

    private static void ClearThingsFrom(World world)
    {
        world.GetChildren().OfType<Tile>().ForEach(x => ((Node)x).QueueFree());
        world.GetChildren().OfType<IActor>().Except([world.Player]).ForEach(x => ((Node)x).QueueFree());
        world.GetChildren().OfType<IInteractable>().ForEach(x => ((Node)x).QueueFree());
        world.Turner.Clear();
    }
}