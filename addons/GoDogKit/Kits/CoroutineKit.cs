using System;
using System.Collections;
using Godot;

namespace GoDogKit;

/// <summary>
/// Aim to Simplify the usages of Coroutine.
/// </summary>
public static class CoroutineKit
{
    /// <summary>
    /// Start a coroutine by GlobalCoroutineLauncher.
    /// </summary>    
    public static void StartCoroutine(this Node node, Coroutine coroutine)
    {
        Global.CoroutineLauncher.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Start a coroutine by GlobalCoroutineLauncher.
    /// </summary>
    public static void StartCoroutine(this Node node, Func<IEnumerator> iterator)
    {
        StartCoroutine(node, new Coroutine(iterator()));
    }

    /// <summary>
    /// Start a coroutine by GlobalCoroutineLauncher.
    /// </summary>
    public static void StartCoroutine(this Node node, Func<IEnumerable> iterator)
    {
        StartCoroutine(node, iterator().GetEnumerator());
    }

    /// <summary>
    /// Start a coroutine by GlobalCoroutineLauncher.
    /// </summary>    
    public static void StartCoroutine(this Node node, IEnumerator enumerator)
    {
        StartCoroutine(node, new Coroutine(enumerator));
    }

    /// <summary>
    /// Start a coroutine by GlobalCoroutineLauncher.
    /// </summary>    
    public static void StartCoroutine(this Node node, IEnumerable enumerable)
    {
        StartCoroutine(node, enumerable.GetEnumerator());
    }

    /// <summary>
    /// Stop a coroutine by GlobalCoroutineLauncher.
    /// </summary>    
    public static void StopCoroutine(this Node node, Coroutine coroutine)
    {
        Global.CoroutineLauncher.StopCoroutine(coroutine);
    }

    /// <summary>
    /// Get the coroutines' info of the GlobalCoroutineLauncher.
    /// </summary>
    public static CoroutineLauncher.CoroutineLaunchInfo GetGlobalInfo()
    => Global.CoroutineLauncher.GetInfo();
}