

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
            _animator.Play("Fire", _upperBodyLayer, 0.0f);
        }
        public void SetFireRate(float rpm)
        {
            float rps = rpm / 60f;
            float targetPeriod = 1f / rps;
            float originalClipLength = 0.15f;
            float fireRate = originalClipLength / targetPeriod;
            _animator.SetFloat("FireRate", fireRate);
        }
        public void SetReload()
        {
            _animator.SetTrigger("Reload");
        }
        public void SetAnimationLayerWeight(int index, float weight)
        {
            _animator.SetLayerWeight(index, weight);
        }


        /*

        Fire 속도 관련해서
        Fire 애니메이션의 클립 레퍼런스를 가져와서 rpm 계산하는게 더 좋음

        */
    }
}