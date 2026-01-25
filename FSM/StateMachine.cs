using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Grimore.Entities;
using Grimore.Loaders;

namespace Grimore.FSM;

public class StateMachine<T>()
{
    private T Entity { get; init; }

    public StateMachine<T> State(State<T> state)
    {
        AllowedStates.Add(state);
        return this;
    }

    private List<State<T>> AllowedStates { get; } = new();

    public StateMachine<T> DefaultState(Action<T> func)
    {
        State(new State<T>().OnEnter(func));
        return this;
    }

    public void Start(T entity)
    {
        Current = AllowedStates.First();
        Current.Enter(entity);
    }

    public State<T> Current { get; set; }
}