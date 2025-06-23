using UnityEngine;


namespace Dave6.ShooterFramework.Movement
{
    public partial class DaveMovementController : MonoBehaviour
    {
        #region 외부 제어 함수
        private bool HasMovementInput() => InputDirection != Vector3.zero;
        private bool IsForward() => LastDirection.z >= -0.8f;
        public bool CanJump() => Jump && _jumpTimeoutDelta <= 0.0f;
        public bool IsAirborne() => !Grounded && _fallTimeoutDelta <= 0.0f;
        public void PerformJump()
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _verticalSpeed = Mathf.Sqrt(MovementProfile.JumpHeight * -2f * MovementProfile.Gravity);
        }
        public void CalculateGroundSpeed()
        {
            float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

            if (Mathf.Abs(currentHorizontalSpeed - TargetSpeed) > _speedOffset)
            {
                float desired = TargetSpeed;

                _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, desired, Time.deltaTime * MovementProfile.SpeedChangeRate);
                _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
            }
            else
            {
                _currentSpeed = TargetSpeed;
            }
        }
        public void CalculateAirborneSpeed()
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, MovementProfile.MoveSpeed, MovementProfile.AirDeceleration * Time.deltaTime);
        }
        public void CalculateMoveDirection()
        {
            MoveDirection = Quaternion.Euler(0f, _cameraInfo.YawAngle, 0f) * LastDirection;
            MoveDirection.Normalize();
        }

        public void OverrideSpeed(float force) => _currentSpeed = force;
        #endregion

        #region 유틸 함수
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        #endregion
    }
}
