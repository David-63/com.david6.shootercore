using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace David6.ShooterFramework
{
    public class InputState
    {
        public bool  Pressed;      // 이번 프레임에 눌렸는지
        public bool  Held;         // 누르고 있는 중인지
        public bool  Released;     // 이번 프레임에 떼졌는지
        public float PressedTime;  // 마지막으로 누른 시각 (LongPress 용)
        public bool  ToggleState;  // Toggle 모드 상태 보관
    }

    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        // 에디터에서 드래그할 SO 리스트
        [SerializeField] private List<ActionSetupSO> ActionSetups;

        // 내부 데이터
        private PlayerInput                         _playerInput;
        private Dictionary<string, ActionSetupSO>   _configMap;
        public Dictionary<string, InputState>       _stateMap;
        private InputBuffer                         _buffer;
        private Dictionary<eInputMode, Func<InputState, ActionSetupSO, float, bool>> _modeChecks;

        // 외부 구독 이벤트: 액션 이름을 발행
        public event Action<string> OnActionTriggered;
        public event Action<string> OnActionCanceled;
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;

        public bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "PC";

        void Awake()
        {
            // PlayerInput 바인딩
            _playerInput = GetComponent<PlayerInput>();

            // 맵 초기화
            _configMap = ActionSetups.ToDictionary(s => s.ActionName);
            _stateMap = ActionSetups.ToDictionary(s => s.ActionName, s => new InputState());
            _buffer = new InputBuffer(ActionSetups.Max(s => s.BufferTime));

            // 모드별 체크 로직 람다 매핑
            _modeChecks = new()
            {
                { eInputMode.Press,     (state,config,now) => state.Pressed },
                { eInputMode.Hold,      (state,config,now) => state.Held },
                { eInputMode.LongPress, (state,config,now) => state.Released && now - state.PressedTime >= config.LongPressDuration },
                { eInputMode.Toggle,    (state,config,now) =>
                    {
                        if (!state.Pressed) return false;
                        bool next = !_stateMap[config.ActionName].ToggleState;
                        _stateMap[config.ActionName].ToggleState = next;
                        return next;
                    }
                },
                // BufferPress는 Interpreter 업데이트에서 별도 처리
            };

            // PlayerInput 액션 이벤트 바인딩
            var playerMap = _playerInput.actions.FindActionMap("Basic");
            foreach (var action in playerMap.actions)
            {
                action.started      += OnStarted;
                action.performed    += OnPerformed;
                action.canceled     += OnCanceled;
                action.Enable();
            }
        }

        /// <summary>
        /// 버튼을 처음 누를시 콜백
        /// Button 타입인 경우 초기 상태 기록
        /// </summary>
        /// <param name="ctx"></param>
        private void OnStarted(InputAction.CallbackContext ctx)
        {
            // 등록된 액션이 없으면 리턴
            if (!_configMap.TryGetValue(ctx.action.name, out var config)) return;
            // 버튼 방식이 아니면 리턴
            if (ctx.action.type != InputActionType.Button) return;


            string name = config.ActionName;
            InputState state = _stateMap[name];
            Debug.Log($"[OnStarted] {config.ActionName} pressed");

            // Start 상태 업데이트
            state.Pressed = true;
            state.Held = true;
            state.PressedTime = Time.time;

            // BufferPress: 누른 순간 예약
            if (config.InputMode == eInputMode.BufferPress)
            {
                _buffer.Enqueue(name, config.BufferTime);
            }

            // Press/Toggle: 즉시 Trigger
            if (config.InputMode == eInputMode.Press || config.InputMode == eInputMode.Toggle)
            {
                OnActionTriggered?.Invoke(name);
            }
            else if (config.InputMode == eInputMode.Hold)
            {
                OnActionTriggered?.Invoke(name);
            }
        }

        /// <summary>
        /// Performed 인 경우 콜백
        /// Value 타입 이벤트 발행
        /// LongPress 모드인 경우 Trigger
        /// </summary>
        /// <param name="ctx"></param>
        private void OnPerformed(InputAction.CallbackContext ctx)
        {
            // 축 입력 처리
            if (ctx.action.type == InputActionType.Value)
            {
                var v = ctx.ReadValue<Vector2>();
                switch (ctx.action.name)
                {
                    case "Move": OnMove?.Invoke(v); break;
                    case "Look": OnLook?.Invoke(v); break;
                }
                return;
            }
            // LongPress: 임계치 충족시 Trigger
            else if (_configMap.TryGetValue(ctx.action.name, out var config)
                && config.InputMode == eInputMode.LongPress
                && ctx.phase == InputActionPhase.Performed)
            {
                OnActionTriggered?.Invoke(config.ActionName);
            }
        }

        /// <summary>
        /// 버튼을 땔 경우 콜백
        /// Held 해제, Released 상태 기록
        /// </summary>
        /// <param name="ctx"></param>
        private void OnCanceled(InputAction.CallbackContext ctx)
        {
            if (ctx.action.type == InputActionType.Value)
            {
                var v = ctx.ReadValue<Vector2>();
                switch (ctx.action.name)
                {
                    case "Move": OnMove?.Invoke(v); break;
                    case "Look": OnLook?.Invoke(v); break;
                }
                return;
            }

            if (!_configMap.TryGetValue(ctx.action.name, out var config)) return;            
            
            var state       = _stateMap[config.ActionName];
            Debug.Log($"[OnCanceled] {config.ActionName} released");
            state.Released  = true;
            state.Held      = false;

            if (config.InputMode != eInputMode.Toggle)
            {
                OnActionCanceled?.Invoke(config.ActionName);
            }
        }

        private void Update()
        {
            float now = Time.time;

            // 1) BufferPress 처리
            foreach (var name in _buffer.DequeueAll()) OnActionTriggered?.Invoke(name);

            // 2) 나머지 모드 처리
            foreach (var config in ActionSetups)
            {
                // Press, BufferPress, Toggle 모드는 Started() 에서만 처리
                if (config.InputMode == eInputMode.Press
                || config.InputMode == eInputMode.BufferPress
                || config.InputMode == eInputMode.Toggle
                || config.InputMode == eInputMode.Hold)
                    continue;

                var name  = config.ActionName;
                var state = _stateMap[name];

                bool should = _modeChecks[config.InputMode](state, config, now);
                if (should)
                {
                    OnActionTriggered?.Invoke(name);
                }

                // 매 프레임 누름/떼짐만 리셋
                state.Pressed  = false;
                state.Released = false;
            }
        }
    }
}
