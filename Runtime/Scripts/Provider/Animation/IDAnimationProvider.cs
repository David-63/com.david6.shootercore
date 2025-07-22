


using UnityEngine;

namespace David6.ShooterCore.Provider
{
    /// <summary>
    /// 애니메이터 제공자 인터페이스
    /// </summary>
    public interface IDAnimatorProvider
    {
        void SetSpeed(float speed);
        void SetJump(bool isJumping);
        void SetGrounded(bool isGrounded);
        void SetFreeFall(bool isFreeFall);
        void SetDirection(Vector2 direction);
        void SetFocus(bool isFocus);

        void SetFire();
        void SetFireRate(float rpm);
        void SetReload();
        void SetAnimationLayerWeight(int index, float weight);
    }
}