using UnityEngine;

namespace David6.ShooterFramework
{
    /// <summary>
    /// 아직 사용 안함, 애니메이션과 카메라 세팅 구분에 필요할 수 있음
    /// </summary>
    enum eUpperBodyState
    {
        UnEquip,
        Equip
    }
    public partial class DaveController : MonoBehaviour
    {
        private eUpperBodyState _prevUperBodyState = eUpperBodyState.UnEquip;
        private eUpperBodyState _currentUperBodyState = eUpperBodyState.UnEquip;

        private bool Grounded = false;
        private bool _engaged = false;
        
        public bool IsCurrentDeviceMouse() => InputManager.Instance.IsCurrentDeviceMouse;

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - MovementAsset.GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, MovementAsset.GroundedRadius, MovementAsset.GroundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            UpdateAnimGrounded(Grounded);
        }

        private bool CanJump()
        {
            return _inputJump && _jumpTimeoutDelta <= 0.0f ? true : false;
        }
    }
}
