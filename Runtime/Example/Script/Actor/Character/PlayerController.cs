using UnityEngine;
using UnityEngine.InputSystem;

namespace David6.ShooterFramework
{
    [RequireComponent(typeof(CharacterController))]
    public partial class PlayerController : ControllerBehaviour
    {
        #region Fields
        
        /// <summary>
        /// 플레이어의 각종 데이터를 가진 메니저 객체
        /// </summary>
        private IPlayerManager _playerManager;
        /// <summary>
        /// 플레이어의 메인 컴포넌트 (입력을 받는 용도로 사용)
        /// </summary>
        private CharacterBehaviour _playerCharacter;
        /// <summary>
        /// 유사 물리에 쓰임
        /// </summary>
		private CharacterController _controller;
        /// <summary>
        /// 입력 디바이스 감지하는 용도
        /// </summary>
		private PlayerInput _playerInput;

        #endregion

        #region 유니티 기본 함수

        protected override void Awake()
        {
            _playerManager = ServiceLocator.Current.Get<IPlayerManager>();
            _playerCharacter = _playerManager.GetPlayerCharacter();
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
        }
        protected override void Start()
        {
            InitializeCamera();
            InitializeAnimations();
        }
        protected override void FixedUpdate()
        {
            // 안씀
        }
        protected override void Update()
        {
            ApplyGravity();
			GroundedCheck();
			HandleMovement();
        }
        protected override void LateUpdate()
        {
            CameraRotation();
        }

        private void OnDrawGizmosSelected()
		{
			Gizmos.color = Grounded ? 
				new Color(0.0f, 1.0f, 0.0f, 0.35f) : 
				new Color(1.0f, 0.0f, 0.0f, 0.35f);

			Vector3 pos = transform.position;
			
			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(
				new Vector3(pos.x, pos.y - GroundedOffset, pos.z),
				GroundedRadius);
		}

        private bool IsCurrentDeviceMouse
		{
			get
			{
				return _playerInput.currentControlScheme == "PC";
			}
		}

        #endregion
    }
}
