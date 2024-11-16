using Godot;

namespace GoDogKit
{
    /// <summary>
    /// Provides a simple way to play a video file as a splash screen.
    /// Currently need to set the "Main Scene" of the project to this node to functionate.
    /// You can also use this as an cut scene CG player.
    /// </summary>
    public partial class BootSplashPlayer : VideoStreamPlayer
    {
        [Export] public PackedScene NextScene { get; set; }

        /// <summary>
        /// If this path is not null, it will load the video from the specified path.        
        /// Which means you can easily switch the video by changing the path,
        /// even after exporting the project.
        /// </summary>
        [Export] public string StreamingPath { get; set; }

        public override void _Ready()
        {
            Finished += OnFinished;

            if (!string.IsNullOrEmpty(StreamingPath))
            {
                Stream = RuntimeLoader.LoadVideoStreaming(StreamingPath);
            }

            if (Stream == null)
            {
                GD.PushError("No video file found.");
                return;
            }

            Play();
        }

        private void OnFinished()
        {
            GetTree().ChangeSceneToPacked(NextScene);
        }
    }
}

