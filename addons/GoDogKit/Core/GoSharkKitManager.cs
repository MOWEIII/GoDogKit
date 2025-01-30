#if TOOLS
using System.Collections.Generic;
using Godot;

namespace GoDogKit;

[Tool]
public sealed partial class GoDogKitManager : EditorPlugin
{
	// const string GoSharkPluginName = "GoShark";
	public const string GoDogKitFolderPath = "res://addons/GoSharkKit/";
	public const string GoDogKitIconFolderPath = "res://addons/GoSharkKit/Icons/";

	private static readonly List<string> GoDogNodes = [];
	private static readonly List<string> GoDogAutoloads = [];
	// public static readonly ExportManager ExportManager = new();

	private void AddNode(string nodeName, string parentNodeName, string scriptName, string IconName)
	{
		var script = ResourceLoader.Load<Script>(GoDogKitFolderPath + scriptName);
		var icon = ResourceLoader.Load<Texture2D>(GoDogKitIconFolderPath + IconName);

		AddCustomType(nodeName, parentNodeName, script, icon);

		GoDogNodes.Add(nodeName);
	}

	private void AddAutoload(string Name, string scriptName)
	{
		AddAutoloadSingleton(Name, GoDogKitFolderPath + scriptName);
		GoDogAutoloads.Add(Name);
	}

	private void ClearNodes()
	{
		foreach (var nodeName in GoDogNodes)
		{
			RemoveCustomType(nodeName);
		}

		GoDogNodes.Clear();
	}

	private void ClearAutoloads()
	{
		foreach (var autoloadName in GoDogAutoloads)
		{
			RemoveAutoloadSingleton(autoloadName);
		}
	}

	public override void _EnablePlugin()
	{

	}

	public override void _DisablePlugin()
	{

	}
}

#endif