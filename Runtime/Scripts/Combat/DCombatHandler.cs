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
        public DGear Gear;
        public GameObject Prefab;
        public DFrameHandler FrameHandler;
    }

    public class DCombatHandler : IDCombatHandler
    {
        // 무기나 장비를 알고 있어야함

        // 외부에서 호출함
        IDContextProvider _context;
        Dictionary<EDGearSlot, DWeaponInstance> _weapons = new();
        EDGearSlot _currentType;

        // 제어 변수
        public float CurrentFireRate { get; private set; }
        const float MAX_DISTANCE = 500.0f;
        LayerMask _hitMask;

        // focus

        const string FOCUS_KEY = "Focus";
        const float FOCUS_DURATION = 5.5f;
        public float GetFocusDuration => FOCUS_DURATION;
        public bool IsFocus { get; private set; } = false;

        public event Action OnFocusActive;
        public event Action OnFocusInactive;

        public DCombatHandler(IDContextProvider context)
        {
            _context = context;

            for (EDGearSlot type = EDGearSlot.Primary; type <= EDGearSlot.Sidearm; ++type)
            {
                _weapons[type] = null;
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
        public void RequestFocus(float duration = FOCUS_DURATION)
        {
            IsFocus = true;
            _context.CooldownProvider.StartCooldown(FOCUS_KEY, duration);
            OnFocusActive?.Invoke();
            _context.CameraHandlerProvider.SetLayerActive(EDCameraLayer.Focus, true);
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
            _context.CameraHandlerProvider.SetLayerActive(EDCameraLayer.Focus, false);
        }
        #endregion

        public void SetWeapon(EDGearSlot type, DGear data)
        {
            // Weapon 인스턴스 등록
            InstanceSetup(type, data);

            _context.AnimatorProvider.SetFireRate(CurrentFireRate);
        }

        private void InstanceSetup(EDGearSlot type, DGear item)
        {
            if (!_weapons.TryGetValue(type, out var instance) || instance == null)
            {
                instance = _weapons[type] = new DWeaponInstance();
            }

            _currentType = type;
            instance.Gear = item;

            if (instance.Prefab == null)
            {
                //AssembleWeapon
                instance.Prefab = _context.AssembleWeapon(item, _context.WeaponSocket);
            }

            instance.FrameHandler = instance.Prefab.GetComponent<DFrameHandler>();

            instance.FrameHandler.Initialize(_context, item);
        }

        public void TryFire()
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
                _context.StopRumble(0.1f);
            }

            _context.AnimatorProvider.SetFire();
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
            
            // 게임패드
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

        public bool IsChamberLoaded()
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return false;

            return currentWeapon.FrameHandler.IsChamberLoaded();
        }

        /// <summary>
        /// 현재 무기 존재 여부 체크 + 로그, 호출자에게 바로 반환
        /// </summary>
        /// <returns>null이면 무기 없음, 아니면 현재 무기 반환</returns>
        public DWeaponInstance GetWeapon()
        {
            var weapon = _weapons[_currentType];
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
            var weapon = _weapons[_currentType];
            if (weapon == null)
            {
                Log.WhatHappend("무기 업승");
                return (false, null);
            }
            return (true, weapon);
        }

    }

}