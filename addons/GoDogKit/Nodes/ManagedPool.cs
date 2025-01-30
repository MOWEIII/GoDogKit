using System.Collections.Generic;
using Godot;

namespace GoDogKit;

/// <summary>
/// A safe version of ObjectPool, provides security checks while release or free.
/// The node managed by a ManagedPool can uses relative extension methods to simplify works.
/// </summary>
[GlobalClass]
[Icon(GoDogKitManager.GoDogKitIconFolderPath + "ManagedPool.png")]
public partial class ManagedPool : ObjectPool
{
    // How the ManagedPool work is just record the relation between node which be managed
    // and the pool itself.
    public static Dictionary<Node, ManagedPool> ManagedMap { get; private set; } = [];
    public static int ManagedNodeCount { get => ManagedMap.Count; }
    public HashSet<Node> m_register;

    /// <summary>
    /// The total size of all managed objects.
    /// </summary>    
    public int ManagedSize
    {
        get => m_register.Count;
    }

    public override void _Ready()
    {
        m_register = [];

        base._Ready();
    }

    public override Node Instantiate()
    {
        Node node = base.Instantiate();

        ManagedMap.Add(node, this);

        m_register.Add(node);

        return node;
    }

    /// <summary>
    /// Release a node managed by the pool.
    /// </summary>
    /// <param name="node"> The Node to release. </param>
    public override void Release(Node node)
    {
        if (!m_register.Contains(node)) return;

        base.Release(node);
    }

    /// <summary>
    /// Free a node managed by the pool.
    /// </summary>
    /// <param name="node"> The Node to free. </param>
    public override void Free(Node node)
    {
        if (!m_register.Contains(node)) return;

        m_register.Remove(node);

        ManagedMap.Remove(node);

        base.Free(node);
    }

    /// <summary>
    /// Free the pool, and all of it's managed objects.
    /// </summary>
    public override void Clean()
    {
        foreach (Node node in m_register)
        {
            ManagedMap.Remove(node);

            node.QueueFree();
        }

        m_queue.Clear();

        m_register.Clear();
    }
}