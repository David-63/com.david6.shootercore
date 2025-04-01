
using UnityEngine;

namespace David6.ShooterFramework
{
    public interface IPlayerManager : IGameManager
    {
        CharacterBehaviour GetPlayerCharacter();
        float GetWalkSpeed();
        float GetSprintSpeed();
        float GetSpeedChangeRate();
        float GetRotationSpeed();

        float GetJumpHeight();
        float GetDodgeForce();
        float GetAirControlFactor();

        LayerMask GetGroundLayer();

        GameObject GetCinemachineCameraTarget();
		float GetTopClamp();
		float GetBottomClamp();
    }
}
