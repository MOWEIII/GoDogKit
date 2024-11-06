using Godot;

namespace GoDogKit
{
    /// <summary>
    /// Provides a simple way to play a video file as a splash screen.
    /// </summary>
    public partial class BootSplashPlayer : VideoStreamPlayer
    {
        [Export] public bool OneShot { get; set; } = true;
        private static bool m_Over = false;
        public override void _EnterTree()
        {
            if (m_Over) return;

            ProcessMode = ProcessModeEnum.Always;
            GetTree().Paused = true;
            Play();

            Finished += () =>
            {
                GetTree().Paused = false;
                ProcessMode = ProcessModeEnum.Disabled;
                if (OneShot) m_Over = true;
            };
        }
    }
}

