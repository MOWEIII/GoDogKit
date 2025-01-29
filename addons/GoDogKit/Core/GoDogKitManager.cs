#if TOOLS
using System.Collections.Generic;
using Godot;

namespace GoDogKit;

[Tool]
public sealed partial class GoDogKitManager : EditorPlugin
{
	// const string GoDogKitPluginName = "GoDogKit";
	public const string GoDogKitFolderPath = "res://addons/GoDogKit/";
	public const string GoDogKitIconFolderPath = "res://addons/GoDogKit/Icons/";

	private static readonly List<string> GoDogKitNodes = [];
	private static readonly List<string> GoDogKitAutoloads = [];
	// public static readonly ExportManager ExportManager = new();

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

	public override void _EnablePlugin()
	{
		// AddNode("ObjectPool", "Node", "ObjectPool/ObjectPool.cs", "ObjectPool.png");
		// AddNode("ManagedPool", "ObjectPool", "ObjectPool/ManagedPool.cs", "ObjectPool.png");
		// AddNode("AutoCamera2D", "Camera2D", "Camera/AutoCamera2D.cs", "AutoCamera2D.png");
		// AddNode("BootSplashPlayer", "VideoStreamPlayer", "UI/BootSplashPlayer.cs", "BootSplashPlayer.png");

		// AddAutoload("GlobalCoroutineLauncher", "Coroutine/GlobalCoroutineLauncher.cs");
		// AddAutoload("GlobalInputManager", "Input/GlobalInputManager.cs");
		// AddAutoload("GlobalObjectPool", "ObjectPool/GlobalObjectPool.cs");
		// AddAutoload("GlobalAudioManager", "Audio/GlobalAudioManager.cs");

		// AddExportPlugin(ExportManager);
	}

	public override void _DisablePlugin()
	{
		// ClearNodes();
		// ClearAutoloads();
		// RemoveExportPlugin(ExportManager);
	}
}


#endif
