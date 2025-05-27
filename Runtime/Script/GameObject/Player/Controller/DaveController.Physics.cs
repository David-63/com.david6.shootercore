using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class DaveController : MonoBehaviour
    {
		private float _verticalVelocity;

        // timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		private const float _terminalVelocity = 53.0f;

        private void UpdateGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = MovementAsset.FallTimeout;

                // update animator if using character
                UpdateAnimJump(false);
                UpdateAnimFreeFall(false);

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = MovementAsset.JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    UpdateAnimFreeFall(true);
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += MovementAsset.Gravity * Time.deltaTime;
            }
        }
    }
}
