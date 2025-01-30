using System.Collections.Generic;
using Godot;

namespace GoDogKit;

/// <summary>
/// Provides handy methods about nodes.
/// </summary>
public static class NodeKit
{
    private static readonly Dictionary<Node, Node> m_relation_caches = [];

    /// <summary>
    /// Add the node back to it's parent if the parent still exists.
    /// </summary>
    public static void Enable(this Node node)
    {
        if (node.IsInsideTree()) return;

        if (m_relation_caches.TryGetValue(node, out var parent))
        {
            if (parent is null) node.QueueFree();
            else parent.AddChild(node);

            m_relation_caches.Remove(node);
        }
    }
    /// <summary>
    /// Set the process mode to disabled.
    /// </summary>
    public static void Disable(this Node node)
    {
        if (!node.IsInsideTree()) return;

        Node parent = node.GetParent();

        if (m_relation_caches.TryAdd(node, parent))
        {
            parent.RemoveChild(node);
        }
    }
}