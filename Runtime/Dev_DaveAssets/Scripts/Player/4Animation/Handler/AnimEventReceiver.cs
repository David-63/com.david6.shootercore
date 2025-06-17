using UnityEngine;

namespace Dave6.ShooterFramework.Animation
{
    public class AnimEventReceiver : MonoBehaviour
    {
        private DaveAnimatorHandler _animScript;

        private void Start()
        {
            _animScript = GetComponentInParent<DaveAnimatorHandler>();
        }

        public void OnFootstep(AnimationEvent animationEvent)
        {
            _animScript?.OnFootstep(animationEvent);
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            _animScript?.OnLand(animationEvent);
        }
    }
}
