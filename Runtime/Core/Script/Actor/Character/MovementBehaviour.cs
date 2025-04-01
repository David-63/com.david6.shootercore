using UnityEngine;

namespace David6.ShooterFramework
{
    /// <summary>
    /// 움직임의 핵심 컴포넌트
    /// </summary>
    public abstract class MovementBehaviour : MonoBehaviour
    {
        #region 유니티 기본 함수

        protected virtual void Awake(){}
        protected virtual void Start(){}        
        protected virtual void FixedUpdate(){}
        protected virtual void Update(){}
        protected virtual void LateUpdate(){}

        #endregion

    }
}

