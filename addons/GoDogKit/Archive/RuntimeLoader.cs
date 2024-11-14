using System.IO;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace GoDogKit
{
    public class LoadTask(string targetPath)
    {
        public string TargetPath { get; } = targetPath;

        /// <summary>
        /// Represents the progress of the load operation, ranges from 0 to 1.
        /// </summary>        
        public double Progress
        {
            get
            {
                Update();
                return (double)m_Progress[0];
            }
        }
        protected Array m_Progress = [];

        public ResourceLoader.ThreadLoadStatus Status
        {
            get
            {
                Update();
                return m_Status;
            }
        }
        private ResourceLoader.ThreadLoadStatus m_Status;

        public Resource Result
        {
            get
            {
                return ResourceLoader.LoadThreadedGet(TargetPath);
            }
        }

        public LoadTask Load(string typeHint = "", bool useSubThreads = false, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse)
        {
            ResourceLoader.LoadThreadedRequest(TargetPath, typeHint, useSubThreads, cacheMode);
            return this;
        }

        protected void Update()
        {
            m_Status = ResourceLoader.LoadThreadedGetStatus(TargetPath, m_Progress);
        }
    }

    public class LoadTask<T>(string targetPath) : LoadTask(targetPath) where T : Resource
    {
        public new T Result
        {
            get
            {
                return ResourceLoader.LoadThreadedGet(TargetPath) as T;
            }
        }
    }

    /// <summary>
    /// Provides some helper methods for loading resources in runtime.
    /// Most of them serve as async wrappers of the ResourceLoader class.
    /// </summary>
    public static class RuntimeLoader
    {
        public static string Root { get; private set; }

        static RuntimeLoader()
        {
            if (OS.HasFeature("editor"))
            {
                Root = "Streaming";
            }
            else
            {
                Root = OS.GetExecutablePath().GetBaseDir();
            }
        }
        /// <summary>
        /// Loads a resource from the given path asynchronously and returns a LoadTask object
        /// that can be used to track the progress and result of the load operation.
        /// </summary>        
        public static LoadTask Load(string path, string typeHint = "", bool useSubThreads = false, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse)
        {
            return new LoadTask(path).Load(typeHint, useSubThreads, cacheMode);
        }

        /// <summary>
        /// Loads a resource from the given path asynchronously and returns a LoadTask object
        /// that can be used to track the progress and result of the load operation.
        /// </summary>
        public static LoadTask<T> Load<T>(string path, string typeHint = "", bool useSubThreads = false, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse) where T : Resource
        {
            return new LoadTask<T>(path).Load(typeHint, useSubThreads, cacheMode) as LoadTask<T>;
        }

        public static Task<string> LoadStreaming(string path)
        {
            return File.ReadAllTextAsync(Path.Combine(Root, path));
        }
    }

}


