//TODO: This file is deprecated. Waiting on next update...
using Godot;

namespace GoDogKit;

// /// <summary>
// /// Base class for all UI elements. Inherit from CanvasLayer. Provides common functionality for all UI elements.
// /// </summary>
// public partial class UIBase : CanvasLayer
// {
//     [Export] public UIBase[] Children { get; set; }

//     /// <summary>
//     /// Default switch focus to a control if it's not null when UI is opened.
//     /// </summary>
//     [Export] public Control FirstGrab { get; set; }
//     [Export] public ProcessModeEnum OpenProcessMode { get; set; } = ProcessModeEnum.Inherit;
//     [Export] public ProcessModeEnum CloseProcessMode { get; set; } = ProcessModeEnum.Disabled;
//     public UIBase Parent { get; set; }

//     // New added two signals for UIBase denotes the opening and closing of the UI element.
//     [Signal] public delegate void OnOpenEventHandler();
//     [Signal] public delegate void OnCloseEventHandler();

//     public override void _Ready()
//     {
//         foreach (UIBase child in Children)
//         {
//             child.Parent = this;
//         }

//         if (Parent != null) Layer = Parent.Layer + 1;
//     }

//     public virtual UIBase OpenChild(string UIName)
//     {
//         foreach (UIBase child in Children)
//         {
//             if (child.Name == UIName)
//             {
//                 child.Open();
//                 return child;
//             }
//         }

//         return null;
//     }

//     public virtual UIBase OpenChild(int index)
//     {
//         if (index < 0 || index >= Children.Length)
//         {
//             return null;
//         }

//         UIBase child = Children[index];
//         child.Open();
//         return child;
//     }

//     public virtual UIBase CloseChild(string UIName)
//     {
//         foreach (UIBase child in Children)
//         {
//             if (child.Name == UIName)
//             {
//                 child.Close();
//                 return child;
//             }
//         }

//         return null;
//     }

//     public virtual UIBase CloseChild(int index)
//     {
//         if (index < 0 || index >= Children.Length)
//         {
//             return null;
//         }

//         UIBase child = Children[index];
//         child.Close();
//         return child;
//     }

//     public virtual void Open()
//     {
//         Show();
//         FirstGrab?.GrabFocus();
//         EmitSignal(SignalName.OnOpen);
//         ProcessMode = OpenProcessMode;
//     }

//     public virtual void Close()
//     {
//         Hide();
//         EmitSignal(SignalName.OnClose);
//         ProcessMode = CloseProcessMode;
//     }

//     // Override old expression. Choose Open and Close instead of Show and Hide.
//     protected virtual new void Show() { base.Show(); }
//     protected virtual new void Hide() { base.Hide(); }
// }