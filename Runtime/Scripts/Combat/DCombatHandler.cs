using System;
using System.Collections.Generic;
using David6.ShooterCore.Item;
using David6.ShooterCore.Item.Weapon;
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


        public event Action<bool, int> OnCountingRounds;
        public event Action<int> OnCountingAmmunition;
        public event Action<float> OnSpreedChanged;

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
        const string FOCUS_KEY = "Focus";
        const float FOCUS_DURATION = 5.5f;
        public float GetFocusDuration => FOCUS_DURATION;
        public bool IsFocus { get; private set; } = false;
        public bool IsAiming { get; private set; } = false;
        public event Action OnFocusActive;
        public event Action OnFocusInactive;
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

        public void AimStart()
        {
            IsAiming = true;
            RequestFocus();
            LockFocus();
        }
        public void AimStop()
        {
            IsAiming = false;
            UnlockFocus();
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

            // Data Update
            _activeSlot = slot;
            _activeWeapon = _weaponInstances[slot];

            // Anim Update
            CurrentFireRate = _activeWeapon.FrameHandler.WeaponData.FireRate;
            _context.AnimatorProvider.SetFireRate(CurrentFireRate);
            _activeWeapon.FrameHandler.OnConsumeAmmo += OnCountingRounds;

            // UI Update
            OnCountingRounds?.Invoke(IsChamberLoaded(), GetCurrentRounds());
            int ammoCount = _context.AmmunitionCount(_activeWeapon.FrameHandler.WeaponData.AmmoType);
            OnCountingAmmunition?.Invoke(ammoCount);
            _activeWeapon.FrameHandler.OnSpreedChanged += OnSpreedChanged;
            Log.WhatHappend("Accuracy 바인딩함");
        }
        void EquipNewWeapon(EDGearSlot slot, DGear item)
        {
            // 이전 무기 비활성
            InactivePrevWeapon();

            var instance = new DWeaponInstance();
            _weaponInstances[slot] = instance;

            BuildWeaponInstance(item, instance);
        }
        void RepaceWeapon(EDGearSlot slot, DGear item)
        {
            var instance = _weaponInstances[slot];
            if (instance.Prefab != null)
            {
                instance.FrameHandler.OnConsumeAmmo -= OnCountingRounds;
                _context.DestroyPrefab(instance.Prefab);
            }
            BuildWeaponInstance(item, instance);
        }
        void SwapWeapon(EDGearSlot slot, DGear item)
        {
            InactivePrevWeapon();

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
        }
        private void InactivePrevWeapon()
        {
            var currentInstance = _weaponInstances[_activeSlot];
            if (currentInstance?.Prefab != null)
            {
                // 이벤트 해제
                currentInstance.FrameHandler.OnConsumeAmmo -= OnCountingRounds;
                currentInstance.Prefab.SetActive(false);
            }
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
            //instance.FrameHandler.InsertMagazine();
            instance.FrameHandler.ConsumeAmmo();
        }

        public void TryShoot()
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return;

            _context.AnimatorProvider.PlayFire();
        }

        public void OnEjectMagazine(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeaponHandler();
            if (!success) return;

            _context.EjectRumble();
            _context.StopRumble(0.25f);

            currentWeapon.EjectMagazine();
            OnCountingRounds?.Invoke(IsChamberLoaded(), GetCurrentRounds());

        }
        public void OnInsertMagazine(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeaponHandler();
            if (!success) return;

            _context.InsertRumble();
            _context.StopRumble(0.25f);

            var weaponData = currentWeapon.WeaponData;
            currentWeapon.InsertMagazine(_context.ConsumeAmmunition(weaponData.AmmoType, weaponData.MagazineCapacity));
            OnCountingAmmunition?.Invoke(_context.AmmunitionCount(weaponData.AmmoType));
            OnCountingRounds?.Invoke(IsChamberLoaded(), GetCurrentRounds());
        }

        public void OnChamberLoad(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeaponHandler();
            if (!success) return;

            _context.ChamberLoadRumble();
            _context.StopRumble(0.25f);

            currentWeapon.ChamberLoad();
        }


        public void OnShoot(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeaponHandler();
            if (!success) return;

            Vector3 intendedPoint = CalculateIntendedPoint();

            if (currentWeapon.Shoot(intendedPoint, IsAiming))
            {
                _context.FireRoundRumble();
            }
            else
            {
                _context.EmptyChamberRumble();
            }

            _context.StopRumble(0.1f);
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

        public bool IsChamberLoaded()
        {
            if (_activeWeapon == null) return false;
            return _activeWeapon.FrameHandler.ChamberLoaded;
        }
        public int GetCurrentRounds()
        {
            if (_activeWeapon == null) return -1;
            return _activeWeapon.FrameHandler.CurrentRounds;
        }

        /// <summary>
        /// 현재 무기 존재 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool EquippedWeapon()
        {
            var weapon = _weaponInstances[_activeSlot];

            return (weapon == null) ? false : true;            
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

        /// <summary>
        /// 핸들러 반환버전
        /// </summary>
        /// <returns></returns>
        public (bool success, DFrameHandler handler) TryGetWeaponHandler()
        {
            var weapon = _weaponInstances[_activeSlot];
            if (weapon == null)
            {
                Log.WhatHappend("무기 업승");
                return (false, null);
            }

            return (true, weapon.FrameHandler);
        }

        public DFrameHandler GetWeaponHandler()
        {
            return _activeWeapon.FrameHandler;
        }

    }

}