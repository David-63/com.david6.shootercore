using Dave6.ShooterFramework.Data;
using Dave6.ShooterFramework.Provider;
using UnityEngine;

namespace Dave6.ShooterFramework.Animation
{
    public class DaveAnimatorHandler : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AudioClip LandingAudioClip;
        [SerializeField] private AudioClip[] FootstepAudioClips;
        [Range(0, 1)]
        [SerializeField] private float FootstepAudioVolume = 0.5f;

        private CharacterController _characterController;

        private int _animIDMotionSpeed;
        private int _animIDSpeed;
        private int _animIDJump;
        private int _animIDGrounded;
        private int _animIDFreeFall;
        private int _animIDDirectionX;
        private int _animIDDirectionY;

        private void Start()
        {
            AssignAnimationIDs();
        }

        private void AssignAnimationIDs()
        {
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDDirectionX = Animator.StringToHash("DirectionX");
            _animIDDirectionY = Animator.StringToHash("DirectionY");
        }

        public void SetCharacterController(ICharacterControllerProvider provider)
        {
            _characterController = provider.GetController();
        }


        #region Movement Action Bind
        public void SetSpeed(AnimSpeedData speed)
        {
            _animator.SetFloat(_animIDSpeed, speed.MoveSpeed);
            _animator.SetFloat(_animIDMotionSpeed, speed.MotionSpeed);
        }

        public void SetGroundData(AnimGroundData ground)
        {
            _animator.SetBool(_animIDJump, ground.IsJump);
            _animator.SetBool(_animIDGrounded, ground.IsGrounded);
            _animator.SetBool(_animIDFreeFall, ground.IsFreeFall);
        }
        public void SetDirection(AnimInputDir dir)
        {
            _animator.SetFloat(_animIDDirectionX, dir.DirectionX);
            _animator.SetFloat(_animIDDirectionY, dir.DirectionY);
        }
        #endregion


        #region Animation Callback
        public void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_characterController.center), FootstepAudioVolume);
                }
            }
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_characterController.center), FootstepAudioVolume);
            }
        }
        #endregion
    }
}
