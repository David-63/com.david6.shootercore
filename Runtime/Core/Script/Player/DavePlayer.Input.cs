using UnityEngine;
using UnityEngine.InputSystem;

namespace David6.ShooterFramework
{
    public interface IInputProvider
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool Jump { get; }
        bool Sprint { get; }
    }

	[RequireComponent(typeof(PlayerInput))]
    public partial class DavePlayer : MonoBehaviour, IInputProvider
    {
        #region Input

        // PlayerInput 컴포넌트 자동 참조
        private PlayerInput _playerInput;

        // 내부에서 사용할 입력 값들
        private Vector2 _axisMove = Vector3.zero;
        private Vector2 _axisLook = Vector3.zero;
        // private bool _inputSprint = false;
        // private bool _inputCrouch = false;
        // private bool _inputJump = false;

        private bool _sprint = false;
        private bool _crouch = false;

        private bool _jump = false;
        private bool _equip = false;
        private bool _interact = false;
        private bool _reload = false;
        
        private bool _aim = false;
        private bool _fire = false;

        private bool _holdingButtonSprint = false;
        private bool _holdingButtonCrouch = false;
        
        private bool _holdingButtonJump = false;
        private bool _holdingButtonEquip = false;
        private bool _holdingButtonInteract = false;
        private bool _holdingButtonReload = false;
        
        private bool _holdingButtonAim = false;
        private bool _holdingButtonFire = false;


        
        
        private bool _cursorLocked = true;

        #endregion

        public Vector2 Move => _axisMove;
        public Vector2 Look => _axisLook;
        
        public bool Sprint => _sprint;
        public bool Crouch => _crouch;

        public bool Jump => _jump;
        public bool Equip => _equip;
        public bool Interact => _interact;
        public bool Reload => _reload;

        public bool Aim => _aim;
        public bool Fire => _fire;


        public bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "PC";

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            // C# Events 방식으로 액션 트리거를 구독
            _playerInput.onActionTriggered += OnActionTriggered;
        }
        private void OnDisable()
        {
            _playerInput.onActionTriggered -= OnActionTriggered;
        }

        #region Input Handler

        private void OnActionTriggered(InputAction.CallbackContext ctx)
        {
            switch (ctx.action.name)
            {
                case "Move":
                _axisMove = ctx.ReadValue<Vector2>();
                break;
                case "Look":
                _axisLook = ctx.ReadValue<Vector2>();
                break;

                case "Crouch":
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                    _holdingButtonCrouch = true;
                    break;
                    case InputActionPhase.Canceled:
                    _holdingButtonCrouch = false;
                    break;
                }
                break;
                case "Sprint":
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                    _holdingButtonSprint = true;
                    break;
                    case InputActionPhase.Canceled:
                    _holdingButtonSprint = false;
                    break;
                }
                break;

                case "Jump":
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                    _holdingButtonJump = true;
                    break;                    
                    case InputActionPhase.Canceled:
                    _holdingButtonJump = false;
                    break;
                }
                break;
                case "Equip":
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                    _holdingButtonEquip = true;
                    break;                    
                    case InputActionPhase.Canceled:
                    _holdingButtonEquip = false;
                    break;
                }
                break;
                case "Interact":
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                    _holdingButtonInteract = true;
                    break;                    
                    case InputActionPhase.Canceled:
                    _holdingButtonInteract = false;
                    break;
                }
                break;
                case "Reload":
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                    _holdingButtonReload = true;
                    break;                    
                    case InputActionPhase.Canceled:
                    _holdingButtonReload = false;
                    break;
                }
                break;

                case "Aim":
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                    _holdingButtonAim = true;
                    break;                    
                    case InputActionPhase.Canceled:
                    _holdingButtonAim = false;
                    break;
                }
                break;
                case "Fire":
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                    _holdingButtonFire = true;
                    break;                    
                    case InputActionPhase.Canceled:
                    _holdingButtonFire = false;
                    break;
                }
                break;
            }
        }

        #endregion

        private void InputConditionUpdate()
        {
            _crouch = _holdingButtonCrouch;
            _sprint = _holdingButtonSprint && CanSprint();

            _jump = _holdingButtonJump;
            _equip = _holdingButtonEquip;
            _interact = _holdingButtonInteract;
            _reload = _holdingButtonReload;

            _aim = _holdingButtonAim && CanAim();
            _fire = _holdingButtonFire && CanFire();
        }


        private bool CanSprint()
        {
            if (_crouch) return false;
            if (_equip) return false;
            if (_reload) return false;
            if (_axisMove.y <= 0) return false;

            return true;
        }
        private bool CanAim()
        {
            if (_sprint) return false;
            if (_reload) return false;
            if (_jump) return false;

            return true;
        }
        private bool CanFire()
        {
            if (_sprint) return false;
            if (_reload) return false;

            return true;
        }
    }
}
