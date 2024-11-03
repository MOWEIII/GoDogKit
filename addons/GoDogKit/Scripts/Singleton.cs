using Godot;

namespace GoDogKit
{
    /// <summary>
    /// Easy base for singleton nodes.
    /// </summary>
    public partial class Singleton<T> : Node where T : Node
    {
        public static T Instance { get; private set; }
        protected Singleton() { }
        public override void _EnterTree()
        {
            Instance ??= this as T;

            if (Instance != this)
            {
                Instance.QueueFree();
                Instance = this as T;
            }
        }
    }
}

