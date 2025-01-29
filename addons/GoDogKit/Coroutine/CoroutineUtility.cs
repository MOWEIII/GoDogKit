using System.Collections;
using Godot;

namespace GoDogKit;

/// <summary>
/// Aim to Simplify the usages of Coroutine.
/// </summary>
public static class CoroutineUtility
{
    /// <summary>
    /// Start a coroutine by GlobalCoroutineLauncher.
    /// </summary>    
    public static void StartCoroutine(this Node node, Coroutine coroutine)
    {
        GlobalUtility.CoroutineLauncher.StartCoroutine(coroutine);
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
        GlobalUtility.CoroutineLauncher.StopCoroutine(coroutine);
    }

    /// <summary>
    /// Get the coroutines' info of the GlobalCoroutineLauncher.
    /// </summary>
    public static CoroutineLauncher.CoroutineLaunchInfo GetGlobalInfo()
    => GlobalUtility.CoroutineLauncher.GetInfo();
}