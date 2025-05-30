using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class DaveController : MonoBehaviour
    {
        private Vector2 _axisMove, _axisLook;

        private bool _inputSprint = false;
        private bool _inputCrouch = false;

        private bool _inputJump = false;
        private bool _inputEquip = false;
        private bool _inputInteract = false;
        private bool _inputReload = false;

        private bool _inputAim = false;
        private bool _inputFire = false;

        private bool _inputDrop = false;

        private bool _inputInventory = false;

        private bool _cursorLocked = true;

        public ItemDataSO dropItem;


        private void InputSetup()
        {
            // 버튼 액션 구독
            InputManager.Instance.OnActionTriggered += OnInputTriggered;
            InputManager.Instance.OnActionCanceled += OnInputCanceled;
            // 축 액션 구독
            InputManager.Instance.OnMove += v => _axisMove = v;
            InputManager.Instance.OnLook += v => _axisLook = v;
        }
        private void InputDisable()
        {
            InputManager.Instance.OnActionTriggered -= OnInputTriggered;
            InputManager.Instance.OnActionCanceled -= OnInputCanceled;

            InputManager.Instance.OnMove -= v => _axisMove = v;
            InputManager.Instance.OnLook -= v => _axisLook = v;
        }

        private void OnInputTriggered(string actionName)
        {
            switch (actionName)
            {
                case "Jump":
                    ActiveAction(ref _inputJump);
                    break;
                case "Sprint":
                    ActiveAction(ref _inputSprint);
                    break;
                case "Equip":
                    ToggleAction(ref _inputEquip);
                    break;
                case "Aim":
                    ActiveAction(ref _inputAim);
                    break;
                case "Drop":
                    ActiveAction(ref _inputDrop);
                    break;
                case "Inventory":
                    ToggleAction(ref _inputInventory);
                    break;
            }
            DoInputTriggerAction(actionName);
        }

        private void OnInputCanceled(string actionName)
        {
            switch (actionName)
            {
                case "Sprint":
                    ReleaseAction(ref _inputSprint);
                    break;
                case "Aim":
                    ReleaseAction(ref _inputAim);
                    break;
            }
            DoInputReleaseAction(actionName);
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
            ReleaseAction(ref _inputJump);
            ReleaseAction(ref _inputDrop);
        }

        private void ReadInputs()
        {
            _inputDirection = new Vector3(_axisMove.x, 0.0f, _axisMove.y).normalized;


            if (_inputDirection != Vector3.zero)
                {
                    _targetSpeed = _inputSprint ? MovementAsset.SprintSpeed : MovementAsset.MoveSpeed;
                }
                else
                {
                    _targetSpeed = 0.0f;
                }

            _inputMagnitude = IsCurrentDeviceMouse() ? _axisMove.magnitude : 1f;

            // ToggleEquip();
            // HoldADS();

            // if (CanDrop())
            // {
            //     _inventorySystem.DropItem(dropItem, 7);
            // }

            // _inventorySystem.ToggleInventoryPanelUI(Inventory);
        }

        /// <summary>
        /// 각 액션 기능들을 예외처리
        /// </summary>
        private void DoInputTriggerAction(string actionName)
        {
            // Toggle 방식

            if (actionName == "Equip")
            {
                if (_inputEquip)
                {
                    FollowCamera.CameraSetup(EquipCameraSetup);
                }
                else
                {
                    FollowCamera.CameraSetup(UnEquipCameraSetup);
                }
                UpdateAnimEquip(_inputEquip);
            }

            if (actionName == "Inventory")
            {
                _inventorySystem.ToggleInventoryPanelUI(_inputInventory);
                _cursorLocked = !_inputInventory;
                UpdateCursorState();
            }


            // Press 방식
            if (actionName == "Drop")
            {
                _inventorySystem.DropItem(dropItem, 7);
            }
        }

        private void DoInputReleaseAction(string actionName)
        {

        }

        /// <summary>
        /// Update에서 호출
        /// </summary>
        private void UpdateUpperBody()
        {
            if (_inputEquip)
            {
                if (_inputAim)
                {
                    FollowCamera.CameraSetup(AimCameraSetup);
                }
                else
                {
                    FollowCamera.CameraSetup(EquipCameraSetup);
                }
                UpdateAnimAimDownSight(_inputAim);
            }
            else
            {
                FollowCamera.CameraSetup(UnEquipCameraSetup);
            }
        }
        


        private void UpdateCursorState()
        {
            Cursor.visible = !_cursorLocked;
            Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}