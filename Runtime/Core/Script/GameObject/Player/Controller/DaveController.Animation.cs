using UnityEngine;
using David6.ShooterFramework.Extension;

namespace David6.ShooterFramework
{
    public partial class DaveController : MonoBehaviour
    {
        private Animator _animator;
        private bool _hasAnimator;

        private float _animationBlend;

		private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDXDirection;
        private int _animIDYDirection;

        private int _animIDEquip;
        private int _animIDEngage;
        private int _animIDAimDownSight;



        private void InitializeAnimator()
        {
            _hasAnimator = this.TryGetComponentInChildren<Animator>(out _animator);
            
            if (_hasAnimator)
            {
                _animIDSpeed = Animator.StringToHash("MoveSpeed");
                _animIDGrounded = Animator.StringToHash("Grounded");
                _animIDJump = Animator.StringToHash("Jump");
                _animIDFreeFall = Animator.StringToHash("FreeFall");
                _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
                _animIDXDirection = Animator.StringToHash("XDirection");
                _animIDYDirection = Animator.StringToHash("YDirection");
                _animIDEquip = Animator.StringToHash("Equip");
                _animIDEngage = Animator.StringToHash("Engage");
                _animIDAimDownSight = Animator.StringToHash("AimDownSight");
            }
            else
            {
                Log.AttentionPlease("애니메이터 없는데?");
            }
        }

        private void UpdateAnimSpeed(float motionSpeed)
        {
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
            }
        }

        private void UpdateAnimDirection(Vector2 direction)
        {
            if (!_hasAnimator) return;
            _animator.SetFloat(_animIDXDirection, direction.x);
            _animator.SetFloat(_animIDYDirection, direction.y);
        }
        private void UpdateAnimGrounded(bool grounded)
        {
            if (!_hasAnimator) return;
            _animator.SetBool(_animIDGrounded, grounded);
        }

        private void UpdateAnimJump(bool jump)
        {
            if (!_hasAnimator) return;
            _animator.SetBool(_animIDJump, jump);
        }
        private void UpdateAnimFreeFall(bool freeFall)
        {
            if (!_hasAnimator) return;
            _animator.SetBool(_animIDFreeFall, freeFall);
        }
        private void UpdateAnimEquip(bool equiped)
        {
            if (!_hasAnimator) return;
            _animator.SetBool(_animIDEquip, equiped);
        }
        private void UpdateAnimAimDownSight(bool ads)
        {
            if (!_hasAnimator) return;
            _animator.SetBool(_animIDAimDownSight, ads);
        }
    }
}
