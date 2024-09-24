#if TOOLS
using System.Collections.Generic;
using Godot;

namespace GoDogKit
{
	[Tool]
	public sealed partial class GoDogKitManager : EditorPlugin
	{
		// const string GoDogKitPluginName = "GoDogKit";
		public const string GoDogKitScriptFolderPath = "res://addons/GoDogKit/Scripts/";
		public const string GoDogKitIconFolderPath = "res://addons/GoDogKit/Icons/";

		private static readonly List<string> GoDogKitNodes = new();
		private void AddNode(string nodeName, string parentNodeName, string scriptName, string IconName)
		{
			var script = ResourceLoader.Load<Script>(GoDogKitScriptFolderPath + scriptName);
			var icon = ResourceLoader.Load<Texture2D>(GoDogKitIconFolderPath + IconName);

			AddCustomType(nodeName, parentNodeName, script, icon);

			GoDogKitNodes.Add(nodeName);
		}

		private void ClearNodes()
		{
			foreach (var nodeName in GoDogKitNodes)
			{
				RemoveCustomType(nodeName);
			}

			GoDogKitNodes.Clear();
		}

		public override void _EnterTree()
		{
			AddNode("ObjectPool", "Node", "ObjectPool.cs", "ObjectPool.png");
			AddNode("AutoCamera2D", "Camera2D", "AutoCamera2D.cs", "AutoCamera2D.png");
		}

		public override void _ExitTree()
		{
			ClearNodes();
		}
	}
}

#endif
