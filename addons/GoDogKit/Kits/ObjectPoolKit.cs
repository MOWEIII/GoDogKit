using Godot;

namespace GoDogKit;

/// <summary>
/// Provides shortcuts of GlobalObjectPool. And other utilities.
/// </summary>
public static class ObjectPoolKit
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
    public static bool ReleaseToManagedPool(this Node node)
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
    public static bool FreeByManagedPool(this Node node)
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
    public static PackedScene RegisterToGlobalObjectPool(this PackedScene scene, Node poolParent = null, int poolInitialSize = 10)
    {
        Global.ObjectPool.Register(scene, poolParent, poolInitialSize);
        return scene;
    }

    /// <summary>
    /// Unregister this from the global object pool.
    /// </summary>
    /// <returns> Itself </returns>    
    public static PackedScene UnregisterFromGlobalObjectPool(this PackedScene scene)
    {
        Global.ObjectPool.Unregister(scene);
        return scene;
    }

    public static T GetFromGlobalObjectPool<T>(this PackedScene scene) where T : Node
    => Global.ObjectPool.Get<T>(scene);

    public static void ReleaseToGlobalObjectPool(this PackedScene scene, Node node)
    => Global.ObjectPool.Release(scene, node);

    public static ObjectPool GetGlobalObjectPool(this PackedScene scene)
    => Global.ObjectPool.GetPool(scene);
}