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

        public override void _Ready()
        {
            Finished += OnFinished;

            Play();
        }

        private void OnFinished()
        {
            GetTree().ChangeSceneToPacked(NextScene);
        }
    }
}

