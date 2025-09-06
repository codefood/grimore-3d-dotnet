namespace Grimore;

public static class GameState
{
    public class State
    {
        private State() {}
        public static readonly State WaitingForInput = new();
        public static readonly State Paused = new();
    }
		
    public static State Current = State.WaitingForInput;

    public static void Pause() => Current = State.Paused;
    public static void Resume() => Current = State.WaitingForInput;
}