using Grimore.Entities;
using Grimore.Entities.Quests;
using static Godot.Vector2;
using static Grimore.Entities.Quests.Quest;

namespace Grimore.Loaders;

public abstract class QuestLoader 
{
    public static void Load(World world) =>
        world.Quest = new Quest(
            new MoveSuccess(Up, "North"),
            new MoveSuccess(Down, "South"),
            new MoveSuccess(Left, "East"),
            new MoveSuccess(Right, "West"),
            new InteractionCountRequired<Key>("Collect", "Key", 2),
            new InteractionRequired("Open", "Door 3")
        );
}