using UnityEngine;

namespace David6.ShooterFramework
{
    public abstract class CharacterBehaviour : MonoBehaviour
    {
        #region 유니티 기본 함수

        protected virtual void Awake(){}
        protected virtual void Start(){}
        protected virtual void Update(){}
        protected virtual void LateUpdate(){}

        #endregion

        #region 프로퍼티

        /// <summary>
        /// 달리기 상태 반환
        /// </summary>
        /// <returns> bool </returns>
        public abstract bool IsSprinting();

        /// <summary>
        ///  조준 상태 반환
        /// </summary>
        /// <returns> bool </returns>
        public abstract bool IsAiming();
        /// <summary>
        ///  점프 상태 반환
        /// </summary>
        /// <returns> bool </returns>
        public abstract bool IsJumping();

        /// <summary>
        /// 커서 잠금상태 반환
        /// </summary>
        /// <returns></returns>
		public abstract bool IsCursorLocked();

        /// <summary>
        /// 카메라 시점 상태 반환
        /// </summary>
        /// <returns></returns>
        public abstract bool IsCameraSwitch();


        /// <summary>
        /// 이동 입력값 반환
        /// </summary>
        /// <returns> Vector2 </returns>
        public abstract Vector2 GetInputMovement();

        /// <summary>
        /// 시점 입력값 반환
        /// </summary>
        /// <returns> Vector2 </returns>
        public abstract Vector2 GetInputLook();

        /// <summary>
        /// 조준점이 표시되어야 하는지 여부를 반환.
        /// </summary>
        public abstract bool IsCrosshairVisible();

        #endregion
    }
}