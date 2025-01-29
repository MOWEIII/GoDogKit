using Godot;

namespace GoDogKit;

public static class ObjectPoolUtility
{
    /// <summary>
    /// Create a new ObjectPool node with this PackedScene.
    /// </summary>
    public static ObjectPool CreateObjectPool(this PackedScene scene, Node parent = null, int initialSize = 10)
    {
        return new()
        {
            Scene = scene,
            Parent = parent,
            InitialSize = initialSize
        };
    }

    /// <summary>
    /// Create a new ManagedPool node with this PackedScene.
    /// </summary>
    public static ManagedPool CreateManagedPool(this PackedScene scene, Node parent = null, int initialSize = 10)
    {
        return new()
        {
            Scene = scene,
            Parent = parent,
            InitialSize = initialSize
        };
    }

    /// <summary>
    /// Release this node, if it's managed by a ManagedPool.
    /// </summary>    
    /// <returns> Whether the node is released successfully. </returns>
    public static bool TryRelease(this Node node)
    {
        if (ManagedPool.ManagedMap.TryGetValue(node, out ManagedPool pool))
        {
            pool.Release(node);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Free this node, if it's managed by a ManagedPool.
    /// </summary>    
    /// <returns> Whether the node is free successfully. </returns>
    public static bool TryFree(this Node node)
    {
        if (ManagedPool.ManagedMap.TryGetValue(node, out var pool))
        {
            pool.Free(node);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Register this to the global object pool.
    /// </summary>        
    /// <param name="poolParent"> The parent of the pool. </param>
    /// <param name="poolInitialSize"> The initial size of the pool. </param>
    /// <returns> Itself </returns>
    public static PackedScene RegisterGlobally(this PackedScene scene, Node poolParent = null, int poolInitialSize = 10)
    {
        GlobalUtility.ObjectPool.Register(scene, poolParent, poolInitialSize);
        return scene;
    }

    /// <summary>
    /// Unregister this from the global object pool.
    /// </summary>
    /// <returns> Itself </returns>    
    public static PackedScene UnregisterGlobally(this PackedScene scene)
    {
        GlobalUtility.ObjectPool.Unregister(scene);
        return scene;
    }

    public static Node GetGlobally(this PackedScene scene)
    => GlobalUtility.ObjectPool.Get(scene);

    public static T GetGlobally<T>(this PackedScene scene) where T : Node
    => GetGlobally(scene) as T;

    public static void ReleaseGlobally(this PackedScene scene, Node node)
    => GlobalUtility.ObjectPool.Release(scene, node);

    public static ObjectPool GetPoolGlobally(this PackedScene scene)
    => GlobalUtility.ObjectPool.GetPool(scene);

}