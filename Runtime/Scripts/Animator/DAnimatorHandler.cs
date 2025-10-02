

using David6.ShooterCore.Provider;
using UnityEngine;

namespace David6.ShooterCore.Animation
{
    /// <summary>
    /// 플레이어 애니메이터 컨트롤러
    /// </summary>
    public class DAnimatorHandler : IDAnimatorHandlerProvider
    {
        IDContextProvider _context;
        Animator _animator;

        const int UPPERBODY_LAYER = 1;
        const int HAND_LAYER = 2;

        public DAnimatorHandler(IDContextProvider context, Animator animator) { _context = context; _animator = animator; }

        public Animator PlayerAnimator => _animator;

        #region 애니메이터 변수 세팅
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
        public void PlayFire()
        {
            _animator.Play("Fire", UPPERBODY_LAYER, 0.0f);
        }
        public void PlayReload()
        {
            _animator.SetTrigger("Reload");
        }
        #endregion

        public void FocusOn()
        {
            _animator.SetBool("Focus", true);
        }
        public void FocusOff()
        {
            _animator.SetBool("Focus", false);
        }

        public void SetFireRate(float rpm)
        {
            float rps = rpm / 60f;                                  // 초당 발사 횟수
            float targetPeriod = 1f / rps;                          // 발사 간격
            float originalClipLength = 0.15f;                       // 원본 클립 길이
            float fireRate = originalClipLength / targetPeriod;     // 배속 조절
            if (originalClipLength <= fireRate)
            {
                _animator.SetFloat("FireRate", 1);
            }
            else
            {
                _animator.SetFloat("FireRate", fireRate);
            }
        }

        // 이건 안씀
        public void SetAnimationLayerWeight(int index, float weight)
        {
            _animator.SetLayerWeight(index, weight);
        }

        public void ActiveUpperbodyLayer()
        {
            if (_context.CombatHandler.EquippedWeapon())
            {
                _animator.SetLayerWeight(UPPERBODY_LAYER, 1);
            }
        }
        public void InactiveUpperbodyLayer()
        {
            _animator.SetLayerWeight(UPPERBODY_LAYER, 0);
        }


        /*

        Fire 속도 관련해서
        Fire 애니메이션의 클립 레퍼런스를 가져와서 rpm 계산하는게 더 좋음
        */


        

    }
}