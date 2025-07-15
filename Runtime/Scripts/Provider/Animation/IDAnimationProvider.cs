


using UnityEngine;

namespace David6.ShooterCore.Provider
{
    /// <summary>
    /// 애니메이터 제공자 인터페이스
    /// </summary>
    public interface IDAnimatorProvider
    {
        // animation hash
        // animation hash2
        // animation crossfade
        void SetSpeed(float speed);
        void SetJump(bool isJumping);
        void SetGrounded(bool isGrounded);
        void SetFreeFall(bool isFreeFall);
        void SetDirection(Vector2 direction);
    }
}