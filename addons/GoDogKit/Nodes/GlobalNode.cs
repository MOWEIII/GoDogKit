using Godot;

namespace GoDogKit;

/// <summary>
/// Special Node which will not be freed during GoDogKit scene management.
/// </summary>
[GlobalClass] public partial class GlobalNode : Node;