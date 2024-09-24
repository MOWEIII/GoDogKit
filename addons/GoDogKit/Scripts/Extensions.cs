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

        public static Vector2 RandomShpere(float radius)
        {
            var biasX = (float)GD.RandRange(-radius, radius);
            var biasY = (float)GD.RandRange(-radius, radius);
            return new Vector2(biasX, biasY);
        }
    }
}

