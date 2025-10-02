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
        event Action<bool, int> OnCountingRounds;
        event Action<int> OnCountingAmmunition;
        public event Action<float> OnSpreedChanged;


        void OnUpdate();

        void RequestFocus();
        void LockFocus();
        void UnlockFocus();
        void CancelFocus();

        void AimStart();
        void AimStop();

        float CurrentFireRate { get; }
        void EquipWeapon(EDGearSlot slot, DGear item);
        void TryShoot();

        void OnEjectMagazine(AnimationEvent animationEvent);
        void OnInsertMagazine(AnimationEvent animationEvent);
        void OnChamberLoad(AnimationEvent animationEvent);
        void OnShoot(AnimationEvent animationEvent);

        bool IsChamberLoaded();
        int GetCurrentRounds();

        bool EquippedWeapon();
        DWeaponInstance GetWeapon();
        (bool success, DWeaponInstance weapon) TryGetWeapon();
        (bool success, DFrameHandler handler) TryGetWeaponHandler();
    }
}