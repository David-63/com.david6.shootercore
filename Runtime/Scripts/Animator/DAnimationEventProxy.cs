using System;
using UnityEngine;

namespace David6.ShooterCore.Animation
{
    public class DAnimationEventProxy : MonoBehaviour
    {
        public event Action<AnimationEvent> OnFootstepEvent;
        public event Action<AnimationEvent> OnLandEvent;
        public event Action<AnimationEvent> OnEjectMagazineEvent;
        public event Action<AnimationEvent> OnInsertMagazineEvent;
        public event Action<AnimationEvent> OnChamberLoadEvent;

        public void OnFootstep(AnimationEvent animationEvent)
        {
            OnFootstepEvent?.Invoke(animationEvent);
        }
        public void OnLand(AnimationEvent animationEvent)
        {
            OnLandEvent?.Invoke(animationEvent);
        }
        public void OnEjectMagazine(AnimationEvent animationEvent)
        {
            OnEjectMagazineEvent?.Invoke(animationEvent);
        }
        public void OnInsertMagazine(AnimationEvent animationEvent)
        {
            OnInsertMagazineEvent?.Invoke(animationEvent);
        }
        public void OnChamberLoad(AnimationEvent animationEvent)
        {
            OnChamberLoadEvent?.Invoke(animationEvent);
        }
        
    }
}