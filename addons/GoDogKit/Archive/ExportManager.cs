#if TOOLS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

namespace GoDogKit
{
    // [Obsolete("This exporter is deprecated due to inconvenience, Working on GUI Plugin instead...")]
    // public partial class ExportManager : EditorExportPlugin
    // {
    //     private static string ExportPath;
    //     public static readonly string StreamingFloderPath = ArchiveUtility.StreamingFolderPath;
    //     public static bool HasStreamingFloder
    //     {
    //         get => Directory.Exists(StreamingFloderPath);
    //     }
    //     public static readonly string[] StreamingIgnoreExtensions
    //      = [".import"];

    //     public override string _GetName()
    //     {
    //         return base._GetName();
    //     }

    //     public override void _ExportBegin(string[] features, bool isDebug, string path, uint flags)
    //     {
    //         // Current version export path is "Real Export Path//{Project Name}.tmp",
    //         // so we need to go up one level to get the actual export path.
    //         ExportPath = new DirectoryInfo(path).Parent.FullName;

    //         if (HasStreamingFloder)
    //         {
    //             // Record all the files' full-path in the Streaming folder and its subdirectories.
    //             var paths = new List<string>();

    //             RecordPaths(new DirectoryInfo(StreamingFloderPath), ref paths);

    //             ExportStreamingFiles(paths);
    //         }
    //     }

    //     override public void _ExportEnd()
    //     {
    //         PrintStreamingExportMessage();
    //     }

    //     /// <summary>
    //     /// Record all the files' full-path in a directory and its subdirectories.
    //     /// </summary>
    //     /// <param name="dir"> The directory to record the paths from. </param>
    //     /// <param name="paths"> The list to record the paths to. </param>
    //     private static void RecordPaths(DirectoryInfo dir, ref List<string> paths)
    //     {
    //         foreach (var file in dir.GetFiles())
    //         {
    //             paths.Add(file.FullName);
    //         }

    //         foreach (var subdir in dir.GetDirectories())
    //         {
    //             RecordPaths(subdir, ref paths);
    //         }
    //     }

    //     private static void ExportStreamingFiles(List<string> paths)
    //     {
    //         // Copy all the files in the Streaming folder and its subdirectories to the export path.
    //         foreach (var oldPath in paths)
    //         {
    //             // Ignore files with certain extensions.
    //             if (StreamingIgnoreExtensions.Any(ext => Path.GetExtension(oldPath) == ext))
    //                 continue;

    //             var newPath = ExportPath + oldPath.Split(StreamingFloderPath).Last();

    //             // GD.Print("Copying Streaming file: " + oldPath + " to " + newPath);
    //             Directory.CreateDirectory(Path.GetDirectoryName(newPath));

    //             File.Copy(oldPath, newPath, true);
    //         }
    //     }

    //     private static void PrintStreamingExportMessage()
    //     {
    //         GD.Print("Streaming Export Path: " + ExportPath);
    //     }
    // }

}
#endif