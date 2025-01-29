//TODO: All the 'Global' relatives should be related to this Utility.
using System;
using System.Collections.Generic;
using Godot;

namespace GoDogKit;

/// <summary>
/// Provides all global classes and relative functions, only initialize if being used.
/// All the global classes inner are response to a singleton.
/// </summary>
public static class GlobalUtility
{
    private static readonly Dictionary<Type, Node> m_globals;
    public static SceneTree SceneTree
    {
        get => Engine.GetMainLoop() as SceneTree;
    }

    // Register all global class while using.
    static GlobalUtility()
    {
        m_globals = [];
        m_globals.Add(typeof(GlobalAudioManager), null);
        m_globals.Add(typeof(GlobalCoroutineLauncher), null);
        m_globals.Add(typeof(GlobalInputManager), null);
        m_globals.Add(typeof(GlobalObjectPool), null);
    }

    public static GlobalAudioManager AudioManager
    {
        get => ApplyGlobal<GlobalAudioManager>();
    }

    public static GlobalCoroutineLauncher CoroutineLauncher
    {
        get => ApplyGlobal<GlobalCoroutineLauncher>();
    }

    public static GlobalInputManager InputManager
    {
        get => ApplyGlobal<GlobalInputManager>();
    }

    public static GlobalObjectPool ObjectPool
    {
        get => ApplyGlobal<GlobalObjectPool>();
    }

    public static T ApplyGlobal<T>() where T : Node, new()
    {
        var type = typeof(T);

        if (m_globals.TryGetValue(type, out Node node))
        {
            // Ensure that only one instance remains.
            if (node is null)
            {
                // GD.Print($"Create an instance of {type.Name}!");
                T instance = new() { Name = type.Name };
                m_globals[type] = instance;
                node = instance;
                SceneTree.Root.AddChild(instance);
            }

            return node as T;
        }
        else return null;
    }
}