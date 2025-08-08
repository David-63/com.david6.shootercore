

using System;
using David6.ShooterCore.Data;
using David6.ShooterCore.Provider;
using UnityEngine;
using UnityEngine.InputSystem;

namespace David6.ShooterCore.Input
{
    public class DInputHandler : MonoBehaviour, IDInputProvider
    {
        [SerializeField] DInputSettingProfile inputSettingProfile;

        #region Events for External Subscribers
        public event Action OnPause = delegate { };
        public event Action OnResume = delegate { };
        public event Action OnPop = delegate { };

        public event Action<Vector2> OnMove = delegate { };
        public event Action<Vector2> OnLook = delegate { };
        public event Action OnStartJump = delegate { };
        public event Action OnStopJump = delegate { };
        public event Action OnStartRun = delegate { };
        public event Action OnStopRun = delegate { };
        public event Action OnStartAim = delegate { };
        public event Action OnStopAim = delegate { };
        public event Action OnStartFire = delegate { };
        public event Action OnStopFire = delegate { };
        public event Action OnStartReload = delegate { };
        public event Action OnStopReload = delegate { };

        Action<InputAction.CallbackContext> _onPause, _onResume, _onPop;
        Action<InputAction.CallbackContext> _onStartJump, _onStopJump, _onStartRun, _onStopRun;
        Action<InputAction.CallbackContext> _onStartAim, _onStopAim, _onStartFire, _onStopFire;
        Action<InputAction.CallbackContext> _onStartReload, _onStopReload;
        
        #endregion

        #region 내부 액션 참조
        InputActionMap _basicMap, _UIMap;
        InputAction _pauseAction, _resumeAction, _popAction;
        InputAction _moveAction, _lookAction, _jumpAction, _runAction;
        InputAction _aimAction, _fireAction, _reloadAction;
        #endregion


        #region 캐싱

        Vector2 _prevMove, _prevLook;

        #endregion

        private void Awake()
        {
            _basicMap = inputSettingProfile.InputActions.FindActionMap("Basic", throwIfNotFound: true);
            _UIMap = inputSettingProfile.InputActions.FindActionMap("UI", throwIfNotFound: true);

            _pauseAction = _basicMap.FindAction("Pause", throwIfNotFound: true);
            _resumeAction = _UIMap.FindAction("Resume", throwIfNotFound: true);
            _popAction = _UIMap.FindAction("Pop", throwIfNotFound: true);

            _moveAction = _basicMap.FindAction("Move", throwIfNotFound: true);
            _lookAction = _basicMap.FindAction("Look", throwIfNotFound: true);
            _jumpAction = _basicMap.FindAction("Jump", throwIfNotFound: true);
            _runAction = _basicMap.FindAction("Run", throwIfNotFound: true);

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
            _onPause = _ => HandlePause();
            _pauseAction.performed += _onPause;
            _onStartJump = _ => OnStartJump();
            _jumpAction.performed += _onStartJump;
            _onStopJump = _ => OnStopJump();
            _jumpAction.canceled += _onStopJump;

            _onStartRun = _ => OnStartRun();
            _runAction.performed += _onStartRun;
            _onStopRun = _ => OnStopRun();
            _runAction.canceled += _onStopRun;

            _onStartAim = _ => OnStartAim();
            _aimAction.performed += _onStartAim;
            _onStopAim = _ => OnStopAim();
            _aimAction.canceled += _onStopAim;
            _onStartFire = _ => OnStartFire();
            _fireAction.performed += _onStartFire;
            _onStopFire = _ => OnStopFire();
            _fireAction.canceled += _onStopFire;

            _onStartReload = _ => OnStartReload();
            _reloadAction.performed += _onStartReload;
            _onStopReload = _ => OnStopReload();
            _reloadAction.canceled += _onStopReload;
        }
        private void UnsubscribeBasicActions()
        {
            ClearActionInput();

            _pauseAction.performed -= _onPause;
            _jumpAction.performed -= _onStartJump;
            _jumpAction.canceled -= _onStopJump;
            _runAction.performed -= _onStartRun;
            _runAction.canceled -= _onStopRun;
            _aimAction.performed -= _onStartAim;
            _aimAction.canceled -= _onStopAim;
            _fireAction.performed -= _onStartFire;
            _fireAction.canceled -= _onStopFire;
            _reloadAction.performed -= _onStartReload;
            _reloadAction.canceled -= _onStopReload;

        }
        private void SubscribeUIActions()
        {
            _onResume = _ => OnResume();
            _resumeAction.performed += _onResume;
            _onPop = _ => OnPop();
            _popAction.performed += _onPop;
        }
        private void UnsubscribeUIActions()
        {
            _resumeAction.performed -= _onResume;
            _popAction.performed -= _onPop;
        }

        private void Update()
        {
            if (_basicMap.enabled)
            {
                Vector2 moveValue = _moveAction.ReadValue<Vector2>();
                if (moveValue != _prevMove)
                {
                    _prevMove = moveValue;
                    OnMove(moveValue);
                }

                Vector2 lookValue = _lookAction.ReadValue<Vector2>();
                if (lookValue != _prevLook)
                {
                    _prevLook = lookValue;
                    OnLook(lookValue);
                }
            }

            // UI map 에서 폴링이 필요하면 이곳에 액션 추가
        }

        public void HandlePause()
        {
            OnPause();

            _basicMap.Disable();
            UnsubscribeBasicActions();
            _UIMap.Enable();
            SubscribeUIActions();
        }

        public void HandleResume()
        {
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
            OnStopRun();
            OnStopAim();
            OnStopFire();
            OnStopReload();
        }
    }
}