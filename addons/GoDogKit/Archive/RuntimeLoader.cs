using System.IO;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace GoDogKit;

// public class LoadTask(string targetPath)
// {
//     public string TargetPath { get; } = targetPath;

//     /// <summary>
//     /// Represents the progress of the load operation, ranges from 0 to 1.
//     /// </summary>        
//     public double Progress
//     {
//         get
//         {
//             Update();
//             return (double)m_Progress[0];
//         }
//     }
//     protected Array m_Progress = [];

//     public ResourceLoader.ThreadLoadStatus Status
//     {
//         get
//         {
//             Update();
//             return m_Status;
//         }
//     }
//     private ResourceLoader.ThreadLoadStatus m_Status;

//     public Resource Result
//     {
//         get
//         {
//             return ResourceLoader.LoadThreadedGet(TargetPath);
//         }
//     }

//     public LoadTask Load(string typeHint = "", bool useSubThreads = false, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse)
//     {
//         ResourceLoader.LoadThreadedRequest(TargetPath, typeHint, useSubThreads, cacheMode);
//         return this;
//     }

//     protected void Update()
//     {
//         m_Status = ResourceLoader.LoadThreadedGetStatus(TargetPath, m_Progress);
//     }
// }

// public class LoadTask<T>(string targetPath) : LoadTask(targetPath) where T : Resource
// {
//     public new T Result
//     {
//         get
//         {
//             return ResourceLoader.LoadThreadedGet(TargetPath) as T;
//         }
//     }
// }

// /// <summary>
// /// Provides some helper methods for loading resources in runtime.
// /// Most of them serve as async wrappers of the ResourceLoader class.
// /// </summary>
// public static class RuntimeLoader
// {
//     /// <summary>
//     /// Loads a resource from the given path asynchronously and returns a LoadTask object
//     /// that can be used to track the progress and result of the load operation.
//     /// </summary>        
//     public static LoadTask Load(string path, string typeHint = "", bool useSubThreads = false, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse)
//     {
//         return new LoadTask(path).Load(typeHint, useSubThreads, cacheMode);
//     }

//     /// <summary>
//     /// Loads a resource from the given path asynchronously and returns a LoadTask object
//     /// that can be used to track the progress and result of the load operation.
//     /// </summary>
//     public static LoadTask<T> Load<T>(string path, string typeHint = "", bool useSubThreads = false, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse) where T : Resource
//     {
//         return new LoadTask<T>(path).Load(typeHint, useSubThreads, cacheMode) as LoadTask<T>;
//     }

//     /// <summary>
//     /// Load a text file from the Streaming folder asynchronously.
//     /// </summary>
//     /// <param name="path"> The path to the file, relative to the Streaming folder. </param>
//     /// <returns> A Task that can be awaited to get the contents of the file as a string. </returns> 
//     public static Task<string> LoadStreaming(string path)
//     {
//         return File.ReadAllTextAsync(Path.Combine(Archive.Streaming, path));
//     }

//     /// <summary>
//     /// Load an image file from the Streaming folder.
//     /// </summary>
//     /// <param name="path"> The path to the file, relative to the Streaming folder. </param>
//     /// <returns> The loaded image as an Godot Image object. </returns>
//     public static Image LoadImageStreaming(string path)
//     {
//         var image = new Image();
//         image.Load(Path.Combine(Archive.Streaming, path));
//         return image;
//     }

//     /// <summary>
//     /// Load an audio file from the Streaming folder.
//     /// </summary>
//     /// <param name="path"> The path to the file, relative to the Streaming folder. </param>
//     /// <returns> The loaded audio as an Godot AudioStream object. </returns>
//     public static AudioStream LoadAudioStreaming(string path)
//     {
//         if (!Path.Exists(Path.Combine(Archive.Streaming, path)))
//             throw new System.Exception("Audio file not found: " + path);

//         var extension = Path.GetExtension(path).ToLower() ?? throw new System.Exception("File extension needed : " + path);

//         return extension switch
//         {
//             ".mp3" => new AudioStreamMP3() { Data = File.ReadAllBytes(Path.Combine(Archive.Streaming, path)) },

//             ".wav" => new AudioStreamWav() { Data = File.ReadAllBytes(Path.Combine(Archive.Streaming, path)) },

//             ".ogg" => AudioStreamOggVorbis.LoadFromFile(Path.Combine(Archive.Streaming, path)),

//             _ => throw new System.Exception("Unsupported audio format: " + Path.GetExtension(path)),
//         };
//     }

//     public static T LoadAudioStreaming<T>(string path) where T : AudioStream
//     {
//         var audioStream = LoadAudioStreaming(path);
//         if (audioStream is T) return audioStream as T;
//         else throw new System.Exception("Audio stream type mismatch: expected " + typeof(T).Name);
//     }

//     /// <summary>
//     /// Load a video file from the Streaming folder.
//     /// </summary>
//     /// <param name="path"> The path to the file, relative to the Streaming folder. </param>
//     /// <returns> The loaded video as an Godot VideoStreamTheora object. </returns>
//     public static VideoStreamTheora LoadVideoStreaming(string path)
//     {
//         // Seems like VideoStream is not supported,
//         // must use VideoStreamTheora instead.
//         // Maybe Only VideoStreamTheora can be decoded currently.
//         return new VideoStreamTheora
//         {
//             File = Path.Combine(Archive.Streaming, path)
//         };
//     }
// }