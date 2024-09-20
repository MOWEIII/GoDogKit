using Godot;

namespace GoDogKit
{
    public static class Extensions
    {
        public static Node Instantiate(this PackedScene scene, Node parent = null)
        {
            Node node = scene.Instantiate();

            if (parent != null)
            {
                parent.AddChild(node);
            }
            else
            {
                node.GetTree().Root.AddChild(node);
            }

            return node;
        }
    }
}

