
using System.Collections;
using David6.ShooterCore.Animator;
using David6.ShooterCore.Movement;
using David6.ShooterCore.Provider;
using David6.ShooterCore.StateMachine;
using UnityEngine;

namespace David6.ShooterCore.Context
{
    /// <summary>
    /// Player context for the DPlayer.
    /// </summary>
    public partial class DPlayerContext : MonoBehaviour, IDContextProvider
    {
        private CharacterController _characterController;
        [SerializeField] private DMovementProfile _movementProfile;
        public DMovementProfile MovementProfile { get { return _movementProfile; } }
        [SerializeField] private IDAnimatorProvider AnimatorProvider;

        private IDStateMachineProvider _stateMachine;
        private IDStateFactoryProvider _stateFactory;


        // [속력 변수]
        public float _currentSpeed;
        // [회전 변수]
        private float _rotationSpeed;
        private float _targetRotation;
        private float _characterRotation;

        // [이동 방향]
        public Vector3 _lastDirection;
        public Vector3 _moveDirection;
        public float _verticalSpeed;


        // [수직 제어]

        public bool IsForward() => _lastDirection.z >= -0.8f;

        void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _stateFactory = new DStateFactory(this);
            _currentState = _stateFactory.Grounded();
            _currentState.EnterState();
            //_stateMachine = new DStateMachine();
        }
        void Start()
        {
            InitializeCharacterController();
        }

        void Update()
        {
            GroundCheck();

            InputMoveSpeed();
            CalculateGroundSpeed();
            CalculateMoveDirection();
            ApplyCharacterRotation();
            ApplyMovement();

            //_stateMachine.OnUpdate();
            _currentState.UpdateSelf();
        }

        // 임시
        void ResetInput()
        {
            InputJump = false;
        }

        void InitializeCharacterController()
        {
            gameObject.layer = 3;
            if (_characterController == null)
            {
                _characterController.stepOffset = 0.25f;
                _characterController.skinWidth = 0.02f;
                _characterController.minMoveDistance = 0;
                _characterController.center = new Vector3(0, 0.93f, 0);
                if (MovementProfile)
                {
                    _characterController.radius = MovementProfile.GroundedRadius;
                }
                _characterController.height = 1.8f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (_grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - _movementProfile.GroundedOffset, transform.position.z),
                _movementProfile.GroundedRadius);
        }

    }
}