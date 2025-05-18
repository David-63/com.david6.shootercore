using UnityEngine;

namespace David6.ShooterFramework
{
	[RequireComponent(typeof(CharacterController))]
    public partial class DaveController : MonoBehaviour
    {
		[Header("핵심")]
		[Tooltip("플레이어")]
        [SerializeField] private DavePlayer InputProvider;
		[Tooltip("Movement 속성 애셋")]
        [SerializeField] private MovementSO MovementAsset;
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		[SerializeField] private GameObject CinemachineCameraTarget;
		[SerializeField] private DaveCamera FollowCamera;

		[SerializeField] private CameraSetupSO UnEquipCameraSetup;
		[SerializeField] private CameraSetupSO EquipCameraSetup;
		[SerializeField] private CameraSetupSO AimCameraSetup;



        private CharacterController _controller;
        private GameObject _mainCamera;


        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
		{
			_controller = GetComponent<CharacterController>();
            InitializeAnimator();
			

			// reset our timeouts on start
			_jumpTimeoutDelta = MovementAsset.JumpTimeout;
			_fallTimeoutDelta = MovementAsset.FallTimeout;
		}

        private void FixedUpdate()
        {
            // if (_aimDownSight)
			// {
			// 	Ray aimRay = Camera.main.ScreenPointToRay()
			// }
        }

        private void Update()
		{
			if (InputProvider == null) return;

			UpdateGravity();
			GroundedCheck();
			Movement();
		}

        private void LateUpdate()
		{
			if (InputProvider == null) return;
			
			CameraRotation();
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - MovementAsset.GroundedOffset, transform.position.z), MovementAsset.GroundedRadius);
		}
    }
}