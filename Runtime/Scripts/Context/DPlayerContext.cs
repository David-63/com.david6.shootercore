using David6.ShooterCore.Animation;
using David6.ShooterCore.Combat;
using David6.ShooterCore.Cooldown;
using David6.ShooterCore.Data;
using David6.ShooterCore.Provider;
using David6.ShooterCore.StateMachine.Action;
using David6.ShooterCore.StateMachine.Locomotion;
using David6.ShooterCore.TickSystem;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.Context
{
    /// <summary>
    /// Player context for the DPlayer.
    /// </summary>
    public partial class DPlayerContext : MonoBehaviour, IDContextProvider, IDTickable
    {
        // 등록된 컴포넌트        
        CharacterController _characterController;
        DAnimationEventProxy _animationEventProxy;


        // 생성한 컴포넌트
        IDAnimatorProvider _animatorHandler;
        IDStateMachineProvider _locomotionStateMachine;
        IDStateMachineProvider _actionStateMachine;
        IDCooldownProvider _cooldownHandler;
        DCombatHandler _combatHandler;

        // 외부 컴포넌트
        IDRootPanelControllerProvider _rootPanelController;
        IDCameraHandlerProvider _cameraHandler;


        // 리소스
        [SerializeField] DMovementProfile _movementProfile;
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;


        // 컴포넌트 가져오기
        void Awake()
        {
            var bootstrapper = FindAnyObjectByType(typeof(DPlayerBootstrapper)) as DPlayerBootstrapper;
            bootstrapper.Register<IDContextProvider>(this);

            _characterController = GetComponent<CharacterController>();
            if (this.TryGetComponentInChildren<Animator>(out var animComponent))
            {
                _animatorHandler = new DAnimatorController(animComponent);
            }
            if (this.TryGetComponentInChildren<DAnimationEventProxy>(out var proxy))
            {
                _animationEventProxy = proxy;
            }
        }
        void Start()
        {
            _locomotionStateMachine = new DLocomotionStateMachine(this);
            _actionStateMachine = new DActionStateMachine(this);
            _cooldownHandler = new DCooldownHandler();
            _combatHandler = new DCombatHandler();

            InitializeCharacterController();

            _animatorHandler.SetGrounded(true);

            _animationEventProxy.OnFootstepEvent += OnFootstep;
            _animationEventProxy.OnLandEvent += OnLand;



            if (DGameLoop.Instance != null)
            {
                DGameLoop.Instance.Register(this);
                DGameLoop.Instance.Register(_cooldownHandler);
            }
        }

        void OnDestroy()
        {
            if (DGameLoop.Instance != null)
            {
                DGameLoop.Instance.Unregister(this);
                DGameLoop.Instance.Unregister(_cooldownHandler);
            }
        }

        public void Tick(float deltaTime)
        {
            FocusCheck();
            GroundCheck();

            _locomotionStateMachine.OnUpdate(deltaTime);
            _actionStateMachine.OnUpdate(deltaTime);

            ApplyMovement();
            CameraTransitionCheck();
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

        void OnDrawGizmosSelected()
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
        
        void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_characterController.center), FootstepAudioVolume);
                }
            }
        }

        void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_characterController.center), FootstepAudioVolume);
            }
        }

    }
}