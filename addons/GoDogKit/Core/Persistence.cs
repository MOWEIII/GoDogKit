using System;
using System.Collections.Generic;
using Godot;

namespace GoDogKit;

// public static class Persistence
// {
//     public static Dictionary<string, Node> Nodes { get; private set; }
//     public readonly static SceneTree SceneTree;
//     public readonly static Node Node;

//     static Persistence()
//     {
//         Nodes = [];

//         SceneTree = Engine.GetMainLoop() as SceneTree;

//         if (SceneTree is null)
//         {
//             throw new Exception("SceneTree Not Found! Unable To Create Persistence Node!");
//         }

//         Node node = new()
//         {
//             Name = "Persistences"
//         };

//         SceneTree.Root.AddChild(node);

//         Node = node;
//     }

//     public static void Add(Node node, string id, bool rename = false)
//     {
//         if (node.IsInsideTree())
//         {
//             node.Reparent(Node);
//         }
//         else
//         {
//             Node.AddChild(node);
//         }

//         if (rename) node.Name = id;

//         Nodes.Add(id, node);
//     }

//     public static void Remove(string id)
//     {
//         if (Nodes.TryGetValue(id, out Node node))
//         {
//             Nodes.Remove(id);

//             node.QueueFree();
//         }
//     }

//     public static void RemoveAll()
//     {
//         foreach (Node node in Nodes.Values)
//         {
//             node.QueueFree();
//         }

//         Nodes.Clear();
//     }
// }