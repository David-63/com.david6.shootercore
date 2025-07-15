using System;
using UnityEngine;

namespace David6.ShooterCore.Animation
{
    public class DAnimationEventProxy : MonoBehaviour
    {
        public event Action<AnimationEvent> OnFootstepEvent;
        public event Action<AnimationEvent> OnLandEvent;

        public void OnFootstep(AnimationEvent animationEvent)
        {
            OnFootstepEvent?.Invoke(animationEvent);
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            OnLandEvent?.Invoke(animationEvent);
        }
    }
}