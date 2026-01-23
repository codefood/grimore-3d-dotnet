namespace Grimore.Loaders;

public static class TurnLoader
{
    public static void Load(World world)
    {
        TurnManager.OnTurnStart += world.Quest.Check;
        TurnManager.OnPlayerMove += world.Quest.Check;
        TurnManager.OnInteractionSuccess += world.Quest.Check;
        TurnManager.OnTurnStart += world.Interface.UpdateCurrentTurn;
    }
}