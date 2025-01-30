using Godot;

namespace GoDogKit;

/// <summary>
/// Provides shortcuts of GlobalSceneManager. And other utilities.
/// </summary>
public static class SceneKit
{
    /// <summary>
    /// Go to GlobalScene with given key.
    /// </summary>    
    /// <returns> True if Succces, else false. </returns>
    public static bool GoToScene(string key)
    => Global.SceneManager.GoTo(key);

    /// <summary>
    /// Go to GlobalScene with given index.
    /// </summary>    
    /// <returns> True if Succces, else false. </returns>
    public static bool GoToScene(int index)
    => Global.SceneManager.GoTo(index);

    public static void RegisterToGlobalScenes(this PackedScene scene, string key)
    => Global.SceneManager.Register(scene, key);

    public static void UnregisterFromGlobalScenes(string key)
    => Global.SceneManager.Unregister(key);
}