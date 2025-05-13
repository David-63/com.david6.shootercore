using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class DaveController : MonoBehaviour
    {
		private bool _equiped = false;
        private bool _aimDownSight = false;
        private bool _fire = false;

        public const float _equipDelayTime = 0.2f;
        public const float _fireDelayTime = 0.02f;

        private float _lastEquipedInput = 0.0f;
        private float _lastFireLastInput = 0.0f;




        private void ReadInputs()
        {
			_inputDirection = new Vector3(InputProvider.Move.x, 0.0f, InputProvider.Move.y).normalized;
            if (_inputDirection != Vector3.zero)
            {
                _targetSpeed = InputProvider.Sprint ? MovementAsset.SprintSpeed : MovementAsset.MoveSpeed;
            }
            else
            {
                _targetSpeed = 0.0f;
            }
            
            _inputMagnitude = InputProvider.IsCurrentDeviceMouse ? InputProvider.Move.magnitude : 1f;

            // 인풋 TimeoutDelta 구현하기?

            ToggleEquip();
            HoldADS();
        }

        private void ToggleEquip()
        {
            if (!InputProvider.Equip) return;
            if (Time.time >= _lastEquipedInput + _equipDelayTime)
            {
                _lastEquipedInput = Time.time;
                _equiped = !_equiped;
                UpdateAnimEquip(_equiped);
            }
        }

        private void HoldADS()
        {
            _aimDownSight = InputProvider.Aim;            
            UpdateAnimAimDownSight(_aimDownSight);
        }
    }
}