using System.Collections.Generic;
using Godot;

namespace GoDogKit.Optimizer;

/// <summary>
/// A pool of objects that can be reused. ObjectPool do not handle about destruction of objects.
/// </summary>
public class ObjectPool<T> where T : new()
{
    protected readonly Queue<T> m_objects;
    public int Count => m_objects.Count;

    public ObjectPool() => m_objects = [];

    public ObjectPool(int initialSize)
    {
        m_objects = new(initialSize);

        for (int i = initialSize - 1; i >= 0; i--)
        {
            m_objects.Enqueue(Instantiate());
        }
    }

    public virtual T Instantiate() => new();

    public virtual T Get() => m_objects.Count > 0 ? m_objects.Dequeue() : Instantiate();

    public virtual void Release(T obj) => m_objects.Enqueue(obj);
}

/// <summary>
/// Variant of ObjectPool for nodes. Automatically removes nodes from the scene when released.
/// </summary>
public class NodePool<T> : ObjectPool<T> where T : Node, new()
{
    public override void Release(T obj)
    {
        if (obj.IsInsideTree()) obj.GetParent().RemoveChild(obj);

        base.Release(obj);
    }
}

/// <summary>
/// Variant of ObjectPool for scenes. Automatically instatiates packed scenes.
/// </summary>
public class ScenePool<T>(PackedScene scene) : NodePool<T> where T : Node, new()
{
    public override T Instantiate() => scene.Instantiate() as T;
}