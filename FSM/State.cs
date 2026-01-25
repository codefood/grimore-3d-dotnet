using System;
using System.Collections.Generic;
using Godot;

namespace Grimore.FSM;

public class State<T>
{
    private Action<T> onEnterFunction;
    public State<T> OnEnter(Action<T> func)
    {
        onEnterFunction = func;
        return this;
    }

    public State<T> AddChild<TChild>(StateMachine<TChild> machine, Func<T, TChild> resolver) =>
        AddChild(machine, resolver, null);
    
    public State<T> AddChild<TChild>(StateMachine<TChild> machine, Func<T, TChild> resolver, Action<T, TChild> registrations)
    {
        Children.Add(new Reg()
        {
            Machine = machine as StateMachine<object>,
            Resolver = resolver as Func<object, object>,
            Registrations = registrations as Action<object, object>
        });
        return this;
    }

    public List<Reg> Children { get; set; } = new();

    public void Enter(T entity)
    {
        onEnterFunction.Invoke(entity);
        foreach (var childTuple in Children)
        {
            Node childEntity = childTuple.Resolver.Invoke(entity) as Node;
            childTuple.Registrations.Invoke(entity, childEntity);
            childTuple.Machine.Start(childEntity);
        }
    }

    public State<T> Attach<TOther>(Func<T> func, Func<TOther> func1)
    {
        throw new NotImplementedException();
    }
}

public class Reg
{
    public StateMachine<object> Machine { get; set; }
    public Func<object, object> Resolver { get; set; }
    public Action<object, object> Registrations { get; set; }
}