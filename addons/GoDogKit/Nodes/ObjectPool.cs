using System.Collections.Generic;
using Godot;

namespace GoDogKit;

/// <summary>
/// A simple implementation of an object pool without security checks.
/// </summary>
[GlobalClass]
[Icon(GoDogKitManager.GoDogKitIconFolderPath + "ObjectPool.png")]
public partial class ObjectPool : Node
{
    /// <summary>
    /// The scene stored in the pool.
    /// </summary>
    [Export] public PackedScene Scene { get; set; }

    /// <summary>
    /// The parent where the nodes will be added to. 
    /// If null, the nodes will be added to the object pool node.
    /// </summary>
    [Export] public Node Parent { get; set; }

    /// <summary>
    /// The initial size of the pool.
    /// </summary>
    [Export]
    public int InitialSize
    {
        get => m_initialSize;
        set => m_initialSize = value < 0 ? 0 : value;
    }
    private int m_initialSize = 10;

    /// <summary>
    /// The number of nodes in the pool currently.       
    /// </summary>    
    public int Size
    {
        get => m_queue.Count;
    }

    /// <summary>
    /// Emitted when a node is gotten from the pool.    
    /// </summary>
    /// <param name="node"> The node that was gotten.</param>
    [Signal] public delegate void GotEventHandler(Node node);

    /// <summary>
    /// Emitted when a node is instantiated inside the pool.
    /// </summary>
    /// <param name="node"> The node that was instantiated.</param>
    [Signal] public delegate void InstantiatedEventHandler(Node node);

    /// <summary>
    /// Emitted when a node is released back to the pool.        
    /// </summary>
    /// <param name="node"> The node that was released.</param>
    [Signal] public delegate void ReleasedEventHandler(Node node);

    /// <summary>
    /// Emitted when a node is freed from the pool.
    /// </summary>
    /// <param name="node"> The node that was freed.</param>
    [Signal] public delegate void FreedEventHandler(Node node);

    /// <summary>
    /// Emitted when the pool is destroyed.
    /// </summary>
    [Signal] public delegate void DestroyedEventHandler();

    // The m_queue of nodes in the pool.
    protected Queue<Node> m_queue;

    // Enable equals to adding the node into scene tree.
    // While disable equals to remove it,
    // which will stop processing that node but
    // remains the memory of it.
    /// <summary>
    /// Call once node being got.
    /// </summary>    
    protected virtual void Enable(Node node)
    {
        if (Parent != null)
        {
            Parent.AddChild(node);
        }
        else
        {
            AddChild(node);
        }
    }
    /// <summary>
    /// Call once node being released.
    /// </summary>    
    protected virtual void Disable(Node node)
    {
        node.GetParent()?.RemoveChild(node);
    }

    public override void _Ready()
    {
        m_queue = [];

        // Auto initialize if initial size is greater than 0
        if (InitialSize > 0)
        {
            for (int i = 0; i < InitialSize; i++)
            {
                Instantiate();
            }
        }
    }

    /// <summary>
    /// Instantiates a new node into the pool.
    /// </summary>  
    /// <returns> The node that was instantiated. </returns>
    public virtual Node Instantiate()
    {
        Node node = Scene.Instantiate();

        m_queue.Enqueue(node);

        EmitSignal(SignalName.Instantiated, node);

        return node;
    }

    /// <summary>
    /// Get a node from the pool as Node, and enable it.
    /// If the pool is empty, a new node will be instantiated.
    /// </summary>         
    /// <returns> The node that was gotten. </returns>
    public virtual Node Get()
    {
        if (!m_queue.TryDequeue(out Node node))
        {
            // Instantiates one more for dequeue.
            Instantiate();
            node = m_queue.Dequeue();
        }

        EmitSignal(SignalName.Got, node);

        Enable(node);

        return node;
    }

    /// <summary>
    /// Get a node from the pool as specified Type, and enable it.
    /// If the pool is empty, a new node will be instantiated.
    /// </summary>
    /// <typeparam name="T"> Node type to get. </typeparam>
    /// <returns> The node that was gotten. </returns>        
    public virtual T Get<T>() where T : Node
    {
        return Get() as T;
    }

    /// <summary>
    /// Release a node back to the pool and disable it.
    /// Notice that there are no checks for whethet the node was creatd by the pool or not.
    /// </summary>
    /// <param name="node"> The Node to release. </param>
    public virtual void Release(Node node)
    {
        EmitSignal(SignalName.Released, node);

        m_queue.Enqueue(node);

        Disable(node);
    }

    /// <summary>
    /// Free a node from the pool.
    /// Notice that there are no checks for whether the node was creatd by the pool or not.
    /// </summary>
    /// <param name="node"> The Node to free. </param>
    public virtual void Free(Node node)
    {
        node.QueueFree();

        EmitSignal(SignalName.Freed, node);
    }

    /// <summary>
    /// Free all the nodes still inside the pool, will emit nodes' freed signal.
    /// </summary>
    public virtual void Clean()
    {
        foreach (Node node in m_queue)
        {
            Free(node);
        }

        m_queue.Clear();
    }

    /// <summary>
    /// Clean and then free the pool.
    /// </summary>
    public virtual void Destroy()
    {
        Clean();
        QueueFree();
        EmitSignal(SignalName.Destroyed);
    }
}