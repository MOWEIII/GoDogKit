using System;
using System.Collections.Generic;

namespace GoDogKit.AI;

/// <summary>
/// This is the interface for all state-like objects in the whole AI system.
/// </summary>
public interface IState
{
    public void Ready();
    public void Enter();
    public void Process(double delta);
    public void Exit();
}

/// <summary>
/// Used to store different types of values with string keys.
/// </summary>
public class BlackBoard
{
    private readonly Dictionary<string, object> m_Values = [];
    public BlackBoard((string, object)[] values)
    {
        foreach (var item in values)
        {
            m_Values.Add(item.Item1, item.Item2);
        }
    }

    public T Get<T>(string key)
    {
        if (m_Values.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        else throw new Exception($"No key '{key}' found in blackboard!");
    }

    public void Add<T>(string key, T value) => m_Values.Add(key, value);
}