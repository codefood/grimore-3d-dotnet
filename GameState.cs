using System;
using Grimore.Entities;
using static Grimore.GameState;

namespace Grimore;

public static class States
{
    public static readonly TurnState Playing = new();
    public static readonly State Paused = new();
    public static readonly State Menu = new();
    public static readonly EndState Ended = new();
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

    public class EndState : State
    {
        internal State Won()
        {
            HasWon = true;
            return Enter();
        }

        public bool HasWon { get; set; }
    }
		
    public static State Current = States.Playing;

    public static void Pause()
    {
        _previous = Current;
        Current = States.Paused.Enter();
    }

    public static void Toggle()
    {
        if (Current == States.Paused) Start();
        else Pause();
    }

    public static void Start() => 
        Current = _previous ?? States.Playing.Enter();
    
    public static void GameOver(bool won) => 
        Current = won 
            ? States.Ended.Won() 
            : States.Ended.Enter();
}