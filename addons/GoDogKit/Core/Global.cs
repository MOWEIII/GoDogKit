using System;
using System.Collections.Generic;
using Godot;

namespace GoDogKit;

/// <summary>
/// Provides all Global Classes and relative functions,
/// the Global Classes only be instantiated when using them.
/// All the global classes inner are response to a singleton.
/// </summary>
public static class Global
{
    private static readonly Dictionary<Type, GlobalNode> m_globals;
    public static SceneTree SceneTree
    {
        get => Engine.GetMainLoop() as SceneTree;
    }

    // Register all global class while using.
    static Global()
    {
        m_globals = [];
        m_globals.Add(typeof(GlobalAudioManager), null);
        m_globals.Add(typeof(GlobalCoroutineLauncher), null);
        m_globals.Add(typeof(GlobalInputManager), null);
        m_globals.Add(typeof(GlobalObjectPool), null);
        m_globals.Add(typeof(GlobalSceneManager), null);
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

    public static GlobalSceneManager SceneManager
    {
        get => ApplyGlobal<GlobalSceneManager>();
    }

    public static T ApplyGlobal<T>() where T : GlobalNode, new()
    {
        var type = typeof(T);

        if (m_globals.TryGetValue(type, out var node))
        {
            // Ensure that only one instance remains.
            if (node is null)
            {
                // GD.Print($"Create an instance of {type.Name}!");
                T instance = new() { Name = type.Name };
                m_globals[type] = instance;
                node = instance;

                SceneTree.Root.CallDeferred(Node.MethodName.AddChild, node);
            }

            return node as T;
        }
        else return null;
    }
}