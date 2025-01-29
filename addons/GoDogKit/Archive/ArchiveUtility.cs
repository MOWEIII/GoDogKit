using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using FileAccess = Godot.FileAccess;

namespace GoDogKit;

public static class ArchiveUtility
{
    public static string ExeDirectory { get => Path.GetDirectoryName(OS.GetExecutablePath()); }

    /// <summary>
    /// Create directories if the given do not exists.
    /// </summary>
    public static string EnsureDirectoryExists(string path)
    {
        string directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        return path;
    }

    /// <summary>
    /// Create a empty file if the given do not exists.
    /// </summary>
    public static string EnsureFileExists(string path)
    {
        if (!File.Exists(path))
            // Must remember close the file !!!
            File.Create(path).Close();

        return path;
    }

    /// <summary>
    /// Make sure the given path always exists.
    /// </summary>
    public static string EnsurePathExists(string path)
    {
        EnsureDirectoryExists(path);
        EnsureFileExists(path);
        return path;
    }

    public static List<string[]> CsvReadLines(string path, bool ignoreFirstLine = true)
    {
        var list = new List<string[]>();

        var csv = FileAccess.Open(path, FileAccess.ModeFlags.Read);

        string[] line;

        if (ignoreFirstLine) csv.GetCsvLine();

        while ((line = csv.GetCsvLine()) is not null)
        {
            list.Add(line);
        }

        return list;
    }

    public static bool CsvProcessLines(List<string[]> data, Action<string[]> callback)
    {
        if (data == null || data.Count == 0)
        {
            GD.PushError("CSV file is empty or not found.");
            return false;
        }

        foreach (var line in data) callback(line);

        return true;
    }

    public static bool CsvReadAndProcessLines
    (string path, Action<string[]> callback, bool ignoreFirstLine = true)
    => CsvProcessLines(CsvReadLines(path, ignoreFirstLine), callback);

    // /// <summary>
    // /// The path to the user's My Documents folder.
    // /// </summary>
    // public readonly static string MyDocumentsFolderPath;

    // /// <summary>
    // /// The path to the Streaming Assets folder.
    // /// </summary>
    // public readonly static string StreamingFolderPath;

    // static ArchiveUtility()
    // {
    //     MyDocumentsFolderPath
    //     = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

    //     if (OS.HasFeature("editor"))
    //     {
    //         StreamingFolderPath = "Streaming";
    //     }
    //     else
    //     {
    //         StreamingFolderPath = OS.GetExecutablePath().GetBaseDir();
    //     }
    // }

    // public static Task<string> LoadStreaming(string path)
    // => File.ReadAllTextAsync(Path.Combine(StreamingFolderPath, path));

    // /// <summary>
    // /// Load an image file from the Streaming folder.
    // /// </summary>
    // /// <param name="path"> The path to the file, relative to the Streaming folder. </param>
    // /// <returns> The loaded image as an Godot Image object. </returns>
    // public static Image LoadImageStreaming(string path)
    // {
    //     string file = Path.Combine(StreamingFolderPath, path);

    //     if (!Path.Exists(file))
    //         throw new System.Exception("Image file not found: " + path);

    //     var image = new Image();
    //     image.Load(Path.Combine(StreamingFolderPath, path));
    //     return image;
    // }

    // /// <summary>
    // /// Load an audio file from the Streaming folder.
    // /// </summary>
    // /// <param name="path"> The path to the file, relative to the Streaming folder. </param>
    // /// <returns> The loaded audio as an Godot AudioStream object. </returns>
    // public static AudioStream LoadAudioStreaming(string path)
    // {
    //     string file = Path.Combine(StreamingFolderPath, path);

    //     if (!Path.Exists(file))
    //         throw new System.Exception("Audio file not found: " + path);

    //     var extension = Path.GetExtension(path).ToLower() ?? throw new System.Exception("File extension needed : " + path);

    //     return extension switch
    //     {
    //         ".mp3" => new AudioStreamMP3() { Data = File.ReadAllBytes(file) },

    //         ".wav" => new AudioStreamWav() { Data = File.ReadAllBytes(file) },

    //         ".ogg" => AudioStreamOggVorbis.LoadFromFile(file),

    //         _ => throw new System.Exception("Unsupported audio format: " + Path.GetExtension(path)),
    //     };
    // }

    // public static T LoadAudioStreaming<T>(string path) where T : AudioStream
    // {
    //     var audioStream = LoadAudioStreaming(path);
    //     if (audioStream is T) return audioStream as T;
    //     else throw new System.Exception("Audio stream type mismatch: expected " + typeof(T).Name);
    // }

    // public static VideoStreamTheora LoadVideoStreaming(string path)
    // {
    //     // Seems like direct VideoStream is not supported,
    //     // must use VideoStreamTheora instead.
    //     // Maybe it's because Only VideoStreamTheora can be decoded currently.
    //     return new VideoStreamTheora
    //     {
    //         File = Path.Combine(StreamingFolderPath, path)
    //     };
    // }

    // public static PackedScene LoadSceneStreaming(string path)
    // {
    //     return ResourceLoader.Load<PackedScene>(Path.Combine(StreamingFolderPath, path));
    // }
}