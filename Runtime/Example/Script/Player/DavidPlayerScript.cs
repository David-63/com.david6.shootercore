using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

namespace David6.ShooterFramework
{
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson
    }

    [RequireComponent(typeof(PlayerInput))]
    public class DavidPlayerScript : MonoBehaviour
    {
        #region Serialize fields

        [Header("제어할 오브젝트 및 트렌스폼")]
        [Tooltip("플레이어가 제어하는 카메라 스크립트"), SerializeField]
        private DavidCharacterCamera OrbitCamera;
        [Tooltip("카메라의 타겟을 설정할 트랜스폼"), SerializeField]
        private Transform CameraFollowPoint;
        [Tooltip("플레이어가 제어할 캐릭터 컨트롤러"), SerializeField]
        private DavidCharacterController Character;
        [Tooltip("플레이어가 제어할 캐릭터 컨트롤러"), SerializeField]
        private List<Collider> IgnoredColliders = new List<Collider>();


        #endregion

        #region Fields

        [Header("디버깅용 입력 값")]
        /// <summary>
        /// 이동키 입력 값.
        /// </summary>
        public Vector2 _axisMovement;
        /// <summary>
        /// 시점 입력 값.
        /// </summary>
        public Vector2 _axisLook;

        /// <summary>
        /// 시점 입력의 보간값.
        /// </summary>
        public Vector2 _averagedLook;
        private CameraMode _currentCameraMode = CameraMode.FirstPerson;
        /// <summary>
        /// 카메라에 전달하는 시점 입력값.
        /// </summary>
        private Vector3 _lookInputVector = Vector3.zero;

        // 시점 보간을 위한 배열과 인덱스.
        private const int _maxLookIndex = 3;
        private int _lookCacheIndex = 0;
        private Vector2[] _axisLookArray = new Vector2[_maxLookIndex];

        private bool _sprint = false;
        private bool _crouch = false;
        private bool _jump = false;
        private bool _dash = false;
        private bool _aim = false;
        private bool _charge = false;
        private bool _noClip = false;

		private bool _holdingButtonSprint = false;
        private bool _holdingButtonCrouch = false;
        private bool _holdingButtonJump = false;
        private bool _holdingButtonAim = false;
		private bool _holdingButtonFire = false;
        
        private bool _cursorLocked = true;

        #endregion

        private void Start()
        {
            UpdateCursorState();
            OrbitCamera.SetFollowTransform(CameraFollowPoint);

            Character.SetIgnoredColliders(IgnoredColliders);
            List<Collider> combinedColliders = IgnoredColliders.Concat(Character.GetComponentsInChildren<Collider>()).ToList();
            OrbitCamera.SetIgnoredColliders(combinedColliders);

        }

        private void Update()
        {
            _crouch = _holdingButtonCrouch;
            _sprint = _holdingButtonSprint && CanSprint();
            _aim = _holdingButtonAim && CanAim();

            _averagedLook = CalcAverageLook(_axisLook);
            // Look 캐시 인덱스 증가
            IncreaseLookCacheIndex();

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        #region 카메라 제어

        /// <summary>
        /// 게임 포커스 제어
        /// </summary>
        private void UpdateCursorState()
        {
            Cursor.visible = !_cursorLocked;
            Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        /// <summary>
        /// LateUpdate에서 카메라 제어
        /// </summary>
        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            _lookInputVector = new Vector3(_averagedLook.x, _averagedLook.y, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                _lookInputVector = Vector3.zero;
            }

            // 카메라 입력처리
            OrbitCamera.UpdateWithInput(Time.deltaTime, _lookInputVector);
        }

        private Vector2 CalcAverageLook(Vector2 axisLook)
        {
            _axisLookArray[_lookCacheIndex] = axisLook;
            Vector2 result = Vector2.zero;
            for (int idx = 0; idx < _axisLookArray.Length; ++idx)
            {
                result += _axisLookArray[idx];
            }
            return result / _axisLookArray.Length;
        }

        private void IncreaseLookCacheIndex()
        {
            ++_lookCacheIndex;
            _lookCacheIndex %= _maxLookIndex;
        }

        #endregion

        #region 캐릭터 제어

        /// <summary>
        /// Update 에서 이동 입력 제어
        /// </summary>
        private void HandleCharacterInput()
        {            
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs(_axisMovement, OrbitCamera.Transform.rotation, _sprint, _jump, _holdingButtonJump, _crouch, _holdingButtonCrouch, _charge, _noClip);
            Character.SetInputs(ref characterInputs);
            _jump = false;
            _charge = false;
            _noClip = false;

            if (_dash)
            {
                Character.Motor.ForceUnground(0.1f);
                Vector3 direction = OrbitCamera.Transform.rotation * Vector3.forward;

                direction = direction.normalized + -Character.Gravity.normalized; // 0.2f는 강도 조절
                Character.Dash(direction.normalized);

                _dash = false;
            }            
        }

        #endregion

        #region 액션 상태 체크

        private bool CanFire()
        {
            return true;
        }
        private bool CanReload()
        {
            return true;
        }
        private bool CanAim()
        {
            return true;
        }

        private bool CanSprint()
        {
            if (_crouch) return false;
            if (_aim) return false;
            if (_holdingButtonFire) return false;
            // || Math.Abs(Mathf.Abs(_axisMovement.x) - 1) < 0.01f
            if (_axisMovement.y <= 0) return false;

            return true;
        }

        #endregion

        #region InputSystem

        /// <summary>
        /// 사격 입력받음.
        /// </summary>
        /// <param name="context"></param>
        public void OnTryFire(InputAction.CallbackContext context)
        {
            if (!_cursorLocked) return;

            switch (context)
            {
                case {phase: InputActionPhase.Started}:
                    _holdingButtonFire = true;
                break;
                case {phase: InputActionPhase.Performed}:
                    if (!CanFire()) break;

                break;
                case {phase: InputActionPhase.Canceled}:
                    _holdingButtonFire = false;
                break;
            }
        }

        /// <summary>
        /// 재장전 입력받음.
        /// </summary>
        /// <param name="context"></param>
        public void OnTryReload(InputAction.CallbackContext context)
        {
            if (!_cursorLocked) return;
            if (!CanReload()) return;

            switch (context)
            {
                case {phase: InputActionPhase.Performed}:
                // DO reload
                break;
            }
        }        

        /// <summary>
		/// 달리기 입력받음.
		/// </summary>
		public void OnTrySprinting(InputAction.CallbackContext context)
		{
			if (!_cursorLocked) return;
            switch (context.phase)
			{
				case InputActionPhase.Started:
					_holdingButtonSprint = true;
                break;
				case InputActionPhase.Canceled:
					_holdingButtonSprint = false;
                break;
			}
		}
        /// <summary>
		/// 웅크리기 입력받음.
		/// </summary>
		public void OnTryCrouch(InputAction.CallbackContext context)
		{
			if (!_cursorLocked) return;
            switch (context.phase)
			{
				case InputActionPhase.Started:
					_holdingButtonCrouch = true;
                break;
				case InputActionPhase.Canceled:
					_holdingButtonCrouch = false;
                break;
			}
		}
        /// <summary>
		/// 정조준 입력받음.
		/// </summary>
		public void OnTryAiming(InputAction.CallbackContext context)
		{
			if (!_cursorLocked) return;

			switch (context.phase)
			{
				case InputActionPhase.Started:
					_holdingButtonAim = true;
                break;
				case InputActionPhase.Canceled:
					_holdingButtonAim = false;
                break;
			}
		}
        /// <summary>
        /// 점프 입력받음.
        /// </summary>
        public void OnTryJump(InputAction.CallbackContext context)
        {
			if (!_cursorLocked) return;

			switch (context.phase)
            {
                case InputActionPhase.Started:
					_holdingButtonJump = true;
                break;
				
                case InputActionPhase.Performed:
                    _jump = true;
                break;
                case InputActionPhase.Canceled:
					_holdingButtonJump = false;
                break;
            }
        }
        public void OnTryDash(InputAction.CallbackContext context)
        {
			if (!_cursorLocked) return;
			switch (context)
            {
                case {phase: InputActionPhase.Performed}:
                    _dash = true;
                break;
            }
        }
        public void OnTryCharge(InputAction.CallbackContext context)
        {
			if (!_cursorLocked) return;
			switch (context)
            {
                case {phase: InputActionPhase.Performed}:
                    _charge = true;
                break;
            }
        }
        public void OnTryNoClip(InputAction.CallbackContext context)
        {
			if (!_cursorLocked) return;
			switch (context)
            {
                case {phase: InputActionPhase.Performed}:
                    _noClip = true;
                break;
            }
        }
        
        public void OnCameraModeSwitch(InputAction.CallbackContext context)
		{
			switch (context)
			{
				case {phase: InputActionPhase.Performed}:
                    _currentCameraMode = _currentCameraMode == CameraMode.FirstPerson ? CameraMode.ThirdPerson : CameraMode.FirstPerson;
                break;
			}

            OrbitCamera.TargetDistance = _currentCameraMode == CameraMode.FirstPerson ? OrbitCamera.DefaultDistance : 0f;
		}
        public void OnLockCursor(InputAction.CallbackContext context)
		{
			switch (context)
			{
				case {phase: InputActionPhase.Performed}:
					_cursorLocked = !_cursorLocked;
                break;
			}
		}
        

        /// <summary>
        /// 이동 입력받음.
        /// </summary>
        /// <param name="context"></param>
        public void OnMove(InputAction.CallbackContext context)
        {
            _axisMovement = _cursorLocked ? context.ReadValue<Vector2>() : default;
        }
        /// <summary>
        /// 카메라 입력받음.
        /// </summary>
        /// <param name="context"></param>
        public void OnLook(InputAction.CallbackContext context)
        {
            _axisLook = _cursorLocked ? context.ReadValue<Vector2>() : default;
        }

        #endregion

    }
}
