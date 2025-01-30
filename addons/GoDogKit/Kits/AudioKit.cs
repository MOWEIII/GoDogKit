using Godot;

namespace GoDogKit;

/// <summary>
/// Provides shortcuts of GlobalObjectPool. And other utilities.
/// </summary>
public static class AudioKit
{
    /// <summary>
    /// Register this to the GlobalAudioManager.
    /// </summary>
    /// <param name="bus"> The bus to register to. </param>
    /// <returns> Itself </returns>
    public static AudioStream Register(this AudioStream stream, string bus = "Master")
    {
        Global.AudioManager.Register(stream, bus);
        return stream;
    }

    /// <summary>
    /// Unregister this from the GlobalAudioManager.
    /// </summary>        
    /// <returns> Itself </returns>
    public static AudioStream Unregister(this AudioStream stream)
    {
        Global.AudioManager.Unregister(stream);
        return stream;
    }

    /// <summary>
    /// Play this stream from the GlobalAudioManager.
    /// </summary>    
    public static void Play(this AudioStream stream)
    => Global.AudioManager.Play(stream);

    /// <summary>
    /// Stop playing this stream from the GlobalAudioManager.
    /// </summary>
    public static void Stop(this AudioStream stream)
    => Global.AudioManager.Stop(stream);

    /// <summary>
    /// Get the player of this stream from the GlobalAudioManager.
    /// </summary>
    public static AudioStreamPlayer GetPlayer(this AudioStream stream)
    => Global.AudioManager.GetPlayer(stream);
}