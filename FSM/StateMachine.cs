using System;
using System.Collections.Generic;
using System.Linq;
using Grimore.Entities;

namespace Grimore.FSM;

public abstract class StateMachine
{
    public abstract void Start(object entity);
}
public class StateMachine<T> : StateMachine
{
    private T Entity { get; init; }

    public StateMachine<T> State(State<T> state)
    {
        AllowedStates.Add(state);
        return this;
    }

    private List<State<T>> AllowedStates { get; } = new();

    public StateMachine<T> Default(Action<T> enter)
    {
        State(new State<T>().OnEnter(enter));
        return this;
    }
    public StateMachine<T> Default(Action<T> enter, Action<T> exit)
    {
        State(new State<T>().OnEnter(enter).OnExit(exit));
        return this;
    }

    public override void Start(object entity) => Start((T)entity);
    public void Start(T entity)
    {
        Current = AllowedStates.First();
        Current.Enter(entity);
    }
    
    public State<T> Current { get; set; }
}