using System;
using System.Collections.Generic;
using Godot;
using Grimore.Entities;

namespace Grimore.FSM;

public class State<T>
{
    private Action<T> onEnterFunction;
    private Action<T> onExitFunction;

    public State<T> OnEnter(Action<T> func)
    {
        onEnterFunction = func;
        return this;
    }

    public State<T> AddChild<TChild>(StateMachine<TChild> machine, TChild node) where TChild : Node
    {
        Children.Add(new Reg<Node>
        {
            Machine = machine,
            Entity = node
        });
        return this;
    }

    public List<Reg<Node>> Children { get; set; } = new();

    public void Enter(T entity)
    {
        Entity = entity;
        onEnterFunction.Invoke(entity);
        foreach (var childTuple in Children)
        {
            childTuple.Machine.Start(childTuple.Entity);
        }
    }

    public T Entity { get; set; }

    public State<T> OnExit(Action<T> func)
    {
        onExitFunction = func;
        return this;
    }
}

public class Reg<T>
{
    public StateMachine Machine { get; set; }
    public T Entity { get; set; }
}