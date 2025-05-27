using UnityEngine;

namespace David6.ShooterFramework
{
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
            return InputProvider.Jump && _jumpTimeoutDelta <= 0.0f ? true : false;
        }

        private bool CanDrop()
        {
            return InputProvider.Drop;
        }

        private bool UpperbodyStateChanged()
        {
            return _equiped == InputProvider.Equip ? false : true;
        }
    }
}
