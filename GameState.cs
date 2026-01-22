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
    private static State _previous;

    public class TurnState : State
    {
        public IActor Actor { get; set; }
        public Command Command { get; set; }
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
        _previous = Current;
        Current = States.Paused.Enter();
    }

    public static void Start() => Current = _previous ?? States.Playing.Enter();
    public static void GameOver() => Current = States.Ended.Enter();
}