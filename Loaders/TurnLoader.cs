namespace Grimore.Loaders;

public static class TurnLoader
{
    public static void Load(World world)
    {
        TurnManager.OnPlayerMove += world.Quest.Moved;
        TurnManager.OnInteractionSuccess += world.Quest.InteractionSuccess;
        TurnManager.OnTurnStart += world.Interface.UpdateCurrentTurn;
    }
}