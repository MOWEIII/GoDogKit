using Godot;
using Godot.Collections;

namespace GoDogKit
{
    public partial class CutScene : Control
    {
        [Export] public string Path { get; set; }
        [Export] public bool AutoSkip { get; set; }
        [Export] public bool InputSkip { get; set; }
        [Export] public Array<InputEvent> SkipInputs { get; set; }
        [Signal] public delegate void LoadedEventHandler();
        [Signal] public delegate void ProgressChangedEventHandler(double progress);
        private LoadTask<PackedScene> m_LoadTask;

        public override void _Ready()
        {
            m_LoadTask = RuntimeLoader.Load<PackedScene>(Path);

            if (AutoSkip)
            {
                Loaded += Skip;
            }
        }

        public override void _Process(double delta)
        {
            // GD.Print("progress: " + m_LoadTask.Progress + " status: " + m_LoadTask.Status);
            EmitSignal(SignalName.ProgressChanged, m_LoadTask.Progress);

            if (m_LoadTask.Status == ResourceLoader.ThreadLoadStatus.Loaded)
                EmitSignal(SignalName.Loaded);
        }

        public override void _Input(InputEvent @event)
        {
            if (InputSkip && m_LoadTask.Status == ResourceLoader.ThreadLoadStatus.Loaded)
            {
                foreach (InputEvent skipEvent in SkipInputs)
                {
                    if (@event.GetType() == skipEvent.GetType()) Skip();
                }
            }
        }

        public void Skip()
        {
            GetTree().ChangeSceneToPacked(m_LoadTask.Result);
        }
    }
}

