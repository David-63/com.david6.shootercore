using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace David6.ShooterFramework
{
    public partial class DavePlayer : MonoBehaviour
    {
        #region Input

        [SerializeField] private InputManager _inputManager;

        // PlayerInput 컴포넌트 자동 참조
        private PlayerInput _playerInput;

        // 내부에서 사용할 입력 값들
        public Vector2 _axisMove, _axisLook;

        private bool _sprint = false;
        private bool _crouch = false;

        private bool _jump = false;
        private bool _equip = false;
        private bool _interact = false;
        private bool _reload = false;

        private bool _aim = false;
        private bool _fire = false;

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


        public bool IsCurrentDeviceMouse() => _inputManager.IsCurrentDeviceMouse;


        private void InputSetup()
        {
            // 버튼 액션 구독
            _inputManager.OnActionTriggered += OnInputTriggered;
            _inputManager.OnActionCanceled += OnInputCanceled;
            // 축 액션 구독
            _inputManager.OnMove += v => _axisMove = v;
            _inputManager.OnLook += v => _axisLook = v;
        }
        private void InputDisable()
        {
            _inputManager.OnActionTriggered -= OnInputTriggered;
            _inputManager.OnActionCanceled -= OnInputCanceled;

            _inputManager.OnMove -= v => _axisMove = v;
            _inputManager.OnLook -= v => _axisLook = v;
        }
        

        #region Input Handler

        private void OnInputTriggered(string actionName)
        {
            switch (actionName)
            {
                case "Jump":
                    ActiveAction(ref _jump);
                    break;
                case "Sprint":
                    ActiveAction(ref _sprint);
                    break;
                case "Equip":
                    ToggleAction(ref _equip);
                    break;
                case "Aim":
                    ActiveAction(ref _aim);
                    break;

                    // …나머지 매핑…
            }
        }

        private void OnInputCanceled(string actionName)
        {
            switch (actionName)
            {
                case "Jump":
                    ReleaseAction(ref _jump);
                    break;
                case "Sprint":
                    ReleaseAction(ref _sprint);
                    break;
                case "Aim":
                    ReleaseAction(ref _aim);
                    break;
                // case "Equip":
                    // ToggleAction(ref _equip);
                    // break;
                    // …나머지 매핑…
            }
        }

        private void ActiveAction(ref bool input)
        {
            input = true;
        }
        private void ReleaseAction(ref bool input)
        {
            input = false;
        }
        private void ToggleAction(ref bool input)
        {
            input = !input;
        }

        private void PressReset()
        {
            ReleaseAction(ref _jump);
        }

        #endregion

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
