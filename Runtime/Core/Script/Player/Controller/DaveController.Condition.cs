using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class DaveController : MonoBehaviour
    {
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

    }
}
