using UnityEngine;

namespace David6.ShooterFramework
{
    public class DavidAnimationEventReceiver : MonoBehaviour
    {

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        private CharacterController Controller;
        
        private void Start()
        {
            Controller = GetComponentInParent<CharacterController>();
        }
        private void OnFootstep(AnimationEvent animationEvent)
        {

            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {                    
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(Controller.center), FootstepAudioVolume);
            }
        }
    }
}
