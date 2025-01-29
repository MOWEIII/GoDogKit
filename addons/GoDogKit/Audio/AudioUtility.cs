using Godot;

namespace GoDogKit;

public static class AudioUtility
{
    /// <summary>
    /// Register this to the global audio manager.
    /// </summary>
    /// <param name="bus"> The bus to register to. </param>
    /// <returns> Itself </returns>
    public static AudioStream Register(this AudioStream stream, string bus = "Master")
    {
        GlobalUtility.AudioManager.Register(stream, bus);
        return stream;
    }

    /// <summary>
    /// Unregister this from the global audio manager.
    /// </summary>        
    /// <returns> Itself </returns>
    public static AudioStream Unregister(this AudioStream stream)
    {
        GlobalUtility.AudioManager.Unregister(stream);
        return stream;
    }

    /// <summary>
    /// Play this stream from the global audio manager.
    /// </summary>    
    public static void Play(this AudioStream stream)
    => GlobalUtility.AudioManager.Play(stream);

    /// <summary>
    /// Stop playing this stream from the global audio manager.
    /// </summary>
    public static void Stop(this AudioStream stream)
    => GlobalUtility.AudioManager.Stop(stream);

    /// <summary>
    /// Get the player of this stream from the global audio manager.
    /// </summary>
    public static AudioStreamPlayer GetPlayer(this AudioStream stream)
    => GlobalUtility.AudioManager.GetPlayer(stream);
}