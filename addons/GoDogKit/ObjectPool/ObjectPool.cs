using System.Collections.Generic;
using Godot;

namespace GoDogKit
{
    /// <summary>
    /// A simple implementation of an object pool without security checks.    
    /// </summary>    
    public partial class ObjectPool : Node
    {
        /// <summary>
        /// The scene stored in the pool.        
        /// </summary>
        [Export] public PackedScene Scene { get; set; }

        /// <summary>
        /// The parent where the nodes will be added to. 
        /// If null, the nodes will be added to the scene tree root.
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
        /// Emitted when a node is gotten from the pool.    
        /// </summary>
        /// <param name="node"> The node that was gotten.</param>
        [Signal] public delegate void GottenEventHandler(Node node);

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

        // The queue of nodes in the pool.
        private Queue<Node> queue;

        /// <summary>
        /// The number of nodes in the pool currently.       
        /// </summary>
        public virtual int Size
        {
            get => queue.Count;
        }

        public override void _Ready()
        {
            queue = new Queue<Node>();

            // Auto initialize if initial size is greater than 0
            if (InitialSize > 0)
            {
                for (int i = 0; i < InitialSize; i++)
                {
                    queue.Enqueue(Instantiate());
                }
            }
        }

        /// <summary>
        /// Instantiate a new node into the pool.
        /// </summary>  
        /// <returns> The node that was instantiated. </returns>
        public virtual Node Instantiate()
        {
            Node node = Scene.Instantiate();

            EmitSignal(SignalName.Instantiated, node);

            return node;
        }

        /// <summary>
        /// Get a node from the pool as Node, and add it to the scene tree root.  
        /// If the pool is empty, a new node will be instantiated and added to the scene tree.
        /// </summary>         
        /// <returns> The node that was gotten. </returns>
        public virtual Node Get()
        {
            if (!queue.TryDequeue(out Node node))
            {
                node = Instantiate();
            }

            EmitSignal(SignalName.Gotten, node);

            if (Parent != null)
            {
                Parent.AddChild(node);
            }
            else
            {
                GetTree().Root.AddChild(node);
            }

            return node;
        }


        /// <summary>
        /// Get a node from the pool as specified Type, and add it to the parent or the scene tree root.  
        ///If the pool is empty, a new node will be instantiated and added to the scene tree root.
        /// </summary>
        /// <typeparam name="T"> Node type to get. </typeparam>
        /// <returns> The node that was gotten. </returns>        
        public virtual T Get<T>() where T : Node
        {
            return Get() as T;
        }

        /// <summary>
        /// Release a node back to the pool and remove it from the parent or the scene tree root.
        /// Notice that there are no checks for whethet the node was creatd by the pool or not.
        /// </summary>
        /// <param name="node"> The Node to release. </param>
        public virtual void Release(Node node)
        {
            EmitSignal(SignalName.Released, node);

            node.GetParent().RemoveChild(node);

            queue.Enqueue(node);
        }

        /// <summary>
        /// Free a node from the pool and remove it from the parent or the scene tree at the end of the frame.
        /// Notice that there are no checks for whether the node was creatd by the pool or not.
        /// </summary>
        /// <param name="node"> The Node to free. </param>
        public virtual void Free(Node node)
        {
            node.QueueFree();

            EmitSignal(SignalName.Freed, node);
        }

        /// <summary>
        /// Clean the pool and all the nodes still inside it.
        /// </summary>
        public virtual void Clean()
        {
            foreach (Node node in queue)
            {
                node.QueueFree();
            }

            queue.Clear();
        }

        /// <summary>
        /// Clean the pool and all the nodes still inside it, and free the pool.
        /// </summary>
        public virtual void Destroy()
        {
            Clean();
            QueueFree();
        }

    }
}