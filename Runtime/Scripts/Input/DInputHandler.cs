

using System;
using David6.ShooterCore.Data;
using David6.ShooterCore.Provider;
using UnityEngine;
using UnityEngine.InputSystem;

namespace David6.ShooterCore.Input
{
    public class DInputHandler : MonoBehaviour, IDInputProvider
    {
        [Header("인풋 액션 에셋")]
        [SerializeField] private DInputSettingProfile inputSettingProfile;

        #region 외부 구독용 이벤트
        public event Action OnPause = delegate { };
        public event Action OnResume = delegate { };
        public event Action<Vector2> OnMove = delegate { };
        public event Action<Vector2> OnLook = delegate { };
        public event Action OnStartJump = delegate { };
        public event Action OnStopJump = delegate { };
        public event Action OnStartSprint = delegate { };
        public event Action OnStopSprint = delegate { };
        public event Action OnStartAim = delegate { };
        public event Action OnStopAim = delegate { };
        public event Action OnStartFire = delegate { };
        public event Action OnStopFire = delegate { };
        public event Action OnStartReload = delegate { };
        public event Action OnStopReload = delegate { };
        #endregion

        #region 내부 액션 참조
        private InputActionMap _basicMap, _UIMap;
        private InputAction _pauseAction, _resumeAction;
        private InputAction _moveAction, _lookAction, _jumpAction, _sprintAction;
        private InputAction _aimAction, _fireAction, _reloadAction;
        #endregion

        private void Awake()
        {
            _basicMap = inputSettingProfile.InputActions.FindActionMap("Basic", throwIfNotFound: true);
            _UIMap = inputSettingProfile.InputActions.FindActionMap("UI", throwIfNotFound: true);

            _pauseAction = _basicMap.FindAction("Pause", throwIfNotFound: true);
            _resumeAction = _UIMap.FindAction("Resume", throwIfNotFound: true);

            _moveAction = _basicMap.FindAction("Move", throwIfNotFound: true);
            _lookAction = _basicMap.FindAction("Look", throwIfNotFound: true);
            _jumpAction = _basicMap.FindAction("Jump", throwIfNotFound: true);
            _sprintAction = _basicMap.FindAction("Sprint", throwIfNotFound: true);

            _aimAction = _basicMap.FindAction("Aim", throwIfNotFound: true);
            _fireAction = _basicMap.FindAction("Fire", throwIfNotFound: true);
            _reloadAction = _basicMap.FindAction("Reload", throwIfNotFound: true);
        }

        private void OnEnable()
        {
            _basicMap.Enable();
            SubscribeBasicActions();

            _UIMap.Disable();
            UnsubscribeUIActions();
        }

        private void OnDisable()
        {
            _basicMap.Disable();
            UnsubscribeBasicActions();
            _UIMap.Disable();
            UnsubscribeUIActions();
        }

        private void SubscribeBasicActions()
        {
            _pauseAction.performed += _ => HandlePause();
            _jumpAction.performed += _ => OnStartJump();
            _jumpAction.canceled += _ => OnStopJump();
            _sprintAction.performed += _ => OnStartSprint();
            _sprintAction.canceled += _ => OnStopSprint();
            _aimAction.performed += _ => OnStartAim();
            _aimAction.canceled += _ => OnStopAim();
            _fireAction.performed += _ => OnStartFire();
            _fireAction.canceled += _ => OnStopFire();
            _reloadAction.performed += _ => OnStartReload();
            _reloadAction.canceled += _ => OnStopReload();
        }
        private void UnsubscribeBasicActions()
        {
            ClearActionInput();

            _pauseAction.performed -= _ => HandlePause();
            _jumpAction.performed -= _ => OnStartJump();
            _jumpAction.canceled -= _ => OnStopJump();
            _sprintAction.performed -= _ => OnStartSprint();
            _sprintAction.canceled -= _ => OnStopSprint();
            _aimAction.performed -= _ => OnStartAim();
            _aimAction.canceled -= _ => OnStopAim();
            _fireAction.performed -= _ => OnStartFire();
            _fireAction.canceled -= _ => OnStopFire();
            _reloadAction.performed -= _ => OnStartReload();
            _reloadAction.canceled -= _ => OnStopReload();

        }
        private void SubscribeUIActions()
        {
            _resumeAction.performed += _ => HandleResume();
        }
        private void UnsubscribeUIActions()
        {
            _resumeAction.performed -= _ => HandleResume();
        }

        private void Update()
        {
            if (_basicMap.enabled)
            {
                Vector2 moveValue = _moveAction.ReadValue<Vector2>();
                OnMove(moveValue);

                Vector2 lookValue = _lookAction.ReadValue<Vector2>();
                OnLook(lookValue);
            }

            // UI map 에서 폴링이 필요하면 이곳에 액션 추가
        }

        private void HandlePause()
        {
            OnPause();

            _basicMap.Disable();
            UnsubscribeBasicActions();
            _UIMap.Enable();
            SubscribeUIActions();
        }

        private void HandleResume()
        {
            OnResume();

            _UIMap.Disable();
            UnsubscribeUIActions();

            _basicMap.Enable();
            SubscribeBasicActions();
        }

        private void ClearActionInput()
        {
            OnLook(Vector2.zero);
            OnMove(Vector2.zero);
            OnStopJump();
            OnStopSprint();
            OnStopAim();
            OnStopFire();
            OnStopReload();
        }
    }
}