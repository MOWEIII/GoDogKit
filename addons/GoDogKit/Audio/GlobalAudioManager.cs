using Godot;
using System.Collections.Generic;

namespace GoDogKit
{
    /// <summary>
    /// Global manager for non-spatial audio (Watch-out! This is not a spatial audio manager).
    /// </summary>
    public partial class GlobalAudioManager : Singleton<GlobalAudioManager>
    {
        private readonly Dictionary<AudioStream, AudioStreamPlayer> m_players = [];

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
        public static void Register(AudioStream stream, string bus = "Master")
        {
            if (Instance.m_players.ContainsKey(stream))
            {
                GD.Print(stream.ResourceName + " already registered to GlobalAudioManager.");
                return;
            }

            var player = new AudioStreamPlayer
            {
                Stream = stream,
                Bus = bus
            };

            Instance.AddChild(player);

            Instance.m_players.Add(stream, player);
        }

        /// <summary>
        /// Unregisters an audio stream from the global audio manager.
        /// </summary>
        /// <param name="stream"> The audio stream to unregister. </param>
        public static void Unregister(AudioStream stream)
        {
            if (!Instance.m_players.TryGetValue(stream, out AudioStreamPlayer player))
            {
                GD.Print(stream.ResourceName + " not registered to GlobalAudioManager.");
                return;
            }

            player.QueueFree();

            Instance.m_players.Remove(stream);
        }

        private static AudioStreamPlayer ForceGetPlayer(AudioStream stream)
        {
            if (!Instance.m_players.TryGetValue(stream, out var player))
            {
                Register(stream);
                player = Instance.m_players[stream];
            }

            return player;
        }

        public static void Play(AudioStream stream) => ForceGetPlayer(stream).Play();

        public static void Stop(AudioStream stream) => ForceGetPlayer(stream).Stop();

        public static AudioStreamPlayer GetPlayer(AudioStream stream) => ForceGetPlayer(stream);

        public static void SetBusMute(string bus, bool mute)
         => AudioServer.SetBusMute(AudioServer.GetBusIndex(bus), mute);

        public static void SetBusVolume(string bus, float db)
         => AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(bus), db);

        public static void MuteAll()
        {
            for (int i = AudioServer.BusCount - 1; i >= 0; i--)
            {
                AudioServer.SetBusMute(i, true);
            }
        }
    }
}

