using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class PlayerController : ControllerBehaviour
    {
        #region Serialize fields

        [Header("Cinemachine")]		
        [Tooltip("1인칭 모드에서 카메라가 따라갈 Cinemachine Camera에 설정된 팔로우 대상입니다.")][SerializeField]
		private Transform FirstPersonCameraTarget;
		[Tooltip("1인칭 모드에서 카메라가 따라갈 Cinemachine Camera에 설정된 팔로우 대상입니다.")][SerializeField]
		private Transform ThirdPersonCameraTarget;
        [Tooltip("카메라를 오버라이드하는 추가 각도입니다. 잠긴 상태에서 카메라 위치를 미세하게 조정하는 데 유용합니다.")][SerializeField]
		private float CameraAngleOverride = 0.0f;
		[Tooltip("모든 축에서 카메라의 위치를 고정하기 위해 사용됩니다.")][SerializeField]
		private bool LockCameraPosition = false;
        [Range(0f, 0.4f)][SerializeField]
		private float FirstPersonCameraRotationSmoothing = 0.3f;

        #endregion
        
        #region Fields

		private bool _firstPerson = true;
        private bool _defaultCamera;		
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;
        private Transform _cameraTarget;
		private float _bodyRotate = 0f;

        #endregion

        public bool FirstPerson
		{
			get => _firstPerson;
			set
			{
				if (_firstPerson == value) return;
				SetFirstPerson(value);
			}
		}

        private static float NormalizeAngle(float angle)
		{
			while (angle > 180f)
				angle -= 360f;
			while (angle < -180f)
				angle += 360f;
			return angle;
		}

		private static float NormalizeAnglePos(float angle) => (angle + 360f) % 360f;
		
		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		public static float GetAngleBetweenAngles(float angleA, float angleB)
		{
			float clockwiseAngle = NormalizeAngle(angleB - angleA);
			float counterClockwiseAngle = NormalizeAngle(angleA - angleB);

			if (Mathf.Abs(clockwiseAngle) < Mathf.Abs(counterClockwiseAngle))
			{
				return clockwiseAngle;
			}

			return counterClockwiseAngle;
		}

		private void InitializeCamera()
		{
			_cameraTarget = _firstPerson ? FirstPersonCameraTarget : ThirdPersonCameraTarget;
			
			CinemachineInstance.Instance.Follow(_cameraTarget);
            CinemachineInstance.Instance.FirstPerson = _firstPerson;
			
			_defaultCamera = _firstPerson;
			_cinemachineTargetYaw = _cameraTarget.rotation.eulerAngles.y;
		}
		
		private void SetFirstPerson(bool value)
		{
			_firstPerson = value;
			_cameraTarget = value ? FirstPersonCameraTarget : ThirdPersonCameraTarget;
				
				
			Vector3 rot = _cameraTarget.rotation.eulerAngles;
			_cameraTarget.rotation = Quaternion.Euler(rot.x, _cinemachineTargetYaw, rot.z);

			CinemachineInstance.Instance.FirstPerson = value;
            CinemachineInstance.Instance.Follow(_cameraTarget);
		}
		
		private void CameraRotation(bool lockInput = false)
		{
			// smooths POV hands
			//if (_firstPerson)

			bool AllowInput = !lockInput;

			var oldYaw = _cinemachineTargetYaw;
			var oldPitch = _cinemachineTargetPitch;

			FirstPerson = _defaultCamera ? !_playerCharacter.IsCameraSwitch() : _playerCharacter.IsCameraSwitch();
			//float deltaTimeMultiplier = IsCurrentDeviceMouse ? Time.deltaTime : 1.0f;

			Vector2 playerInput = _playerCharacter.GetInputLook();
			//playerInput *= deltaTimeMultiplier;
			// 좌우 회전 입력
			if (AllowInput)
            {
                _cinemachineTargetYaw += playerInput.x;
            }

			if (_firstPerson)
			{
				var euler = NormalizeAnglePos(transform.localRotation.eulerAngles.y);

				const float maxDifference = 90f;
				float difference = GetAngleBetweenAngles(_cinemachineTargetYaw, euler);

				if (difference > maxDifference)
				{
					_bodyRotate += difference - maxDifference;
					_bodyRotate = NormalizeAngle(_bodyRotate);

					// apply rotation
					transform.rotation = Quaternion.Euler(0, _bodyRotate, 0);
				}
				else if (difference < -maxDifference)
				{
					_bodyRotate += difference + maxDifference;
					_bodyRotate = NormalizeAngle(_bodyRotate);

					// apply rotation
					transform.rotation = Quaternion.Euler(0, _bodyRotate, 0);
				}

				_cinemachineTargetYaw = NormalizeAnglePos(_cinemachineTargetYaw);
			}

			// 상하 회전 입력
			if (AllowInput)
			{
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch - playerInput.y, _playerManager.GetBottomClamp(), _playerManager.GetTopClamp());
			}
			else
			{
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _playerManager.GetBottomClamp(), _playerManager.GetTopClamp());
			}

			// Cinemachine will follow this target
			if (!_firstPerson)
			{
				_cameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
			}
			else
			{
				_cameraTarget.rotation = Quaternion.Lerp( _cameraTarget.rotation, Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0),
					Time.deltaTime * (FirstPersonCameraRotationSmoothing / Time.smoothDeltaTime));
			}

			// update animator if using character
			
		}
    }
}