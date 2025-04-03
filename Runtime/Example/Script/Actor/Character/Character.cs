using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace David6.ShooterFramework
{
    /// <summary>
    /// 핵심 캐릭터 컴포넌트입니다
    /// 캐릭터의 가장 중요한 기능을 처리하며, 거의 모든 부분의 에셋과 인터페이스를 모아주는 허브 역할을 합니다
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
	[RequireComponent(typeof(PlayerController))]
    public sealed class Character : CharacterBehaviour
    {
        #region Fields Serialized

        

        #endregion

        #region Fields

        /// <summary>
        /// 달리는 상태인지 확인.
        /// </summary>
        private bool _sprinting;
        /// <summary>
        /// 조준 상태인지 확인.
        /// </summary>
        private bool _aiming;

        private bool _jumping;
        /// <summary>
        /// 공격을 제어하는 변수. 마지막 시간을 기록하여 쿨다운 확인
        /// </summary>
        private float _lastShotTime;
        private Vector2 _axisLook;
        private Vector2 _axisMovement;

        /// <summary>
        /// 달리기 키 입력 확인
        /// </summary>
		private bool _holdingButtonSprint;
		/// <summary>
        /// 조준 키 입력 확인
        /// </summary>
        private bool _holdingButtonAim;
        /// <summary>
        /// 점프 키 입력 확인
        /// </summary>
        private bool _holdingButtonJump;
        /// <summary>
        /// 사격 키 입력 확인
        /// </summary>
		private bool _holdingButtonFire;
        
        private bool _cursorLocked;
        private bool _cameraSwitch;
        #endregion

        #region 유니티 기본 함수
        
        protected override void Awake()
        {
            #region 커서 잠금

            _cursorLocked = true;
            UpdateCursorState();

            #endregion

        }
        protected override void Update()
        {
            _sprinting = _holdingButtonSprint && CanSprint();
            _aiming = _holdingButtonAim && CanAim();
            _jumping = _holdingButtonJump;

            if (_holdingButtonFire)
            {
                if (CanFire())
                {
                    if (Time.time - _lastShotTime > 60.0f / 200)
                    {
                        Fire();
                    }
                }
            }
        }
        protected override void LateUpdate()
        {
        }

        #endregion

        #region 프로퍼티

		public override bool IsCrosshairVisible() => !_aiming;

        public override bool IsSprinting() => _sprinting;

        public override bool IsAiming() => _aiming;

        public override bool IsJumping() => _jumping;

		public override bool IsCursorLocked() => _cursorLocked;
		public override bool IsCameraSwitch() => _cameraSwitch;

        public override Vector2 GetInputMovement() => _axisMovement;

        public override Vector2 GetInputLook() => _axisLook;

        #endregion


        #region 로직 함수

        /// <summary>
        /// 무기 사격 함수
        /// </summary>
        private void Fire()
        {
            _lastShotTime = Time.time;
        }

        private void FireEmpty()
        {
            _lastShotTime = Time.time;
        }

        private void UpdateCursorState()
        {
            Cursor.visible = !_cursorLocked;
            Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        #region 액션 상태 체크

        private bool CanFire()
        {
            return true;
        }
        private bool CanReload()
        {
            if (_jumping) return false;
            return true;
        }
        private bool CanAim()
        {
            if (_jumping) return false;

            return true;
        }

        private bool CanSprint()
        {
            if (_aiming) return false;
            if (_jumping) return false;

            if (_holdingButtonFire) return false;

            if (_axisMovement.y <= 0 || Math.Abs(Mathf.Abs(_axisMovement.x) - 1) < 0.01f) return false;

            return true;
        }

        #endregion

        #region InputSystem 이벤트 함수

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

                    if (Time.time - _lastShotTime > 60.0f / 200)
                    {
                        Fire();
                        // FireEmpty();
                    }
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
                case InputActionPhase.Canceled:
                    _holdingButtonJump = false;
                break;
            }
        }
        public void OnLockCursor(InputAction.CallbackContext context)
		{
			switch (context)
			{
				case {phase: InputActionPhase.Performed}:
					_cursorLocked = !_cursorLocked;
					UpdateCursorState();
                break;
			}
		}
        
        public void OnCameraSwitch(InputAction.CallbackContext context)
		{
			switch (context)
			{
				case {phase: InputActionPhase.Performed}:
					_cameraSwitch = !_cameraSwitch;
					UpdateCursorState();
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

        #endregion

    }
}


