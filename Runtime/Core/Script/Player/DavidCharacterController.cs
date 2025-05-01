using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

namespace David6.ShooterFramework
{
    public enum CharacterMode
    {
        Default,
        Charging,
        NoClip,
    }

    /// <summary>
    /// 인풋 데이터 전달 구조체
    /// </summary>
    public readonly struct PlayerCharacterInputs
    {
        public Vector2 MoveAxis { get; }
        public Quaternion CameraRotation { get; }
        public bool Sprint { get; }
        public bool Jump { get; }
        public bool JumpHeld { get; }

        public bool Crouch { get; }
        public bool CrouchHeld { get; }

        public bool Charge { get; }
        public bool NoClip { get; }

        public PlayerCharacterInputs(Vector2 move, Quaternion rotation, bool sprint, bool jump, bool jumpHeld, bool crouch, bool crouchHeld, bool charge, bool noClip)
        {
            MoveAxis = move; CameraRotation = rotation; Sprint = sprint;
            Jump = jump; JumpHeld = jumpHeld; Crouch = crouch;  CrouchHeld = crouchHeld;
            Charge = charge; NoClip = noClip;
        }
    }

    public partial class DavidCharacterController : MonoBehaviour, ICharacterController
    {
        #region Serialize fields

        [Header("필수")]
        public KinematicCharacterMotor Motor;

        [Header("Stable Movement")]
        [SerializeField]
        private float MaxStableWalkSpeed = 2f;
        [SerializeField]
        private float MaxStableSprintSpeed = 6f;
        [SerializeField]
        private float StableMovementSharpness = 15;
        [SerializeField]
        private float OrientationSharpness = 10;

        [Header("Air Movement")]
        [SerializeField]
        private float MaxAirMoveSpeed = 6f;
        [SerializeField]
        private float AirAccelerationSpeed = 5f;
        [SerializeField]
        private float Drag = 0.1f;

        [Header("Jumping")]
        [Tooltip("안정되지 않은 상태에서 점프 허용"), SerializeField]
        private bool AllowJumpingWhenSliding = false;
        [Tooltip("이단 점프 허용"), SerializeField]
        private bool AllowDoubleJump = false;
        [Tooltip("벽 점프 허용"), SerializeField]
        private bool AllowWallJump = false;
        [Tooltip("점프 속도"), SerializeField]
        private float JumpSpeed = 10f;
        [Tooltip("점프 여유시간"), SerializeField]
        private float JumpPreGroundingGraceTime = 0f;
        [Tooltip("점프 여유시간"), SerializeField]
        private float JumpPostGroundingGraceTime = 0f;
        
        [Header("Dash")]
        [Tooltip("대시 크기"), SerializeField]
        private float DashForce = 20f;

        [Header("Charging")]
        [Tooltip("돌진 속력"), SerializeField]
        private float ChargeSpeed = 15f;
        [Tooltip("돌진 최대 유지시간"), SerializeField]
        private float MaxChargeTime = 0.5f;
        [Tooltip("돌진 중단 시간"), SerializeField]
        private float StoppedTime = 0.05f;

        [Header("NoClip")]
        public float NoClipMoveSpeed = 10f;
        public float NoClipSharpness = 15;

        

        [Header("Miscellaneous")]
        [Tooltip("기울기 값 중력 방향 적용"), SerializeField]
        private bool OrientTowardsGravity = true;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        [Tooltip("캐릭터 메쉬"), SerializeField]
        private Transform MeshRoot;
        public CharacterMode CurrentCharacterMode { get; private set; }

        #endregion

        #region Fields

        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;

        /// <summary>
        /// 충돌 무시 변수
        /// </summary>
        private List<Collider> _ignoredColliders = new List<Collider>();
        private bool _sprinting = false;

        // 점프 기능
        private bool _jumpInputIsHeld = false;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private bool _doubleJumpConsumed = false;
        private bool _canWallJump = false;
        private Vector3 _wallJumpNormal;

        // 대시 기능(혹은 힘 가하기)
        private Vector3 _internalVelocityAdd = Vector3.zero;

        // 웅크리기 기능
        private bool _crouchInputIsHeld = false;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;
        private Collider[] _probedColliders = new Collider[8];

        // 돌진 기능
        private Vector3 _currentChargeVelocity;
        private bool _isStopped;
        private bool _mustStopVelocity = false;
        private float _timeSinceStartedCharge = 0;
        private float _timeSinceStopped = 0;

        #endregion

        private void Start()
        {
            // motor 등록
            Motor.CharacterController = this;

            // mode 등록
            InitializeCharacterMode();
            TransitionToMode(CharacterMode.Default);

            InitializeAnimator();
        }
        private void Update()
        {
            // TODO
            // 카메라 타겟 포지션 전달?
        }

        public void SetIgnoredColliders(List<Collider> colliders)
        {
            _ignoredColliders.Clear();
            _ignoredColliders = colliders;
        }

        

        #region 입력 제어

        /// <summary>
        /// 매 프레임마다 플레이어가 입력 내용을 캐릭터에 전달.
        /// </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            if (inputs.Charge)
            {
                if (CurrentCharacterMode != CharacterMode.Charging)
                {
                    
                    TransitionToMode(CharacterMode.Charging);
                }
            }
            else if (inputs.NoClip)
            {
                if (CurrentCharacterMode != CharacterMode.NoClip)
                {
                    TransitionToMode(CharacterMode.NoClip);
                }
            }

