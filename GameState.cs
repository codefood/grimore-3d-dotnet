using System;

namespace Grimore;

public static class GameState
{
    public class State
    {
        internal State Enter()
        {
            OnEnter!.Invoke();
            return this;
        }

        public event Action OnEnter;
        
        private State() {}
        public static readonly State Started = new();
        public static readonly State Paused = new();
        public static readonly State Ended = new();
    }
		
    public static State Current = State.Started;

    public static void Pause() => Current = State.Paused.Enter();
    public static void Start() => Current = State.Started.Enter();
    public static void GameOver() => Current = State.Ended.Enter();
}