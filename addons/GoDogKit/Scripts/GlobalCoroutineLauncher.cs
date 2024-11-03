using System;
using System.Collections;
using System.Collections.Generic;

namespace GoDogKit
{
    /// <summary>
    /// In order to simplify coroutine management, 
    /// this class provides a global singleton that can be used to launch and manage coroutines.
    /// It will be autoloaded by GodogKit.
    /// </summary>
    public partial class GlobalCoroutineLauncher : Singleton<GlobalCoroutineLauncher>
    {
        // public static GlobalCoroutineLauncher Instance { get; private set; }
        private GlobalCoroutineLauncher() { }

        // public override void _EnterTree() => Instance = this;
        private readonly List<Coroutine> m_ProcessCoroutines = [];
        private readonly List<Coroutine> m_PhysicsProcessCoroutines = [];
        private readonly Dictionary<IEnumerator, List<Coroutine>> m_Coroutine2List = [];
        private readonly Queue<Action> m_DeferredRemoveQueue = [];

        public override void _Process(double delta)
        {
            ProcessCoroutines(m_ProcessCoroutines, delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            ProcessCoroutines(m_PhysicsProcessCoroutines, delta);
        }

        public static void AddCoroutine(Coroutine coroutine, CoroutineProcessMode mode)
        {
            switch (mode)
            {
                case CoroutineProcessMode.Idle:
                    Instance.m_ProcessCoroutines.Add(coroutine);
                    Instance.m_Coroutine2List.Add(coroutine.GetEnumerator(), Instance.m_ProcessCoroutines);
                    break;
                case CoroutineProcessMode.Physics:
                    Instance.m_PhysicsProcessCoroutines.Add(coroutine);
                    Instance.m_Coroutine2List.Add(coroutine.GetEnumerator(), Instance.m_PhysicsProcessCoroutines);
                    break;
            }
        }


        // It batter to use IEnumerator to identify the coroutine instead of Coroutine itself.
        public static void RemoveCoroutine(IEnumerator enumerator)
        {
            if (!Instance.m_Coroutine2List.TryGetValue(enumerator, out var coroutines)) return;

            int? index = null;

            for (int i = coroutines.Count - 1; i >= 0; i--)
            {
                if (coroutines[i].GetEnumerator() == enumerator)
                {
                    index = i;
                    break;
                }
            }

            if (index is not null)
            {
                Instance.m_DeferredRemoveQueue.Enqueue(() => coroutines.RemoveAt(index.Value));
            }
        }

        private static void ProcessCoroutines(List<Coroutine> coroutines, double delta)
        {
            foreach (var coroutine in coroutines)
            {
                coroutine.Process(delta);
            }

            // Remove action should not be called while procssing.
            // So we need to defer it until the end of the frame.
            ProcessDeferredRemoves();
        }

        private static void ProcessDeferredRemoves()
        {
            if (!Instance.m_DeferredRemoveQueue.TryDequeue(out var action)) return;

            action();
        }

        /// <summary>
        /// Do not use if unneccessary.
        /// </summary>
        public static void Clean()
        {
            Instance.m_ProcessCoroutines.Clear();
            Instance.m_PhysicsProcessCoroutines.Clear();
            Instance.m_Coroutine2List.Clear();
            Instance.m_DeferredRemoveQueue.Clear();
        }

        /// <summary>
        /// Get the current number of coroutines running, both in Idle and Physics process modes.
        /// </summary>
        /// <returns> The number of coroutines running. </returns>
        public static int GetCurrentCoroutineCount()
        => Instance.m_ProcessCoroutines.Count
        + Instance.m_PhysicsProcessCoroutines.Count;

    }
}

