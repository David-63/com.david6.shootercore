using UnityEngine;

namespace David6.ShooterFramework
{
    [CreateAssetMenu(fileName = "PlayerManager", menuName = "Service/Player Manager")]
    public class PlayerManager : ScriptableObject, IPlayerManager
    {
        #region Fields serialized

        [Header("Movement Value")]
        [Tooltip("기본 속력 m/s")] [SerializeField]
        private float _walkSpeed = 2.0f;
        [Tooltip("달리기 속력 m/s")] [SerializeField]
        private float _sprintSpeed = 5.0f;
        [Tooltip("가속, 감속 변화량")] [SerializeField]
        private float _speedChangeRate = 5.0f;
        [Tooltip("캐릭터 회전 속력")] [SerializeField]
		private float _rotationSpeed = 1.0f;

        [Space(10)]
        [Header("Add Force")]
        [Tooltip("점프 높이")] [SerializeField]
        private float _jumpHeight = 3.0f;
        [Tooltip("닷지 속력")] [SerializeField]
        private float _dodgeForce = 10.0f;

        [Space(10)]
        [Header("Ground Check")]
        [Range(0.0f, 1.0f)]
        [Tooltip("공중 제어 개수 [0 ~ 1]. 0에 가까울수록 공중에서 입력영향이 적어짐")] [SerializeField]
        private float _airControlFactor = 0.2f;
        [Tooltip("바닥충돌에 사용할 레이어 타입")] [SerializeField]
		private LayerMask _groundLayers;


		[Header("Cinemachine")]
		[Tooltip("Cinemachine 가상 카메라가 따라가는 대상(GameObject)을 설정합니다.")] [SerializeField]
		private GameObject _cinemachineCameraTarget;
		[Tooltip("카메라를 위로 움직일 수 있는 최대 각도를 도(degrees)로 정의합니다. 기본값은 90.0f로 설정되어 있습니다.")] [SerializeField]
		private float _topClamp = 80.0f;
		[Tooltip("카메라를 아래로 움직일 수 있는 최대 각도를 지정합니다.")] [SerializeField]
		private float _bottomClamp = -80.0f;

        #endregion

        #region Fields

        private CharacterBehaviour _playerCharacter;

        #endregion

        #region 프로퍼티

        public CharacterBehaviour GetPlayerCharacter()
        {
            if (_playerCharacter == null)
            {
                _playerCharacter = UnityEngine.Object.FindAnyObjectByType<CharacterBehaviour>();
            }
            
            return _playerCharacter;
        }

        public float GetWalkSpeed() => _walkSpeed;
        public float GetSprintSpeed() => _sprintSpeed;
        public float GetSpeedChangeRate() => _speedChangeRate;
        public float GetRotationSpeed() => _rotationSpeed;

        public float GetJumpHeight() => _jumpHeight;
        public float GetDodgeForce() => _dodgeForce;
        public float GetAirControlFactor() => _airControlFactor;

        public LayerMask GetGroundLayer() => _groundLayers;

        public GameObject GetCinemachineCameraTarget() => _cinemachineCameraTarget;
		public float GetTopClamp() => _topClamp;
		public float GetBottomClamp() => _bottomClamp;

        #endregion
    }
}
