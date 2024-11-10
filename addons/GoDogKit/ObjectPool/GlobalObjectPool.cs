using System.Collections.Generic;
using Godot;

namespace GoDogKit
{
    /// <summary>
    /// A Global Manager for Object Pools, Maintains links between PackedScenes and their corresponding ObjectPools.
    /// Provides methods to register, unregister, get and release objects from object pools.
    /// </summary>
    public partial class GlobalObjectPool : Singleton<GlobalObjectPool>
    {
        private readonly Dictionary<PackedScene, ObjectPool> ObjectPools = [];

        /// <summary>
        /// Registers a PackedScene to the GlobalObjectPool.
        /// </summary>
        /// <param name="scene"> The PackedScene to register. </param>
        /// <param name="poolParent"> The parent node of the ObjectPool. </param>
        /// <param name="poolInitialSize"> The initial size of the ObjectPool. </param>
        public static void Register(PackedScene scene, Node poolParent = null, int poolInitialSize = 10)
        {
            if (Instance.ObjectPools.ContainsKey(scene))
            {
                GD.Print(scene.ResourceName + " already registered to GlobalObjectPool.");
                return;
            }

            ObjectPool pool = new()
            {
                Scene = scene,
                Parent = poolParent,
                InitialSize = poolInitialSize
            };

            Instance.AddChild(pool);

            Instance.ObjectPools.Add(scene, pool);
        }

        /// <summary>
        /// Unregisters a PackedScene from the GlobalObjectPool.
        /// </summary>
        /// <param name="scene"> The PackedScene to unregister. </param>
        public static void Unregister(PackedScene scene)
        {
            if (!Instance.ObjectPools.TryGetValue(scene, out ObjectPool pool))
            {
                GD.Print(scene.ResourceName + " not registered to GlobalObjectPool.");
                return;
            }

            pool.Destroy();

            Instance.ObjectPools.Remove(scene);
        }

        //Just for simplify coding. Ensure the pool has always been registered.
        private static ObjectPool ForceGetPool(PackedScene scene)
        {
            if (!Instance.ObjectPools.TryGetValue(scene, out ObjectPool pool))
            {
                Register(scene);
                pool = Instance.ObjectPools[scene];
            }

            return pool;
        }

        /// <summary>
        /// Get a node from the corresponding ObjectPool of the given PackedScene.
        /// </summary>
        /// <param name="scene"> The PackedScene to get the node from. </param>
        /// <returns> The node from the corresponding ObjectPool. </returns>
        public static Node Get(PackedScene scene)
        {
            return ForceGetPool(scene).Get();
        }

        /// <summary>
        /// Get a node from the corresponding ObjectPool of the given PackedScene as a specific type.
        /// </summary>
        /// <param name="scene"> The PackedScene to get the node from. </param>
        /// <typeparam name="T"> The type to cast the node to. </typeparam>
        /// <returns> The node from the corresponding ObjectPool. </returns>
        public static T Get<T>(PackedScene scene) where T : Node
        {
            return Get(scene) as T;
        }

        /// <summary>
        /// Releases a node back to the corresponding ObjectPool of the given PackedScene.
        /// </summary>
        /// <param name="scene"> The PackedScene to release the node to. </param>
        /// <param name="node"> The node to release. </param>
        public static void Release(PackedScene scene, Node node)
        {
            ForceGetPool(scene).Release(node);
        }

        /// <summary>
        /// Unregisters all the PackedScenes from the GlobalObjectPool.
        /// </summary>
        public static void UnregisterAll()
        {
            foreach (var pool in Instance.ObjectPools.Values)
            {
                pool.Destroy();
            }

            Instance.ObjectPools.Clear();
        }

        /// <summary>
        /// Get the ObjectPool of the given PackedScene.
        /// If the PackedScene is not registered, it will be registered.
        /// </summary>
        /// <param name="scene"> The PackedScene to get the ObjectPool of. </param>
        /// <returns> The ObjectPool of the given PackedScene. </returns>
        public static ObjectPool GetPool(PackedScene scene)
        {
            return ForceGetPool(scene);
        }
    }
}


