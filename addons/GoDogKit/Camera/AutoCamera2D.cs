using System;
using System.Collections;
using Godot;

namespace GoDogKit
{
    /// <summary>
    /// A highly customizable camera that automatically follows a target.
    /// </summary>
    [Obsolete(@"This class is too useless, Recommend 'PhantomCamera' plugin instead.")]
    public partial class AutoCamera2D : Camera2D
    {
        /// <summary>
        /// The target to follow.
        /// </summary>
        [Export] public Node2D FollowTarget { get; set; } = null;
        /// <summary>
        /// Defines the maximum distance from the target to follow. Does not effect to the predict behaviour.
        /// </summary>
        [Export] public float FollowClamp { get; set; } = 1.0f;

        /// <summary>
        /// Defines the camera's behaviour when following the target.
        /// </summary>
        public enum BehaviourType
        {
            /// <summary>
            /// Follows the target normally. Results in global position copying.
            /// </summary>
            Normal,
            /// <summary>
            /// Smoothly follows the target in a given duration. Results in global position interpolation.
            /// </summary>          
            Inching,
            /// <summary>
            /// Follow the target with a constant speed. It can be faster or slower than the target's speed.
            /// If the follow speed equals or exceeds the target's speed, results just like the Normal behaviour.
            /// If the follow speed is slower than the target's speed, the camera will
            /// be clamped within a given distance from the target aka max distance.
            /// </summary>
            Slow,
            /// <summary>
            /// Follow the target with predictive behaviour.
            /// It predicts the target's movement based on its last position.
            /// And moves the camera towards the predicted position which 
            /// determined by predict distance with a constant speed.
            /// </summary>
            Predict
        }
        [Export] public BehaviourType Behaviour { get; set; } = BehaviourType.Normal;

        [ExportGroup("Inching Properties")]
        [Export]
        public float InchingDuration
        {
            get => m_inchingDuration;
            set => m_inchingDuration = value;
        }
        private float m_inchingDuration = 1.0f;
        private float m_inchingTimer = 0.0f;

        [ExportGroup("Slow Properties")]
        [Export] public float SlowFollowSpeed { get; set; } = 100.0f;
        [Export] public float SlowFollowMaxDistance { get; set; } = 100.0f;

        [ExportGroup("Predict Properties")]
        [Export] public float PredictFollowSpeed { get; set; } = 100.0f;
        [Export] public float PredictDistance { get; set; } = 100.0f;

        private Vector2 m_targetLastPos = Vector2.Zero;

        private Coroutine m_shakeCoroutine = null;

        public override void _Ready()
        {
            m_inchingTimer = m_inchingDuration;
            m_targetLastPos = Vector2.Zero;
        }

        private void NormalFollow(double delta)
        {
            GlobalPosition = FollowTarget.GlobalPosition;
        }

        private void InchingFollow(double delta)
        {
            float distance = GlobalPosition.DistanceTo(FollowTarget.GlobalPosition);

            // If the target is too close, stop inching.
            if (distance < FollowClamp)
            {
                m_inchingTimer = m_inchingDuration;
                return;
            }

            m_inchingTimer -= (float)delta;

            // If the inching timer has reached 0, reset it and start inching again.
            float rate = m_inchingTimer <= 0.0f ? 1.0f : 1.0f - m_inchingTimer / m_inchingDuration;

            var _x = Mathf.Lerp(GlobalPosition.X, FollowTarget.GlobalPosition.X, rate);
            var _y = Mathf.Lerp(GlobalPosition.Y, FollowTarget.GlobalPosition.Y, rate);

            GlobalPosition = new Vector2(_x, _y);
        }

        private void SlowFollow(double delta)
        {
            float distance = GlobalPosition.DistanceTo(FollowTarget.GlobalPosition);

            // If the target is too close, stop following.
            if (distance < FollowClamp)
            {
                return;
            }

            // If the target is too far, move it to max distance position.
            if (distance > SlowFollowMaxDistance)
            {
                Vector2 distanceVec = (FollowTarget.GlobalPosition - GlobalPosition).Normalized() * SlowFollowMaxDistance;
                GlobalPosition = FollowTarget.GlobalPosition - distanceVec;
                return;
            }

            var _x = Mathf.MoveToward(GlobalPosition.X, FollowTarget.GlobalPosition.X, (float)delta * SlowFollowSpeed);
            var _y = Mathf.MoveToward(GlobalPosition.Y, FollowTarget.GlobalPosition.Y, (float)delta * SlowFollowSpeed);

            GlobalPosition = new Vector2(_x, _y);
        }

        private void PredictFollow(double delta)
        {
            // Predict the direction of the target based on its last position.
            Vector2 predictedDir = (FollowTarget.GlobalPosition - m_targetLastPos).Normalized();

            Vector2 predictedPos = FollowTarget.GlobalPosition + predictedDir * PredictDistance;

            var _x = Mathf.MoveToward(GlobalPosition.X, predictedPos.X, (float)delta * PredictFollowSpeed);
            var _y = Mathf.MoveToward(GlobalPosition.Y, predictedPos.Y, (float)delta * PredictFollowSpeed);

            GlobalPosition = new Vector2(_x, _y);

            // Record the last position of the target for the next prediction.
            m_targetLastPos = FollowTarget.GlobalPosition;
        }

        // Used for constructing the shake coroutine.
        private IEnumerator ShakeCoroutine(float duration, float magnitude, float frequency)
        {
            float timer = 0.0f;

            while (timer < duration)
            {
                timer += frequency;
                GlobalPosition += MathUtility.RandomCircle(magnitude);
                yield return new WaitForSeconds(frequency);
            }

            yield return null;
        }

        public override void _PhysicsProcess(double delta)
        {
            // If there is no target, do nothing.
            if (FollowTarget == null) return;

            switch (Behaviour)
            {
                case BehaviourType.Normal: NormalFollow(delta); break;
                case BehaviourType.Inching: InchingFollow(delta); break;
                case BehaviourType.Slow: SlowFollow(delta); break;
                case BehaviourType.Predict: PredictFollow(delta); break;
            }

            // deal with shake coroutine
            m_shakeCoroutine?.Process(delta);
        }

        /// <summary>
        /// Shake the camera.
        /// </summary>
        /// <param name="duration"> The duration of the shake. </param>
        /// <param name="magnitude"> The magnitude of the shake. </param>
        /// <param name="frequency"> How often the camera should shake. </param>
        public void Shake(float duration, float magnitude, float frequency)
        {
            m_shakeCoroutine = new Coroutine(ShakeCoroutine(duration, magnitude, frequency));
        }
    }
}

