

using David6.ShooterCore.Provider;
using UnityEngine;

namespace David6.ShooterCore.Animation
{
    /// <summary>
    /// 플레이어 애니메이터 컨트롤러
    /// </summary>
    public class DAnimatorController : IDAnimatorProvider
    {
        Animator _animator;
        int _upperBodyLayer = 1;

        public DAnimatorController(Animator animator) { _animator = animator; }

        public Animator PlayerAnimator => _animator;

        public void SetSpeed(float speed)
        {
            _animator.SetFloat("Speed", speed);
        }

        public void SetJump(bool isJumping)
        {
            _animator.SetBool("Jump", isJumping);
        }

        public void SetGrounded(bool isGrounded)
        {
            _animator.SetBool("Grounded", isGrounded);
        }
        public void SetFreeFall(bool isFreeFall)
        {
            _animator.SetBool("FreeFall", isFreeFall);
        }

        public void SetDirection(Vector2 direction)
        {
            _animator.SetFloat("DirectionX", direction.x);
            _animator.SetFloat("DirectionY", direction.y);
        }
        public void SetFocus(bool isFocus)
        {
            _animator.SetBool("Focus", isFocus);
        }
        public void SetFire()
        {
            _animator.CrossFade("Fire", 0.0f, _upperBodyLayer);
        }
        public void SetReload()
        {
            _animator.SetTrigger("Reload");
        }
        public void SetAnimationLayerWeight(int index, float weight)
        {
            _animator.SetLayerWeight(index, weight);
        }
    }
}