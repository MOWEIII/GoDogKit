#if TOOLS
using System.Collections.Generic;
using Godot;

namespace GoDogKit
{
	[Tool]
	public sealed partial class GoDogKitManager : EditorPlugin
	{
		// const string GoDogKitPluginName = "GoDogKit";
		public const string GoDogKitFolderPath = "res://addons/GoDogKit/";
		public const string GoDogKitIconFolderPath = "res://addons/GoDogKit/Icons/";

		private static readonly List<string> GoDogKitNodes = [];
		private static readonly List<string> GoDogKitAutoloads = [];

		private void AddNode(string nodeName, string parentNodeName, string scriptName, string IconName)
		{
			var script = ResourceLoader.Load<Script>(GoDogKitFolderPath + scriptName);
			var icon = ResourceLoader.Load<Texture2D>(GoDogKitIconFolderPath + IconName);

			AddCustomType(nodeName, parentNodeName, script, icon);

			GoDogKitNodes.Add(nodeName);
		}

		private void AddAutoload(string Name, string scriptName)
		{
			AddAutoloadSingleton(Name, GoDogKitFolderPath + scriptName);
			GoDogKitAutoloads.Add(Name);
		}

		private void ClearNodes()
		{
			foreach (var nodeName in GoDogKitNodes)
			{
				RemoveCustomType(nodeName);
			}

			GoDogKitNodes.Clear();
		}

		private void ClearAutoloads()
		{
			foreach (var autoloadName in GoDogKitAutoloads)
			{
				RemoveAutoloadSingleton(autoloadName);
			}
		}

		public override void _EnterTree()
		{
			AddNode("ObjectPool", "Node", "Core/ObjectPool.cs", "ObjectPool.png");
			AddNode("AutoCamera2D", "Camera2D", "Camera/AutoCamera2D.cs", "AutoCamera2D.png");
			AddNode("BootSplashPlayer", "VideoStreamPlayer", "Core/BootSplashPlayer.cs", "BootSplashPlayer.png");

			AddAutoload("GlobalCoroutineLauncher", "Coroutine/GlobalCoroutineLauncher.cs");
			AddAutoload("GlobalInputManager", "Core/GlobalInputManager.cs");
		}

		public override void _ExitTree()
		{
			ClearNodes();

			ClearAutoloads();
		}
	}
}

#endif
