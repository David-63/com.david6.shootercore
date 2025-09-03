using System;
using David6.ShooterCore.Combat;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDCombatHandler
    {
        bool IsFocus { get; }
        float GetFocusDuration { get; }

        event Action OnFocusActive;
        event Action OnFocusInactive;

        void OnUpdate();

        void RequestFocus(float duration);
        void LockFocus();
        void UnlockFocus();
        void CancelFocus();

        float CurrentFireRate { get; }
        void SetWeapon(EDGearType type, DGearData data);
        bool TryFire();

        void OnEjectMagazine(AnimationEvent animationEvent);
        void OnInsertMagazine(AnimationEvent animationEvent);
        void OnChamberLoad(AnimationEvent animationEvent);

        bool IsArmed();
        bool IsChamberLoaded();
        DWeaponInstance GetWeapon();
        (bool success, DWeaponInstance weapon) TryGetWeapon();
    }
}