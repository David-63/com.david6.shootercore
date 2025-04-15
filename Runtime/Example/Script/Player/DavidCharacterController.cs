using System.Collections.Generic;
using KinematicCharacterController;
using Unity.VisualScripting;
using UnityEngine;

namespace David6.ShooterFramework
{
    public enum CharacterState
    {
        Default,
        Charging,
        NoClip,
    }
    public enum CharacterDirection
    {
        N,
        E,
        S,
        W,
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

    public class DavidCharacterController : MonoBehaviour, ICharacterController
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

        [Header("Animator")]
        [SerializeField]
        private Animator PlayerAnimator;

        [Header("Miscellaneous")]
        [Tooltip("기울기 값 중력 방향 적용"), SerializeField]
        private bool OrientTowardsGravity = true;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        [Tooltip("캐릭터 메쉬"), SerializeField]
        private Transform MeshRoot;
        public CharacterState CurrentCharacterState { get; private set; }

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

        // 애니메이션
        private bool _hasAnimator = false;
        private int _xVelocityHash;
        private int _yVelocityHash;
        public Vector2 AnimDirection;
        private int _xDirectionHash;
        private int _yDirectionHash;
        private int _moveSpeedHash;
        public float _animMoveSpeed;
        private CharacterDirection _animDirection;

        #endregion

        private void Start()
        {
            // motor 등록
            Motor.CharacterController = this;
            TransitionToState(CharacterState.Default);

            if (PlayerAnimator)
            {
                _hasAnimator = true;
            }
            else
            {
                Log.AttentionPlease("애니메이터가 연결 안됬음.");
            }

            if (_hasAnimator)
            {
                _xVelocityHash = Animator.StringToHash("x_Velocity");
                _yVelocityHash = Animator.StringToHash("y_Velocity");
                _xDirectionHash = Animator.StringToHash("x_Direction");
                _yDirectionHash = Animator.StringToHash("y_Direction");
                _moveSpeedHash = Animator.StringToHash("MoveSpeed");
            }
        }