            if (_inputHandlers.TryGetValue(CurrentCharacterMode, out var handler))
            {
                handler.Invoke(inputs);
            }
        }

        #endregion

        #region 인터페이스 구현 함수

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
            // This is called before the motor does anything
            if (_beforeUpdateHandlers.TryGetValue(CurrentCharacterMode, out var handler))
            {
                handler.Invoke(deltaTime);
            }
        }
        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now. 
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            // This is called when the motor wants to know what its rotation should be right now
            if (_rotationHandlers.TryGetValue(CurrentCharacterMode, out var handler))
            {
                handler.Invoke(ref currentRotation, deltaTime);
            }
        }
        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now. 
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            // This is called when the motor wants to know what its velocity should be right now            
            if (_velocityHandlers.TryGetValue(CurrentCharacterMode, out var handler))
            {
                handler.Invoke(ref currentVelocity, deltaTime);
            }

        }
        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            // This is called after the motor has finished everything in its update
            if (_afterUpdateHandlers.TryGetValue(CurrentCharacterMode, out var handler))
            {
                handler.Invoke(deltaTime);
            }
        }
        public bool IsColliderValidForCollisions(Collider coll)
        {
            // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
            if (_ignoredColliders.Contains(coll))
            {
                return false;
            }
            return true;
        }
        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // This is called when the motor's ground probing detects a ground hit
        }
        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // This is called when the motor's movement logic detects a hit

            switch (CurrentCharacterMode)
            {
                case CharacterMode.Default:
                    // 지면에 있지 않고, 장애물에 부딧친 경우에만 벽 점프 가능.
                    if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                    {
                        _canWallJump = true;
                        _wallJumpNormal = hitNormal;
                    }
                break;
                case CharacterMode.Charging:
                    // 장애물에 의해 정지되는지 확인.
                    if (!_isStopped && !hitStabilityReport.IsStable && Vector3.Dot(-hitNormal, _currentChargeVelocity.normalized) > 0.5f)
                    {
                        _mustStopVelocity = true;
                        _isStopped = true;
                    }
                break;
            }

        }
        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
        }
        public void PostGroundingUpdate(float deltaTime)
        {
            // This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }
        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
        }

        #endregion

        #region 움직임 제어

        /// <summary>
        /// 점프 상태 및 속도 제어
        /// </summary>
        /// <param name="currentVelocity"></param>
        /// <param name="deltaTime"></param>
        protected void HandleJumpVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            // 이번 프레임에 점프가 발생했는지 확인하는 플래그.
            _jumpedThisFrame = false;
            // 점프 요청 이후 경과시간 누적. (점프 후에 약간의 여유를 주어, 바닥을 벗어난 후에도 점프가 가능하도록 하는데 사용될 수 있음).
            _timeSinceJumpRequested += deltaTime;
            if (_jumpRequested)
            {
                // 더블 점프 제어.
                if (AllowDoubleJump)
                {
                    // 점프를 이미 소비하고, 더블점프를 사용하지 않은 상태이며, 접지 상태를 확인.
                    if (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                    {
                        // 지면에 스냅되어 있는 상태를 푼다.
                        Motor.ForceUnground(0.1f);

                        // 기존 속도에서 수직 성분을 제거한 다음 점프속도를 추가하여 일정한 점프 힘 적용.
                        currentVelocity += (Motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                        // 점프 상태 업데이트
                        _jumpRequested = false;
                        _doubleJumpConsumed = true;
                        _jumpedThisFrame = true;

                        SetAnimJump(true);
                    }
                }

                // 일반 점프 및 벽 점프 체크 (접지 상태 체크 및 여유시간 보정).
                if (_canWallJump ||
                    (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)))
                {
                    // 점프 방향 설정.
                    Vector3 jumpDirection = Motor.CharacterUp;
                    // 점프 위력 설정.
                    float jumpForce = JumpSpeed;
                    // 점프 방향을 벽 노말로 변경.
                    if (_canWallJump)
                    {
                        jumpForce *= 1.2f;
                        jumpDirection = (_wallJumpNormal + (-Gravity.normalized * 1.5f)).normalized;
                        // 벽점프는 소모 안한 취급
                    }
                    // 경사면인 경우 지면의 노말로 변경.
                    else if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                    {
                        jumpDirection = (Motor.GroundingStatus.GroundNormal + -Gravity.normalized).normalized;
                    }

                    // 캐릭터가 지면에 고정되는 것을 방지.
                    Motor.ForceUnground(0.1f);

                    // 기존 속도의 수직 성분을 제거하고 계산된 점프 방향으로 힘을 적용.
                    currentVelocity += (jumpDirection * jumpForce * 1.2f) - Vector3.Project(currentVelocity, Motor.CharacterUp);

                    // 점프 상태 업데이트
                    _jumpRequested = false;
                    _jumpConsumed = true;
                    _jumpedThisFrame = true;

                    SetAnimJump(true);
                    
                }
            }

            // 벽점프 플래그를 초기화 하여 조건을 다시 평가.
            _canWallJump = false;
        }

        protected void OnLanded()
        {
            SetGrounded(true);
            SetAnimJump(false);
        }

        protected void OnLeaveStableGround()
        {
            SetGrounded(false);            
        }

        public void AddVelocity(Vector3 velocity)
        {
            _internalVelocityAdd += velocity;
        }

        public void Dash(Vector3 direction)
        {
            _internalVelocityAdd += direction * DashForce;
        }


        #endregion

    }
}
