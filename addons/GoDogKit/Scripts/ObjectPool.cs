using System.Collections.Generic;
using Godot;

namespace GoDogKit
{
    public partial class ObjectPool : Node
    {
        // The scene stored in the pool.        
        [Export] public PackedScene Scene { get; set; }

        // The initial size of the pool.
        [Export]
        public int InitialSize
        {
            get => m_initialSize;
            set => m_initialSize = value < 0 ? 0 : value;
        }
        private int m_initialSize = 10;

        // emitted when a node is gotten from the pool.    
        [Signal] public delegate void GottenEventHandler(Node node);

        // emitted when a node is instantiated inside the pool.        
        [Signal] public delegate void InstantiatedEventHandler(Node node);

        // emitted when a node is released back to the pool.        
        [Signal] public delegate void ReleasedEventHandler(Node node);

        // emitted when a node is freed from the pool.        
        [Signal] public delegate void FreedEventHandler(Node node);

        // The queue of nodes in the pool.
        private Queue<Node> queue;

        // The number of nodes in the pool currently.        
        public int Size
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
                    Node node = Scene.Instantiate();
                    queue.Enqueue(node);
                }
            }
        }

        // Get a node from the pool as Node, and add it to the scene tree.  
        // If the pool is empty, a new node will be instantiated and added to the scene tree.
        public virtual Node Get()
        {
            if (!queue.TryDequeue(out Node node))
            {
                node = Scene.Instantiate();
                EmitSignal(SignalName.Instantiated, node);
            }
            AddChild(node);
            EmitSignal(SignalName.Gotten, node);
            return node;
        }

        // Get a node from the pool as specified Type, and add it to the scene tree.  
        // If the pool is empty, a new node will be instantiated and added to the scene tree.
        public virtual T Get<T>() where T : Node
        {
            if (!queue.TryDequeue(out Node node))
            {
                node = Scene.Instantiate() as T;
                EmitSignal(SignalName.Instantiated, node);
            }
            AddChild(node);
            EmitSignal(SignalName.Gotten, node);
            return node as T;
        }

        // Release a node back to the pool and remove it from the scene tree.
        public virtual void Release(Node node)
        {
            RemoveChild(node);
            queue.Enqueue(node);
            EmitSignal(SignalName.Released, node);
        }

        // Free a node from the pool and remove it from the scene tree at the end of the frame.
        public virtual void Free(Node node)
        {
            node.QueueFree();
            EmitSignal(SignalName.Freed, node);
        }
    }
}


