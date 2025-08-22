using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDRigHandlerProvider : IDProvider
    {
        void SetupRigIK(DWeaponFrame weaponFrame);
    }
}