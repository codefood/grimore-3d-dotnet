using System.Collections.Generic;
using System.Linq;
using Godot;
using Grimore.Entities;
using static Godot.ResourceLoader;

namespace Grimore.Loaders;

public static class LevelLoader
{
    private static readonly PackedScene TileScene = Load<PackedScene>(Paths.Entities.Tile);
    private static readonly PackedScene DoorScene = Load<PackedScene>(Paths.Entities.Door);

    private static readonly Dictionary<PackedScene, char[]> Things = new()
    {
        { TileScene, [' '] },
        { DoorScene , ['D', 'd'] },
        { Load<PackedScene>(Paths.Entities.Enemy), ['E', 'e'] },
        { Load<PackedScene>(Paths.Entities.Wall), ['W', 'w'] },
        { Load<PackedScene>(Paths.Entities.Npc), ['N', 'n'] },
        { Load<PackedScene>(Paths.Entities.Key), ['K', 'k'] }
    };
    
    // ReSharper disable once StringLiteralTypo
    private static readonly Dictionary<int, string> Levels = new()
    {
        {
            0,
            """
            WWWWW1WWWWW
            W   k        
            W       e     
            W            
            """
        },
        {
            1,
            """
            WW2WW
            W k W
            w   W
            W   W
            W   W
            W N W
            W   W
            W k W
            WW0WW
            """
        },
        {
            2,
            """
            WWWWWWW
            Wk   kW
            3E   E4
            W  k  W
            WWWWW1W
            """
        },
        {
            3,
            """
            WWW
            W 2
            WWW
            """
        },
        {
            4,
            """
            WWWWWWWWWWWW
            2         EW
            WWWWWWWWWWWW
            """
        }
    };


    public static void Load(World world, int level, int? from = null)
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

            if (instance is Wall wall && (col > 0 && row > 0)) wall.SemiTransparent = true; 

            if (instance is Wall or Tile) continue;
            
            if (instance is IActor actor) world.Turner.Enrol(actor);

            instance.Position += new Vector3(0, World.HalfTileSize, 0);
            
            if (instance is Door d)
            {
                d.DoorIndex = doorIndex;
                if(col == 0 || col == width -1) d.RotateMe();

                d.LoadLevel += transitionTo =>
                {
                    if (level != transitionTo) Load(world, transitionTo, level);
                };
                
                if (doorIndex == from)
                {
                    if(world.Player.CurrentDirection != null)
                        startPosition = instance.Position + new Vector3(world.Player.CurrentDirection.Value.X * World.TileSize, 0, world.Player.CurrentDirection.Value.Y * World.TileSize);
                    else
                        startPosition = instance.Position;
                    
                    d.Open();
                }
            }
            
            var tileForEntity = (Node3D)TileScene.Instantiate();
            tileForEntity.Position = new Vector3((col + offsetCols) * World.TileSize, 0, (row + offsetRows) * World.TileSize);
            world.AddChild(tileForEntity);
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