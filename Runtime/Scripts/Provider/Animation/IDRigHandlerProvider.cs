
using David6.ShooterCore.Item.Weapon;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDRigHandlerProvider : IDProvider
    {
        void SetupRigIK(DFrameHandler weaponFrame);
        void ActiveRig();
        void InactiveRig();
    }
}