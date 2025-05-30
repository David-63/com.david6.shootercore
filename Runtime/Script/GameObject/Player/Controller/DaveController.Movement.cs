using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class DaveController : MonoBehaviour
    {
        private float _speed;
        private const float _speedOffset = 0.1f;
		private const float _threshold = 0.01f;
        
        private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;


        private float _targetSpeed;
        private Vector3 _inputDirection;
        private float _inputMagnitude;

        private void Movement()
        {
            ReadInputs();
            CalculateSpeed();
            PerformMovement();

            if (CanJump())
            {
                PerformJump();
            }


        }
        
        private void CalculateSpeed()
        {
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            if (Mathf.Abs(currentHorizontalSpeed - _targetSpeed) > _speedOffset)
            {
                float desired = _targetSpeed * _inputMagnitude;

                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, desired, Time.deltaTime * MovementAsset.SpeedChangeRate);
                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = _targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, _targetSpeed, Time.deltaTime * MovementAsset.SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            UpdateAnimSpeed(_inputMagnitude);
            UpdateAnimDirection(_axisMove);
        }

        private void PerformMovement()
        {
            // 방향 정렬
            Quaternion cameraRotation = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f);
			transform.rotation = cameraRotation;

            // 월드 좌표로 입력 벡터 변환 후 실제 이동
            Vector3 moveDirection = cameraRotation * _inputDirection;

            Vector3 velocity = moveDirection * _speed + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }

        private void CameraRotation()
		{
            if (!_cursorLocked)
            {
                _axisLook = Vector3.zero;
            }

			// if there is an input and camera position is not fixed
            if (_axisLook.sqrMagnitude >= _threshold && !MovementAsset.LockCameraPosition)
            {
                _cinemachineTargetYaw += _axisLook.x * Time.deltaTime;
                _cinemachineTargetPitch += _axisLook.y * Time.deltaTime;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, MovementAsset.BottomClamp, MovementAsset.TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + MovementAsset.CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
		}

        private void PerformJump()
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _verticalVelocity = Mathf.Sqrt(MovementAsset.JumpHeight * -2f * MovementAsset.Gravity);
            // update animator if using character
            UpdateAnimJump(true);
        }

        private void EquipAction()
        {
            
        }
    }
}