        #region 상태 제어

        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(CharacterState newState)
        {
            CharacterState tempInitialState = CurrentCharacterState;
            OnStateExit(tempInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tempInitialState);
        }
        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                break;
                case CharacterState.Charging:
                        _currentChargeVelocity = Motor.CharacterForward * ChargeSpeed;
                        _isStopped = false;
                        _timeSinceStartedCharge = 0f;
                        _timeSinceStopped = 0f;
                break;
                case CharacterState.NoClip:
                        Motor.SetCapsuleCollisionsActivation(false);
                        Motor.SetMovementCollisionsSolvingActivation(false);
                        Motor.SetGroundSolvingActivation(false);
                break;
            }
        }
        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                break;
                case CharacterState.NoClip:
                    Motor.SetCapsuleCollisionsActivation(true);
                    Motor.SetMovementCollisionsSolvingActivation(true);
                    Motor.SetGroundSolvingActivation(true);
                break;
            }
        }

        #endregion

        /// <summary>
        /// 매 프레임마다 플레이어가 입력 내용을 캐릭터에 전달.
        /// </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            if (inputs.Charge)
            {
                TransitionToState(CharacterState.Charging);
            }
            if (inputs.NoClip)
            {
                if (CurrentCharacterState == CharacterState.Default)
                {
                    TransitionToState(CharacterState.NoClip);
                }
                else if (CurrentCharacterState == CharacterState.NoClip)
                {
                    TransitionToState(CharacterState.Default);
                }
            }

            _jumpInputIsHeld = inputs.JumpHeld;
            _crouchInputIsHeld = inputs.CrouchHeld;
            _sprinting = inputs.Sprint;

            // 입력 크기 제한.
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxis.x, 0f, inputs.MoveAxis.y), 1f);

            // 캐릭터의 수평에 맞춰 카메라 방향과 회전을 계산.
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    // 입력으로부터 이동과 시점 벡터를 세팅.
                    _moveInputVector = cameraPlanarRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;

                    // 점프 입력 설정.
                    if (inputs.Jump)
                    {
                        _timeSinceJumpRequested = 0f;
                        _jumpRequested = true;
                    }

                    // 웅크리기 입력 설정.
                    if (inputs.Crouch)
                    {
                        _shouldBeCrouching = true;

                        if (!_isCrouching)
                        {
                            _isCrouching = true;
                            // 충돌 사이즈 변경.
                            Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                            // 이건 임시적으로 모델 크기를 줄인것(애니메이션으로 웅크리면 굳이 줄일 필요가 없지).
                            MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                        }
                    }
                    else
                    {
                        _shouldBeCrouching = false;
                    }
                break;
                case CharacterState.NoClip:
                    _moveInputVector = inputs.CameraRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;
                break;
            }


            // 입력 방향을 애니메이터로 전달

            Vector3 currentDirection = new Vector3(AnimDirection.x, 0f, AnimDirection.y);
            
            Vector3 slerpedDirection = Vector3.Slerp(currentDirection, moveInputVector, OrientationSharpness * Time.deltaTime);

            AnimDirection = new Vector2(slerpedDirection.x, slerpedDirection.z);

            //Vector2 rawAnimDirection = new Vector2(moveInputVector.x, moveInputVector.z);
            //AnimDirection = Vector2.Lerp(AnimDirection, rawAnimDirection, OrientationSharpness * Time.deltaTime);

            PlayerAnimator.SetFloat(_xDirectionHash, AnimDirection.x);
            PlayerAnimator.SetFloat(_yDirectionHash, AnimDirection.y);
        }
        public void SetIgnoredColliders(List<Collider> colliders)
        {
            _ignoredColliders.Clear();
            _ignoredColliders = colliders;
        }

        #region 인터페이스 구현 함수

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
            // This is called before the motor does anything

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                break;
                case CharacterState.Charging:
                    // Update times
                    _timeSinceStartedCharge += deltaTime;
                    if (_isStopped)
                    {
                        _timeSinceStopped += deltaTime;
                    }
                break;
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

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    // 입력값이 있을때만 유효함.
                    if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
                    {
                        // 캐릭터의 전방 방향을 카메라 입력 방향으로 보간시킴.
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                        // 최종 회전을 결정함 (which will be used by the KinematicCharacterMotor).
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }

                    if (OrientTowardsGravity)
                    {
                        // 위쪽 방향벡터와 중력의 반대방향벡터로 방향을 바꾸는 회전 생성하고 회전에 적용.
                        currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -Gravity) * currentRotation;
                    }
                break;
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
            Vector3 targetMovementVelocity = Vector3.zero;            

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        // 지면 경사로에서 원본 속도 재조정 (경사로 변화에 의한 속도 손실을 예방).
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                        // 목표 속도 계산.
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                        float applyMaxMoveSpeed = _sprinting ? MaxStableSprintSpeed : MaxStableWalkSpeed;
                        targetMovementVelocity = reorientedInput * applyMaxMoveSpeed;

                        // 이동속도 스무딩 처리.
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));

                        // 애니메이터에 이동속력 전달
                        float desiredAnimSpeed = AnimDirection.sqrMagnitude <= 0.1f ? 0 : applyMaxMoveSpeed;
                        _animMoveSpeed = Mathf.Lerp(_animMoveSpeed, desiredAnimSpeed, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        PlayerAnimator.SetFloat(_moveSpeedHash, _animMoveSpeed);
                    }
                    else
                    {
                        // 공중 이동 입력.
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                            // 공중 이동중에 불안정안 경사면을 올라가는것을 방지.
                            if (Motor.GroundingStatus.FoundAnyGround)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                            }

                            // 중력을 제외한 목표속도와 현재속도 차이를 계산, 공중가속속력을 적용하여 목표속도에 점진적으로 도달하도록 계산.
                            Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                            currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                        }

                        // 중력 적용.
                        currentVelocity += Gravity * deltaTime;

                        // 저항 적용.
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }

                    HandleJumpVelocity(ref currentVelocity, deltaTime);

                    // 마지막에 더하기.
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }
                break;
                case CharacterState.Charging:
                    // 멈추거나 속도를 취소하려면 여기서 진행.
                    if (_mustStopVelocity)
                    {
                        //currentVelocity = Vector3.zero;
                        _mustStopVelocity = false;
                    }

                    if (_isStopped)
                    {
                        // 정지할 경우, 중력을 제외한 속도조작은 하면 안됨.
                        currentVelocity += Gravity * deltaTime;
                    }
                    else
                    {
                        // 돌진중에는, 속도는 일정하게 유지
                        float previousY = currentVelocity.y;
                        currentVelocity = _currentChargeVelocity;
                        currentVelocity.y = previousY;
                        currentVelocity += Gravity * deltaTime;
                    }
                break;
                case CharacterState.NoClip:
                    float verticalInput = 0f + (_jumpInputIsHeld ? 1f : 0f) + (_crouchInputIsHeld ? -1f : 0f);

                    // Smoothly interpolate to target velocity
                    targetMovementVelocity = (_moveInputVector + (Motor.CharacterUp * verticalInput)).normalized * NoClipMoveSpeed;
                    currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-NoClipSharpness * deltaTime));
                break;
            }

        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            // This is called after the motor has finished everything in its update

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    // 점프 요청 플래그가 활성화 된 상태에서, 일정 시간이 경과되면 무효처리. (여유시간 보정).
                    if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                    {
                        _jumpRequested = false;
                    }
                    // 지면 상태에 따른 점프 값 리셋 및 타이머 관리.
                    if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                    {
                        // 지면 위에 있는데 점프를 하지 않는다면 상태 업데이트.
                        if (!_jumpedThisFrame)
                        {
                            _doubleJumpConsumed = false;
                            _jumpConsumed = false;
                        }
                        // 점프 여유시간 타이머 초기화.
                        _timeSinceLastAbleToJump = 0f;
                    }
                    else
                    {
                        // 지면에 없는 상태에서 점프가능 시간에 점프 허용을 판단할 때 사용.
                        _timeSinceLastAbleToJump += deltaTime;
                    }

                    // 웅크리기 해제.
                    if (_isCrouching && !_shouldBeCrouching)
                    {
                        
                        // 캐릭터의 서 있는 높이와 장애물이 겹치는지 테스트.
                        Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                        if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders, Motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
                        {
                            // 장애물이 있다면 원위치.
                            Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                        }
                        else
                        {
                            // 장애물이 없다면 웅크리기 해제.
                            MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                            _isCrouching = false;
                        }
                    }
                break;
                case CharacterState.Charging:
                    // 경과시간에 따른 정지요소 감지.
                    if (!_isStopped && _timeSinceStartedCharge > MaxChargeTime)
                    {
                        _mustStopVelocity = true;
                        _isStopped = true;
                    }

                    // 멈추기 종료 단계를 감지하고 기본이동 상태로 전환.
                    if (_timeSinceStopped > StoppedTime)
                    {
                        TransitionToState(CharacterState.Default);
                    }
                break;
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

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    // 지면에 있지 않고, 장애물에 부딧친 경우에만 벽 점프 가능.
                    if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                    {
                        _canWallJump = true;
                        _wallJumpNormal = hitNormal;
                    }
                break;
                case CharacterState.Charging:
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
                    }
                }

                // 일반 점프 및 벽 점프 체크 (접지 상태 체크 및 여유시간 보정).
                if (_canWallJump ||
                    (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)))
                {
                    // 점프 방향 설정.
                    Vector3 jumpDirection = Motor.CharacterUp;
                    // 점프 방향을 벽 노말로 변경.
                    if (_canWallJump)
                    {
                        jumpDirection = (_wallJumpNormal + -Gravity.normalized).normalized;
                    }
                    // 경사면인 경우 지면의 노말로 변경.
                    else if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                    {
                        jumpDirection = (Motor.GroundingStatus.GroundNormal + -Gravity.normalized).normalized;
                    }

                    // 캐릭터가 지면에 고정되는 것을 방지.
                    Motor.ForceUnground(0.1f);

                    // 기존 속도의 수직 성분을 제거하고 계산된 점프 방향으로 힘을 적용.
                    currentVelocity += (jumpDirection * JumpSpeed * 1.2f) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                    // 점프 상태 업데이트
                    _jumpRequested = false;
                    _jumpConsumed = true;
                    _jumpedThisFrame = true;
                }
            }

            // 벽점프 플래그를 초기화 하여 조건을 다시 평가.
            _canWallJump = false;
        }

        protected void OnLanded()
        {
            
        }

        protected void OnLeaveStableGround()
        {
            
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


