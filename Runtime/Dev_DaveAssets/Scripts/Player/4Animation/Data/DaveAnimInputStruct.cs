using UnityEngine;

namespace Dave6.ShooterFramework.Data
{
    public struct AnimInputDir
    {
        public float DirectionX;
        public float DirectionY;

        public AnimInputDir(float directionX, float directionY)        
        {
            DirectionX = directionX;
            DirectionY = directionY;
        }
    }

    public struct AnimGroundData
    {
        public bool IsJump;
        public bool IsGrounded;
        public bool IsFreeFall;

        public AnimGroundData(bool isJump, bool isGrounded, bool isFreeFall)
        {
            IsJump = isJump;
            IsGrounded = isGrounded;
            IsFreeFall = isFreeFall;
        }
    }

    public struct AnimSpeedData
    {
        public float MotionSpeed;
        public float MoveSpeed;

        public AnimSpeedData(float motionSpeed, float moveSpeed)
        {
            MotionSpeed = motionSpeed;
            MoveSpeed = moveSpeed;
        }
    }
}