using static Godot.Vector2;
using static Grimore.Quest;

namespace Grimore;

public class QuestLoader 
{
    public static void Load(World world) =>
        world.Quest = new Quest(
            new MoveSuccess(Up, "North"),
            new MoveSuccess(Down, "South"),
            new MoveSuccess(Left, "East"),
            new MoveSuccess(Right, "West"),
            new InteractionRequired("Collect", "Key 1"),
            new InteractionRequired("Open", "Door 3")
        );
}