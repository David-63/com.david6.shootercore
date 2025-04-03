using UnityEngine;

namespace David6.ShooterFramework
{
    public abstract class ControllerBehaviour : MonoBehaviour
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

