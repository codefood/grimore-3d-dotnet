using System;
using Grimore.Entities;

namespace Grimore;

public static class States
{
    public static readonly GameState.TurnState Playing = new();
    public static readonly GameState.State Paused = new();
    public static readonly GameState.State Ended = new();
}
public static class GameState
{
    private static State previous;

    public class TurnState : State
    {
        public IActor Actor { get; set; }
        public Move Command { get; set; }
    }
    public class State
    {
        internal State Enter()
        {
            OnEnter!.Invoke();
            return this;
        }

        public event Action OnEnter;
    }
		
    public static State Current = States.Playing;

    public static void Pause()
    {
        previous = Current;
        Current = States.Paused.Enter();
    }

    //TODO? do we need to previous?.Enter() that would re-start turn after pause
    public static void Start() => Current = previous ?? States.Playing.Enter();
    public static void GameOver() => Current = States.Ended.Enter();
}