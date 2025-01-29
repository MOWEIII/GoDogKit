using Godot;

namespace GoDogKit;

[GlobalClass]
public partial class MenuBase : Control
{
    [ExportGroup("Buttons")]
    [Export] public Button PlayBut { get; private set; }
    [Export] public Button SettingsBut { get; private set; }
    [Export] public Button ExitBut { get; private set; }

    public override void _Ready()
    {
        PlayBut.Pressed += Play;
        SettingsBut.Pressed += Settings;
        PlayBut.Pressed += Exit;
    }

    public virtual void Play() { }
    public virtual void Settings() { }
    public virtual void Exit() { GetTree().Quit(); }
}