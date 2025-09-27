using System;
using David6.ShooterCore.Combat;
using David6.ShooterCore.Item;
using David6.ShooterCore.Item.Weapon;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDCombatHandler
    {
        bool IsFocus { get; }
        float GetFocusDuration { get; }

        event Action OnFocusActive;
        event Action OnFocusInactive;
        event Action<bool, int> OnConsumeRounds;
        event Action<int> OnCountingAmmunition;

        void OnUpdate();

        void RequestFocus();
        void LockFocus();
        void UnlockFocus();
        void CancelFocus();

        float CurrentFireRate { get; }
        void EquipWeapon(EDGearSlot slot, DGear item);
        void TryShoot();

        void OnEjectMagazine(AnimationEvent animationEvent);
        void OnInsertMagazine(AnimationEvent animationEvent);
        void OnChamberLoad(AnimationEvent animationEvent);
        void OnShoot(AnimationEvent animationEvent);

        bool IsChamberLoaded();
        int GetCurrentRounds();

        DWeaponInstance GetWeapon();
        (bool success, DWeaponInstance weapon) TryGetWeapon();
        (bool success, DFrameHandler handler) TryGetWeaponHandler();
    }
}