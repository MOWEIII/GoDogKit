using System.Collections;
using Godot;

namespace GoDogKit
{
    public static class Extensions
    {
        #region Math

        public static Vector2 RandomShpere(float radius)
        {
            var biasX = (float)GD.RandRange(-radius, radius);
            var biasY = (float)GD.RandRange(-radius, radius);
            return new Vector2(biasX, biasY);
        }

        #endregion

        #region CanvasItem

        public static void Enable(this CanvasItem item)
        {
            item.Show();
            item.ProcessMode = Node.ProcessModeEnum.Inherit;
        }

        public static void Disable(this CanvasItem item)
        {
            item.Hide();
            item.ProcessMode = Node.ProcessModeEnum.Disabled;
        }

        #endregion

        #region Coroutine

        public static void StartCoroutine(this Node node, Coroutine coroutine, CoroutineProcessMode mode = CoroutineProcessMode.Physics)
        {
            coroutine.Start();
            GlobalCoroutineLauncher.AddCoroutine(coroutine, mode);
        }

        public static void StartCoroutine(this Node node, IEnumerator enumerator, CoroutineProcessMode mode = CoroutineProcessMode.Physics)
        {
            StartCoroutine(node, new Coroutine(enumerator), mode);
        }

        public static void StartCoroutine(this Node node, IEnumerable enumerable, CoroutineProcessMode mode = CoroutineProcessMode.Physics)
        {
            StartCoroutine(node, enumerable.GetEnumerator(), mode);
        }

        public static void StopCoroutine(this Node node, IEnumerator enumerator)
        {
            GlobalCoroutineLauncher.RemoveCoroutine(enumerator);
        }

        public static void StopCoroutine(this Node node, Coroutine coroutine)
        {
            StopCoroutine(node, coroutine.GetEnumerator());
        }

        public static void StopCoroutine(this Node node, IEnumerable enumerable)
        {
            StopCoroutine(node, enumerable.GetEnumerator());
        }

        #endregion

        #region Input

        // /// <summary>
        // /// Get the current input mode. Same as GlobalInputManager.CurrentInputMode.
        // /// </summary>        
        // /// <returns> The current input mode. </returns>
        // public static InputMode GetInputMode(this Node node)
        // {
        //     return GlobalInputManager.CurrentInputMode;
        // }

        #endregion

        #region ObjectPool

        public static void Register(this PackedScene scene)
        {
            GlobalObjectPool.Register(scene);
        }

        public static void Unregister(this PackedScene scene)
        {
            GlobalObjectPool.Unregister(scene);
        }

        public static Node Get(this PackedScene scene)
        {
            return GlobalObjectPool.Get(scene);
        }

        public static T Get<T>(this PackedScene scene) where T : Node
        {
            return GlobalObjectPool.Get<T>(scene);
        }

        public static void Release(this PackedScene scene, Node node)
        {
            GlobalObjectPool.Release(scene, node);
        }

        public static ObjectPool GetPool(this PackedScene scene)
        {
            return GlobalObjectPool.GetPool(scene);
        }

        #endregion

        #region Audio

        public static void Register(this AudioStream stream, string bus = "Master")
        {
            GlobalAudioManager.Register(stream, bus);
        }

        public static void Unregister(this AudioStream stream)
        {
            GlobalAudioManager.Unregister(stream);
        }

        public static void Play(this AudioStream stream)
        {
            GlobalAudioManager.Play(stream);
        }

        public static void Stop(this AudioStream stream)
        {
            GlobalAudioManager.Stop(stream);
        }

        public static AudioStreamPlayer GetPlayer(this AudioStream stream)
        {
            return GlobalAudioManager.GetPlayer(stream);
        }

        #endregion
    }
}

