using System;
using System.Collections.Generic;
using David6.ShooterCore.Item;
using David6.ShooterCore.Item.Weapon;
using David6.ShooterCore.Look;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.Combat
{
    public class DWeaponInstance
    {
        public GameObject Prefab;
        public DFrameHandler FrameHandler;
    }

    public class DCombatHandler : IDCombatHandler
    {
        IDContextProvider _context;
        Dictionary<EDGearSlot, DWeaponInstance> _weaponInstances = new();
        EDGearSlot _activeSlot;
        DWeaponInstance _activeWeapon;

        // 제어 변수
        public float CurrentFireRate { get; private set; }
        const float MAX_DISTANCE = 500.0f;
        LayerMask _hitMask;

        // focus

        #region Focus value
        const string FOCUS_KEY = "Focus";
        const float FOCUS_DURATION = 5.5f;
        public float GetFocusDuration => FOCUS_DURATION;
        public bool IsFocus { get; private set; } = false;

        public event Action OnFocusActive;
        public event Action OnFocusInactive;

        #endregion

        public Dictionary<EDGearSlot, GameObject> EquipmentGear = new();


        public DCombatHandler(IDContextProvider context)
        {
            _context = context;

            for (EDGearSlot type = EDGearSlot.Primary; type <= EDGearSlot.Sidearm; ++type)
            {
                _weaponInstances[type] = null;
            }

            _hitMask.value = 1;
        }

        public void OnUpdate()
        {
            if (!IsFocus) return;

            if (_context.CooldownProvider.IsReady(FOCUS_KEY))
            {
                IsFocus = false;
                OnFocusInactive?.Invoke();
            }
        }

        #region Focus Control
        public void RequestFocus()
        {
            IsFocus = true;
            _context.CooldownProvider.StartCooldown(FOCUS_KEY, FOCUS_DURATION);
            OnFocusActive?.Invoke();
        }
        public void LockFocus()
        {
            _context.CooldownProvider.LockCooldown(FOCUS_KEY);
        }
        public void UnlockFocus()
        {
            _context.CooldownProvider.UnlockCooldown(FOCUS_KEY);
        }
        public void CancelFocus()
        {
            IsFocus = false;
            _context.CooldownProvider.CancelCooldown(FOCUS_KEY);
            OnFocusInactive?.Invoke();
        }
        #endregion

        public void EquipWeapon(EDGearSlot slot, DGear item)
        {
            // EquipWeapon | 새 무기를 비여있는 슬롯에 장착
            if (!_weaponInstances.TryGetValue(slot, out var instance) || instance == null)
            {
                EquipNewWeapon(slot, item);                
            }
            // ReplaceWeapon | 새 무기를 동일한 슬롯에 대체
            else if (slot == _activeSlot)
            {
                RepaceWeapon(slot, item);
            }
            // SwapWeapon | 다른 슬롯으로 무기 교체
            else
            {
                SwapWeapon(slot, item);
            }
        }
        void EquipNewWeapon(EDGearSlot slot, DGear item)
        {
            var currentInstance = _weaponInstances[_activeSlot];
            if (currentInstance?.Prefab != null)
            {
                currentInstance.Prefab.SetActive(false);
            }

            var instance = new DWeaponInstance();
            _weaponInstances[slot] = instance;

            BuildWeaponInstance(item, instance);
            _activeSlot = slot;
            _activeWeapon = instance;
        }
        void RepaceWeapon(EDGearSlot slot, DGear item)
        {
            var instance = _weaponInstances[slot];
            if (instance.Prefab != null)
            {
                _context.DestroyPrefab(instance.Prefab);
            }
            BuildWeaponInstance(item, instance);
            _activeWeapon = instance;
        }
        void SwapWeapon(EDGearSlot slot, DGear item)
        {
            var currentInstance = _weaponInstances[_activeSlot];
            if (currentInstance?.Prefab != null)
            {
                currentInstance.Prefab.SetActive(false);
            }

            if (!_weaponInstances.TryGetValue(slot, out var targetInstance) || targetInstance == null)
            {
                targetInstance = new DWeaponInstance();
                _weaponInstances[slot] = targetInstance;
                BuildWeaponInstance(item, targetInstance);
            }

            if (targetInstance.Prefab != null)
            {
                targetInstance.Prefab.SetActive(true);
            }

            var weaponData = item.BaseData as DWeaponData;
            CurrentFireRate = weaponData.FireRate;
            _context.AnimatorProvider.SetFireRate(weaponData.FireRate);

            _activeSlot = slot;
            _activeWeapon = targetInstance;
        }
        void BuildWeaponInstance(DGear item, DWeaponInstance instance)
        {
            var weaponData = item.BaseData as DWeaponData;

            if (instance.Prefab == null)
            {
                instance.Prefab = _context.AssembleWeapon(item, _context.WeaponSocket);
            }

            instance.FrameHandler = instance.Prefab.GetComponent<DFrameHandler>();
            instance.FrameHandler.Initialize(_context, weaponData);
            instance.FrameHandler.InsertMagazine();
            instance.FrameHandler.ChamberLoad();
            CurrentFireRate = weaponData.FireRate;
            _context.AnimatorProvider.SetFireRate(weaponData.FireRate);
        }

        public void TryShoot()
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return;

            _context.AnimatorProvider.PlayFire();
        }

        Vector3 CalculateIntendedPoint()
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray aimRay = _context.CameraHandlerProvider.LookCamera.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(aimRay, out var camHit, MAX_DISTANCE, _hitMask))
            {
                return camHit.point;
            }

            return aimRay.GetPoint(MAX_DISTANCE);
        }

        public void OnEjectMagazine(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return;
            Vector3 intendedPoint = CalculateIntendedPoint();

            if (currentWeapon.FrameHandler.Shoot(intendedPoint))
            {
                _context.FireRoundRumble();
            }
            else
            {
                _context.EmptyChamberRumble();                
            }
            _context.StopRumble(0.25f);
            _context.EjectRumble();
            _context.StopRumble(0.25f);

            currentWeapon.FrameHandler.EjectMagazine();
        }
        public void OnInsertMagazine(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return;

            _context.InsertRumble();
            _context.StopRumble(0.25f);

            currentWeapon.FrameHandler.InsertMagazine();
        }

        public void OnChamberLoad(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return;

            _context.ChamberLoadRumble();
            _context.StopRumble(0.25f);

            currentWeapon.FrameHandler.ChamberLoad();
        }

        public void OnShot(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return;

            Vector3 intendedPoint = CalculateIntendedPoint();

            if (currentWeapon.FrameHandler.Shoot(intendedPoint))
            {
                _context.FireRoundRumble();
            }
            else
            {
                _context.EmptyChamberRumble();                
            }
            _context.StopRumble(0.25f);
        }

        public bool IsChamberLoaded()
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return false;

            return currentWeapon.FrameHandler.IsChamberLoaded();
        }
        public int GetCurrentRounds()
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return -1;

            return currentWeapon.FrameHandler.GetCurrentRounds();
        }

        /// <summary>
        /// 현재 무기 존재 여부 체크 + 로그, 호출자에게 바로 반환
        /// </summary>
        /// <returns>null이면 무기 없음, 아니면 현재 무기 반환</returns>
        public DWeaponInstance GetWeapon()
        {
            var weapon = _weaponInstances[_activeSlot];
            if (weapon == null)
            {
                Log.WhatHappend("무기 업승");
            }
            return weapon;
        }

        /// <summary>
        /// 무기 체크 후 성공 여부와 weapon 반환 (튜플 버전)
        /// </summary>
        public (bool success, DWeaponInstance weapon) TryGetWeapon()
        {
            var weapon = _weaponInstances[_activeSlot];
            if (weapon == null)
            {
                Log.WhatHappend("무기 업승");
                return (false, null);
            }
            return (true, weapon);
        }

    }

}