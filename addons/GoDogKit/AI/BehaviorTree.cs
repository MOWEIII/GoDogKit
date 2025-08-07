using System;
using System.Collections.Generic;

namespace GoDogKit.AI;

/// <summary>
/// The state of a behavior tree node.
/// </summary>
public enum BTState
{
    Ready,
    Invalid,
    Success,
    Failure,
    Running,
}

public abstract class BTNode : IState
{
    public BTNode Parent { get; protected set; } = null;
    public BTState State { get; protected set; } = BTState.Ready;

    public virtual void Ready() => State = BTState.Ready;
    public virtual void Enter() => State = BTState.Running;
    public abstract void Process(double delta);
    public virtual void Exit() { }
    public void SetParent(BTNode parent) => Parent = parent;
    public void SetState(BTState state) => State = state;
    public void Ticks(double delta)
    {
        if (State is BTState.Invalid) return;

        Enter();
        Process(delta);
        Exit();
    }
}

public abstract class BTControlNode : BTNode
{
    public List<BTNode> Children { get; protected set; } = [];

    public override void Ready()
    {
        base.Ready();

        foreach (BTNode child in Children)
        {
            child.Ready();
        }
    }
    public void AddChild(BTNode child)
    {
        child.SetParent(this);

        Children.Add(child);
    }
    public void RemoveChild(BTNode child)
    {
        if (Children.Remove(child))
        {
            child.SetParent(null);
        }
    }
    public List<BTNode> GetChildren(bool recursive = false)
    {
        List<BTNode> nodes = [];

        foreach (BTNode node in Children)
        {
            nodes.Add(node);

            if (recursive && node is BTControlNode controlNode)
            {
                nodes.AddRange(controlNode.GetChildren(recursive));
            }
        }

        return nodes;
    }
}

/// <summary>
/// A behavior tree node that executes its children in sequence.
/// If any child fails, the sequencer fails.
/// </summary>
public class BTSequencer : BTControlNode
{
    public override void Process(double delta)
    {
        foreach (BTNode node in Children)
        {
            node.Ticks(delta);

            if (node.State is BTState.Running) return;

            else if (node.State is BTState.Success) continue;

            else if (node.State is BTState.Failure)
            {
                State = BTState.Failure;
                return;
            }
        }

        State = BTState.Success;
    }
}

/// <summary>
/// A behavior tree node that executes its children in sequence.
/// If any child succeeds, the selector succeeds.
/// </summary>
public class BTSelector : BTControlNode
{
    public override void Process(double delta)
    {
        foreach (BTNode node in Children)
        {
            node.Ticks(delta);

            if (node.State is BTState.Running) return;

            else if (node.State is BTState.Failure) continue;

            if (node.State is BTState.Success)
            {
                State = BTState.Success;
                return;
            }
        }

        State = BTState.Failure;
    }
}

/// <summary>
/// A behavior tree node that executes its children in parallel.
/// If the number of successful children is greater than the requirement, the parallel succeeds.
/// </summary>
/// <param name="requirement"> The minimum number of successful children required for the parallel to succeed. </param>
public class BTParallel(int requirement) : BTControlNode
{
    public override void Process(double delta)
    {
        int successCount = 0;

        foreach (BTNode node in Children)
        {
            node.Ticks(delta);

            if (node.State is BTState.Success) successCount++;
        }

        if (successCount is 0) State = BTState.Failure;
        else if (successCount >= requirement) State = BTState.Success;
        else State = BTState.Running;
    }
}

public abstract class BTExecutionNode : BTNode { }

public abstract class BTTask : BTExecutionNode { }

public abstract class BTTask<T>(T owner) : BTTask
{
    public T Owner { get; protected set; } = owner;
}

/// <summary>
/// Designed for simplifying the creation of simple tasks.
/// Use lambda expressions and the closure feature of C# to build blackboard context.
/// </summary>
public class BTAction(Action<BTAction, double> action) : BTTask
{
    public override void Process(double delta) => action(this, delta);
}

/// <summary>
/// Designed for simplifying the creation of simple boolean conditions.
/// </summary>
public class BTCondition(Func<bool> func) : BTExecutionNode
{
    public override void Process(double delta)
    {
        if (func()) State = BTState.Success;
        else State = BTState.Failure;
    }
}