using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GoDogKit;

/// <summary>
/// Represents a coroutine, used a IEnumerator to represent the coroutine's logic.
/// </summary>
public class Coroutine
{
    private readonly IEnumerator m_enumerator;
    private bool m_isRunning;
    private bool m_isDone;

    public Coroutine(IEnumerator enumerator, bool autoStart = true)
    {
        m_enumerator = enumerator;
        m_isRunning = autoStart;
        m_isDone = false;
    }

    public Coroutine(IEnumerable enumerable, bool autoStart = true)
    {
        m_enumerator = enumerable.GetEnumerator();
        m_isRunning = autoStart;
        m_isDone = false;
    }

    /// <summary>
    /// Make coroutine processable.
    /// </summary>
    public virtual void Start()
    {
        m_isRunning = !m_isDone;
    }

    /// <summary>
    /// What process do is actually enumerates all "yield return" in a "enumerator function" which 
    /// constructs this coroutine. So you can put this function in any logics loop with
    /// a delta time parameter to update the coroutine's state in your preferred ways.
    /// </summary>
    /// <param name="delta"> Coroutine process depends on the delta time. </param>
    public virtual void Process(double delta)
    {
        void MoveNextAndCheck()
        {
            if (!m_enumerator.MoveNext())
            {
                m_isRunning = false;
                m_isDone = true;
                return;
            }
        }

        if (!m_isRunning || m_isDone) return;

        if (m_enumerator.Current is Coroutine coroutine)
        {
            if (coroutine.IsDone())
            {
                MoveNextAndCheck();
            }

            coroutine.Process(delta);
            return;
        }
        else
        {
            MoveNextAndCheck();
        }
        // // If coroutine is not started or already done, do nothing
        // if (!m_processable || m_isDone) return;

        // // If coroutine process encounted a CoroutineTask, process it
        // if (enumerator.Current is Coroutine coroutine)
        // {
        //     coroutine.Process(delta);

        //     if (coroutine.IsDone())
        //     {
        //         enumerator.MoveNext();
        //     }

        //     // Return if current task haven't done
        //     return;
        // }

        // // If there are no nested Coroutine, just move to the next yield of the coroutine's enumerator
        // if (!enumerator.MoveNext())
        // {
        //     m_processable = false;
        //     m_isDone = true;
        // }

    }

    /// <summary>
    /// Pause the coroutine, also means it's not processable any more.
    /// </summary>
    public virtual void Pause() => m_isRunning = false;

    // Using a new coroutine once a time, need no implement Reset method.
    // /// <summary>
    // /// Reset the coroutine, also means it's not done and not processable.
    // /// </summary>
    // public virtual void Reset()
    // {
    //     //TODO: iterator methods seems do not support Reset(), need to implement it manually            
    //     // enumerator.Reset();
    //     m_isDone = false;
    //     m_processable = false;
    // }

    /// <summary>
    /// Stop the coroutine, also means it's done.
    /// </summary>
    public virtual void Stop() => m_isDone = true;

    /// <summary>
    /// Get the enumerator which is used to construct this coroutine.
    /// </summary>
    /// <returns> The enumerator of this coroutine. </returns>
    public virtual IEnumerator GetEnumerator() => m_enumerator;

    /// <summary>
    /// Check if coroutine is running.
    /// </summary>
    /// <returns> True if coroutine is running, otherwise false. </returns>
    public virtual bool IsRunning() => m_isRunning;

    /// <summary>
    /// Check if coroutine is done.
    /// </summary>
    /// <returns> True if coroutine is done, otherwise false. </returns>
    public virtual bool IsDone() => m_isDone;
}

/// <summary>
/// Used for stun a coroutine in a certain duration.
/// </summary>
public class WaitForSeconds(double duration) : Coroutine(enumerator: null, autoStart: true)
{
    private double m_currentTime = 0;
    public override void Process(double delta) => m_currentTime += delta;
    public override bool IsDone() => m_currentTime >= duration;
}

/// <summary>
/// A wrapper for managing coroutines as a container.
/// It can only be used in code, not added to a node.
/// </summary>
public sealed class CoroutineLauncher
{
    /// <summary>
    /// Info struct that clarify the coroutine launcher's status.
    /// Used for debug porposes.
    /// </summary>
    public readonly struct CoroutineLaunchInfo(CoroutineLauncher launcher)
    {
        /// <summary>
        /// The number of coroutines still inside.
        /// </summary>
        public readonly int Totals = launcher.m_coroutines.Count;
        /// <summary>
        /// The number of coroutines still running.
        /// </summary>
        public readonly int Runnings = launcher.m_coroutines.Where(cor => cor.IsRunning()).Count();
        /// <summary>
        /// The number of coroutines done and still inside.
        /// </summary>
        public readonly int Dones = launcher.m_coroutines.Where(cor => cor.IsDone()).Count();
        /// <summary>
        /// The number of coroutines done and clean(removed).
        /// </summary>
        public readonly int Kills = launcher.m_kills;

        public override string ToString()
        {
            return "Coroutine Launch Info: " +
            $"Total: {Totals}, Running: {Runnings}, Done: {Dones}, Kill: {Kills}";
        }
    }

    // Maintains every coroutine indivisually.
    public bool AutoClean { get; set; } = true;
    // Indicates how many coroutines are done by this launcher.
    private int m_kills = 0;
    private readonly HashSet<Coroutine> m_coroutines = [];
    public bool Add(Coroutine coroutine) => m_coroutines.Add(coroutine);
    public bool Remove(Coroutine coroutine) => m_coroutines.Remove(coroutine);
    public CoroutineLaunchInfo GetInfo() => new(this);

    /// <summary>
    /// Clean the coroutines which are done.
    /// </summary>
    /// <param name="all"> Clean all coroutines, no matter whether done or not. </param>
    public void CleanDones(bool all = false)
    {
        if (all)
        {
            m_coroutines.Clear();
        }
        else
        {
            foreach (Coroutine coroutine in m_coroutines.Where(cor => cor.IsDone()))
            {
                m_coroutines.Remove(coroutine);
                m_kills++;
            }
        }
    }

    public Coroutine StartCoroutine(Coroutine coroutine)
    {
        if (!m_coroutines.Contains(coroutine))
        {
            Add(coroutine);
        }

        coroutine.Start();

        return coroutine;
    }

    public Coroutine StartCoroutine(IEnumerator enumerator)
    => StartCoroutine(new Coroutine(enumerator));

    public void StopCoroutine(Coroutine coroutine)
    {
        if (m_coroutines.TryGetValue(coroutine, out Coroutine actual))
        {
            actual.Stop();
        }
    }

    /// <summary>
    /// Process all the coroutine inside.
    /// </summary>
    public void Process(double delta)
    {
        if (AutoClean) CleanDones();

        foreach (var coroutine in m_coroutines)
        {
            coroutine.Process(delta);
        }
    }

    public void StartAllCoroutines()
    {
        foreach (Coroutine coroutine in m_coroutines)
        {
            coroutine.Start();
        }
    }

    public void StopAllCoroutines()
    {
        foreach (Coroutine coroutine in m_coroutines)
        {
            coroutine.Stop();
        }
    }
}