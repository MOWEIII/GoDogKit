using System.Collections.Generic;
using Godot;

namespace GoDogKit;

/// <summary>
/// Provides management for 'GlobalScenes', which are registered by an string key and 
/// can be well identified during scenes changing, freeing, reloading, etc.
/// While handling the persistent nodes which are maintained to be 'GlobalNode'.
/// </summary>
public partial class GlobalSceneManager : GlobalNode
{
    public SceneTree SceneTree { get; } = Engine.GetMainLoop() as SceneTree;
    public PackedScene CurrentScene { get; private set; } = null;
    public List<PackedScene> GlobalScenes { get; private set; } = [];
    public GlobalNode PersistenceNode { get; private set; }
    public Dictionary<string, Node> Persistences { get; private set; }
    private readonly Dictionary<string, PackedScene> m_string2globalScenes = [];

    public GlobalSceneManager()
    {
        // Used as the 'Persistences'.
        GlobalNode node = new()
        {
            Name = "DontFreedOnLoad"
        };

        PersistenceNode = node;

        SceneTree.Root.AddChild(node);
    }

    /// <summary>
    /// Registers a scene as GlobalScene.
    /// </summary>
    public void Register(PackedScene scene, string key)
    {
        if (m_string2globalScenes.TryAdd(key, scene))
        {
            GlobalScenes.Add(scene);
        }
    }

    /// <summary>
    /// Unregisters a GlobalScene.
    /// </summary>    
    public void Unregister(string key)
    {
        if (m_string2globalScenes.TryGetValue(key, out PackedScene value))
        {
            m_string2globalScenes.Remove(key);
            GlobalScenes.Remove(value);
        }
    }

    /// <summary>
    /// Free all non-Global nodes in the scene tree.
    /// </summary>
    public void CleanSceneTree()
    {
        foreach (Node node in SceneTree.Root.GetChildren())
        {
            if (node is not GlobalNode)
            {
                node.QueueFree();
            }
        }
    }

    /// <summary>
    /// Change current Global Scene by key.
    /// Return true if success, else false.
    /// </summary>    
    public bool GoTo(string key)
    {
        if (m_string2globalScenes.TryGetValue(key, out var scene))
        {
            // Clean Tree should be placed after changeD the scene!
            // CleanSceneTree();            

            Error error = SceneTree.ChangeSceneToPacked(scene);

            if (error is not Error.Ok)
            {
                GD.PrintErr(error);
                return false;
            }
            else
            {
                CurrentScene = scene;
                CleanSceneTree();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Change current Global Scene by index relative to register time.
    /// Return true if success, else false.
    /// </summary>    
    public bool GoTo(int index)
    {
        if (index < GlobalScenes.Count)
        {
            PackedScene scene = GlobalScenes[index];

            Error error = SceneTree.ChangeSceneToPacked(scene);

            if (error is not Error.Ok)
            {
                GD.PrintErr(error);
                return false;
            }
            else
            {
                CurrentScene = scene;
                CleanSceneTree();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Go to the next Global Scene by increase the index.
    /// </summary>
    /// <param name="circle"> Whether loop the Global Scenes </param>
    public bool GoNext(bool circle = false)
    {
        int index = GlobalScenes.IndexOf(CurrentScene) + 1;

        if (circle)
        {
            index = index == GlobalScenes.Count ? index++ : index;
        }

        return GoTo(index);
    }

    /// <summary>
    /// Reload current scene.
    /// </summary>
    public void Reload() => GoTo(GlobalScenes.IndexOf(CurrentScene));

    /// <summary>
    /// Add a node into persistences, which means it will survive during scenes' transition.
    /// </summary>    
    /// <param name="id"> A key to retrieve the specific persistent node. </param>
    /// <param name="rename"> Whether to rename the node use id param. </param>
    public void AddPersistence<T>(T node, string id, bool rename = false) where T : Node
    {
        if (node.IsInsideTree())
        {
            node.Reparent(PersistenceNode);
        }
        else
        {
            PersistenceNode.AddChild(node);
        }

        if (rename) node.Name = id;

        Persistences.Add(id, node);
    }

    /// <summary>
    /// Free a persistent node by id.
    /// </summary>    
    public void FreePersistence(string id)
    {
        if (Persistences.TryGetValue(id, out var node))
        {
            Persistences.Remove(id);
            node.QueueFree();
        }
    }

    public void FreeAllPersistences()
    {
        foreach (Node node in Persistences.Values)
        {
            node.QueueFree();
        }

        Persistences.Clear();
    }
}