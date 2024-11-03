using Godot;

namespace GoDogKit
{
    /// <summary>
    /// Base class for all UI elements. Inherit from CanvasLayer. Provides common functionality for all UI elements.
    /// </summary>
    public partial class UIBase : CanvasLayer
    {
        [Export] public UIBase[] Children { get; set; }

        /// <summary>
        /// Default switch focus to a control if it's not null when UI is opened.
        /// </summary>
        [Export] public Control FirstGrab { get; set; }
        public UIBase Parent { get; set; }

        // New added two signals for UIBase denotes the opening and closing of the UI element.
        [Signal] public delegate void OnOpenEventHandler();
        [Signal] public delegate void OnCloseEventHandler();

        public override void _EnterTree()
        {
            foreach (UIBase child in Children)
            {
                child.Parent = this;
            }
        }

        public override void _Ready()
        {
            Layer = Parent.Layer + 1;
        }

        public virtual UIBase OpenChild(string UIName)
        {
            foreach (UIBase child in Children)
            {
                if (child.Name == UIName)
                {
                    child.Show();
                    child.EmitSignal(SignalName.OnOpen);
                    return child;
                }
            }

            return null;
        }

        public virtual UIBase OpenChild(int index)
        {
            if (index < 0 || index >= Children.Length)
            {
                return null;
            }

            UIBase child = Children[index];
            child.Show();
            child.EmitSignal(SignalName.OnOpen);
            return child;
        }

        public virtual void CloseChild(string UIName)
        {
            foreach (UIBase child in Children)
            {
                if (child.Name == UIName)
                {
                    child.Hide();
                    child.EmitSignal(SignalName.OnClose);
                    return;
                }
            }
        }

        public virtual void CloseChild(int index)
        {
            if (index < 0 || index >= Children.Length)
            {
                return;
            }

            UIBase child = Children[index];
            child.Hide();
            child.EmitSignal(SignalName.OnClose);
        }

        public virtual void Open()
        {
            Show();
            ProcessMode = ProcessModeEnum.Inherit;
        }

        public virtual void Close()
        {
            Hide();
            ProcessMode = ProcessModeEnum.Disabled;
        }

        // Override old expression. Choose Open and Close instead of Show and Hide.
        protected virtual new void Show() { base.Show(); FirstGrab?.GrabFocus(); }
        protected virtual new void Hide() { base.Hide(); }
    }
}
