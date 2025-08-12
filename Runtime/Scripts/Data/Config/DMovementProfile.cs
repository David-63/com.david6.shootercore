using UnityEngine;


namespace David6.ShooterCore.Data
{
    /// <summary>
    /// Represents a movement profile for a player.
    /// </summary>
    [CreateAssetMenu(fileName = "MovementProfile", menuName = "Configs/Movement/MovementProfile")]
    public class DMovementProfile : ScriptableObject
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float WalkSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float RunSpeed = 6.0f;
        public float JumpBoostMultiplier = 1.4f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        [Tooltip("Deceleration In Air")]
        public float AirDeceleration = 2.0f;        // 천천히 감속되는걸 목표로
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float AirborneGravity = -15.0f;
        public float GroundGravity = -0.1f;

        public float TerminalVelocity = 53.0f; // 중력 가속이 적용되는 속도

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.01f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.01f;

        [Header("Player Grounded")]
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers = 1;
    }
}