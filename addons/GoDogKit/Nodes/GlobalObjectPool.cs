using System.Collections.Generic;
using Godot;

namespace GoDogKit;

/// <summary>
/// A Global Manager for Object Pools, Maintains links between PackedScenes and their corresponding m_ObjectPools.
/// Provides methods to register, unregister PackedScenes, get and release objects from object pools.
/// </summary>
public partial class GlobalObjectPool : GlobalNode
{
    private readonly Dictionary<PackedScene, ObjectPool> m_objectPools = [];

    /// <summary>
    /// Registers a PackedScene to the GlobalObjectPool.
    /// </summary>
    /// <param name="scene"> The PackedScene to register. </param>
    /// <param name="poolParent"> The parent node of the ObjectPool. </param>
    /// <param name="poolInitialSize"> The initial size of the ObjectPool. </param>
    public void Register(PackedScene scene, Node poolParent = null, int poolInitialSize = 10)
    {
        if (m_objectPools.ContainsKey(scene))
        {
            // GD.Print(scene.ResourceName + " already registered to GlobalObjectPool.");
            return;
        }

        ObjectPool pool = new()
        {
            Scene = scene,
            Parent = poolParent,
            InitialSize = poolInitialSize
        };

        AddChild(pool);

        m_objectPools.Add(scene, pool);
    }

    /// <summary>
    /// Unregisters a PackedScene from the GlobalObjectPool.
    /// </summary>
    /// <param name="scene"> The PackedScene to unregister. </param>
    public void Unregister(PackedScene scene)
    {
        if (!m_objectPools.TryGetValue(scene, out ObjectPool pool))
        {
            // GD.Print(scene.ResourceName + " not registered to GlobalObjectPool.");
            return;
        }

        pool.Destroy();

        m_objectPools.Remove(scene);
    }

    //Just for simplify coding. Ensure the pool has always been registered.
    private ObjectPool ForceGetPool(PackedScene scene)
    {
        if (!m_objectPools.TryGetValue(scene, out ObjectPool pool))
        {
            Register(scene);
            pool = m_objectPools[scene];
        }

        return pool;
    }

    /// <summary>
    /// Get a node from the corresponding ObjectPool of the given PackedScene.
    /// </summary>
    /// <param name="scene"> The PackedScene to get the node from. </param>
    /// <returns> The node from the corresponding ObjectPool. </returns>
    public Node Get(PackedScene scene)
    {
        return ForceGetPool(scene).Get();
    }

    /// <summary>
    /// Get a node from the corresponding ObjectPool of the given PackedScene as a specific type.
    /// </summary>
    /// <param name="scene"> The PackedScene to get the node from. </param>
    /// <typeparam name="T"> The type to cast the node to. </typeparam>
    /// <returns> The node from the corresponding ObjectPool. </returns>
    public T Get<T>(PackedScene scene) where T : Node
    {
        return Get(scene) as T;
    }

    /// <summary>
    /// Releases a node back to the corresponding ObjectPool of the given PackedScene.
    /// </summary>
    /// <param name="scene"> The PackedScene to release the node to. </param>
    /// <param name="node"> The node to release. </param>
    public void Release(PackedScene scene, Node node)
    {
        ForceGetPool(scene).Release(node);
    }

    /// <summary>
    /// Unregisters all the PackedScenes from the GlobalObjectPool.
    /// </summary>
    public void UnregisterAll()
    {
        foreach (var pool in m_objectPools.Values)
        {
            pool.Destroy();
        }

        m_objectPools.Clear();
    }

    /// <summary>
    /// Get the ObjectPool of the given PackedScene.
    /// If the PackedScene is not registered, it will be registered.
    /// </summary>
    /// <param name="scene"> The PackedScene to get the ObjectPool of. </param>
    /// <returns> The ObjectPool of the given PackedScene. </returns>
    public ObjectPool GetPool(PackedScene scene)
    {
        return ForceGetPool(scene);
    }
}



