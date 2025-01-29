using Godot;
using System.Collections.Generic;

namespace GoDogKit;

/// <summary>
/// Global manager for non-spatial audio (Watch-out! This is not a spatial audio manager).
/// All AudioStream resources being registered can play globally.
/// It will be automatically created and added into scene tree root if used.
/// </summary>
public partial class GlobalAudioManager : Node
{
    private readonly Dictionary<AudioStream, AudioStreamPlayer> m_players = [];

    public readonly float MaxDb = 20f;

    public readonly float MinDb = -20f;

    // public override void _Ready()
    // {
    //     AudioServer.SetBusLayout
    //     (ResourceLoader.Load<AudioBusLayout>(GoDogKitManager.GoDogKitFolderPath + "/Audio/DefaultBusLayout.tres"));
    // }

    /// <summary>
    /// Registers an audio stream to the global audio manager.
    /// </summary>
    /// <param name="stream"> The audio stream to register. </param>
    /// <param name="bus"> The bus to assign the stream to. </param>
    public AudioStreamPlayer Register(AudioStream stream, string bus = "Master")
    {
        if (m_players.TryGetValue(stream, out AudioStreamPlayer player))
        {
            return player;
        }

        var newPlayer = new AudioStreamPlayer
        {
            Stream = stream,
            Bus = bus
        };

        AddChild(newPlayer);

        m_players.Add(stream, newPlayer);

        return newPlayer;
    }

    /// <summary>
    /// Unregisters an audio stream from the global audio manager.
    /// </summary>
    /// <param name="stream"> The audio stream to unregister. </param>
    public void Unregister(AudioStream stream)
    {
        if (!m_players.TryGetValue(stream, out AudioStreamPlayer player))
        {
            // GD.Print(stream.ResourceName + " not registered to GlobalAudioManager.");
            return;
        }

        player.QueueFree();

        m_players.Remove(stream);
    }

    /// <summary>
    /// Unregisters all audio streams from the global audio manager.
    /// </summary>
    public void UnregisterAll()
    {
        foreach (var player in m_players.Values)
        {
            player.QueueFree();
        }

        m_players.Clear();
    }

    private AudioStreamPlayer ForceGetPlayer(AudioStream stream)
    {
        if (!m_players.TryGetValue(stream, out var player))
        {
            Register(stream);
            player = m_players[stream];
        }

        return player;
    }

    public void Play(AudioStream stream) => ForceGetPlayer(stream).Play();

    public void Stop(AudioStream stream) => ForceGetPlayer(stream).Stop();

    public AudioStreamPlayer GetPlayer(AudioStream stream) => ForceGetPlayer(stream);

    public static void SetBusMute(string bus, bool mute)
     => AudioServer.SetBusMute(AudioServer.GetBusIndex(bus), mute);

    /// <summary>
    /// Set the volume of a bus in float range [0, 1] base on the min and max db values.
    /// </summary>
    /// <param name="bus"> The bus to set the volume of. </param>
    /// <param name="volume"> The volume of the bus in float range [0, 1]. </param>
    public void SetBusVolume(string bus, float volume = 0.5f)
    {
        var busIndex = AudioServer.GetBusIndex(bus);

        AudioServer.SetBusMute(busIndex, false);

        if (volume <= 0f)
        {
            AudioServer.SetBusMute(busIndex, true);
        }
        else
        {
            var db = Mathf.Lerp(MinDb, MaxDb, volume);
            AudioServer.SetBusVolumeDb(busIndex, db);
        }
    }

    public static void MuteAll()
    {
        for (int i = AudioServer.BusCount - 1; i >= 0; i--)
        {
            AudioServer.SetBusMute(i, true);
        }
    }
}