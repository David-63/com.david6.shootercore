using David6.ShooterCore.Combat;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDCombatHandler
    {
        public DWeaponInstance GetCurrentWeapon { get; }

        void SetWeapon(EDGearType type, DGearData data);
        bool Fire();

        void OnEjectMagazine(AnimationEvent animationEvent);
        void OnInsertMagazine(AnimationEvent animationEvent);
        void OnChamberLoad(AnimationEvent animationEvent);
    }
}