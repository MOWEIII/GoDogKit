using System.Collections;

namespace GoDogKit
{
    /// <summary>
    /// Represents a coroutine, used a IEnumerator to represent the coroutine's logic.    
    /// </summary>
    public class Coroutine
    {
        private readonly IEnumerator enumerator;
        private bool m_processable;
        private bool m_isDone;

        public Coroutine(IEnumerator enumerator, bool autoStart = true)
        {
            this.enumerator = enumerator;
            m_processable = autoStart;
            m_isDone = false;
        }

        public Coroutine(IEnumerable enumerable, bool autoStart = true)
        {
            enumerator = enumerable.GetEnumerator();
            m_processable = autoStart;
            m_isDone = false;
        }

        /// <summary>
        /// Make coroutine processable.
        /// </summary>
        public virtual void Start()
        {
            m_processable = true;
        }

        /// <summary>
        /// What process do is actually enumerates all "yield return" in a "enumerator function" which 
        /// constructs this coroutine. So you can put this function in any logics loop with
        /// a delta time parameter to update the coroutine's state in your preferred ways.
        /// </summary>
        /// <param name="delta"> Coroutine process depends on the delta time. </param>
        public virtual void Process(double delta)
        {
            // If coroutine is not started or already done, do nothing
            if (!m_processable || m_isDone) return;

            // If coroutine process encounted a CoroutineTask, process it
            if (enumerator.Current is Coroutine coroutine)
            {
                coroutine.Process(delta);

                if (coroutine.IsDone())
                {
                    enumerator.MoveNext();
                }

                // Return if current task haven't done
                return;
            }

            // If there are no CoroutineTasks, just move to the next yield of the coroutine's enumerator
            if (!enumerator.MoveNext())
            {
                m_processable = false;
                m_isDone = true;
            }
        }

        /// <summary>
        /// Pause the coroutine, also means it's not processable any more.
        /// </summary>
        public virtual void Pause() => m_processable = false;

        /// <summary>
        /// Reset the coroutine, also means it's not done and not processable.
        /// </summary>
        public virtual void Reset()
        {
            //TODO: iterator methods seems do not support Reset(), need to implement it manually            
            // enumerator.Reset();
            m_isDone = false;
            m_processable = false;
        }

        /// <summary>
        /// Stop the coroutine, also means it's done.
        /// </summary>
        public virtual void Stop() => m_isDone = true;

        /// <summary>
        /// Get the enumerator which is used to construct this coroutine.
        /// </summary>
        /// <returns> The enumerator of this coroutine. </returns>
        public virtual IEnumerator GetEnumerator() => enumerator;

        /// <summary>
        /// Check if coroutine is done.
        /// </summary>
        /// <returns> True if coroutine is done, otherwise false. </returns>
        public virtual bool IsDone() => m_isDone;
    }

    /// <summary>
    /// Used for stun a coroutine in a certain duration.
    /// </summary>
    public class WaitForSeconds : Coroutine
    {
        private readonly double duration;
        private double currentTime;
        public WaitForSeconds(double duration) : base(enumerator: null, autoStart: true)
        {
            this.duration = duration;
            currentTime = 0;
        }
        public override void Process(double delta) => currentTime += delta;
        public override bool IsDone() => currentTime >= duration;
    }
}
