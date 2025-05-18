using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class DaveController : MonoBehaviour
    {
		private bool _equiped = false;
        private bool _aimDownSight = false;
        private bool _fire = false;

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

            _inputMagnitude = InputProvider.IsCurrentDeviceMouse() ? InputProvider.Move.magnitude : 1f;

            ToggleEquip();
            HoldADS();
        }

        private void ToggleEquip()
        {
            if (UpperbodyStateChanged())
            {
                _equiped = InputProvider.Equip;
                SetUpperbodyCamera();
                UpdateAnimEquip(_equiped);
            }
        }

        private void HoldADS()
        {
            _aimDownSight = InputProvider.Aim;

            if (_aimDownSight)
            {
                if (_equiped)
                {
                    FollowCamera.CameraSetup(AimCameraSetup);
                    UpdateAnimAimDownSight(_aimDownSight);
                }
            }
            else
            {

                SetUpperbodyCamera();
            }
        }
    }
}