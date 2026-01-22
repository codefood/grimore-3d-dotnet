namespace Grimore;

public class TurnLoader
{
    public static void Load(World world)
    {
        TurnManager.OnPlayerMove += world.Quest.Moved;
        TurnManager.OnInteractionSuccess += world.Quest.InteractionSuccess;
        TurnManager.OnTurnStart += world.Interface.UpdateCurrentTurn;
    }
}