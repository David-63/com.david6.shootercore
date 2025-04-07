using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class PlayerController : ControllerBehaviour
    {
        #region Serialize fields

        public AudioClip LandingAudioClip;
		public AudioClip[] FootstepAudioClips;

		[Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        #endregion

        #region Fields

        private Animator _animator;
        private bool _hasAnimator;
        private float _animationBlend;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDHeadLookX;
		private int _animIDHeadLookY;
		private int _animIDGunFire;
		private int _animIDGunReload;

        #endregion

        private void InitializeAnimations()
        {
            _hasAnimator = TryGetComponent(out _animator);
            if (_hasAnimator)
            {
                Log.WhatHappend("애니메이션 정상적으로 등록됨");
            }

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            _animIDSpeed = Animator.StringToHash("Speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animIDFreeFall = Animator.StringToHash("FreeFall");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
			_animIDHeadLookX = Animator.StringToHash("HeadLookX");
			_animIDHeadLookY = Animator.StringToHash("HeadLookY");

			_animIDGunFire = Animator.StringToHash("Fire");
			_animIDGunReload = Animator.StringToHash("Reload");
        }

        private void OnFootstep(AnimationEvent animationEvent)
		{
			if (animationEvent.animatorClipInfo.weight > 0.5f)
			{
				if (FootstepAudioClips.Length > 0)
				{
					var index = Random.Range(0, FootstepAudioClips.Length);
					AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
				}
			}
		}

		private void OnLand(AnimationEvent animationEvent)
		{
			if (animationEvent.animatorClipInfo.weight > 0.5f)
			{
				AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
			}
		}
    }
}
