using System;
using System.Collections;
using System.Collections.Generic;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.FX;
using David6.ShooterCore.Pool;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

namespace David6.ShooterCore.Combat
{
    public class DWeaponInstance
    {
        public DGearData GearData;
        public GameObject WeaponObject;
        public DWeaponFrame WeaponFrame;
    }

    public class DCombatHandler : IDCombatHandler
    {
        // 무기나 장비를 알고 있어야함

        // 외부에서 호출함
        IDContextProvider _context;
        Dictionary<EDGearType, DWeaponInstance> _weapons = new();
        EDGearType _currentType;

        // 제어 변수
        public float CurrentFireRate { get; private set; }
        const float MAX_DISTANCE = 500.0f;
        LayerMask _hitMask;



        // 총기 관련 변수

        bool _chamberLoaded = false;
        int _reserveAmmo; // maxReserveAmmo (_magazineCapacity * 4)
        int _currentMagazine; // _magazineCapacity


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

            for (EDGearType type = EDGearType.Primary; type <= EDGearType.Sidearm; ++type)
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

        public void SetWeapon(EDGearType type, DGearData data)
        {
            // Weapon 인스턴스 등록
            if (!_weapons.TryGetValue(type, out var instance) || instance == null)
            {
                instance = _weapons[type] = new DWeaponInstance();
            }

            _currentType = type;
            instance.GearData = data;

            if (instance.WeaponObject == null)
            {
                instance.WeaponObject = _context.MakeObject(data.GearPrefab, _context.WeaponSocket);
            }

            instance.WeaponFrame = instance.WeaponObject.GetComponent<DWeaponFrame>();
            CurrentFireRate = instance.WeaponFrame.FireRate;
            _context.AnimatorProvider.SetFireRate(CurrentFireRate);

            // 탄약 세팅 (나중에 딕셔너리로 캐싱하기)
            _chamberLoaded = true;
            _reserveAmmo = instance.WeaponFrame.MaxReserveAmmo;
            _currentMagazine = instance.WeaponFrame.MagazineCapacity - 1;
        }

        public bool TryFire()
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return false;

            if (!_chamberLoaded)
            {
                _context.EmptyChamberRumble();
                _context.StopRumble(0.1f);
                return false;
            }
            else
            {
                _context.FireRoundRumble();
            }

            _context.AnimatorProvider.SetFire();

            Vector3 intendedPoint = CalculateIntendedPoint();
            ScheduleHit(currentWeapon, intendedPoint);

            WeaponFireFX(currentWeapon, intendedPoint);

            ConsumeAmmo();

            return true;
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

        void ScheduleHit(DWeaponInstance currentWeapon, Vector3 intendedPoint)
        {
            Transform muzzleTransform = currentWeapon.WeaponFrame.MuzzleTransform;
            float travelDistance = Vector3.Distance(muzzleTransform.position, intendedPoint);
            float delay = travelDistance / currentWeapon.WeaponFrame.ProjectileSpeed;

            _context.ExecuteCoroutine(DelayedHit(muzzleTransform.position, intendedPoint, delay));
        }
        void WeaponFireFX(DWeaponInstance currentWeapon, Vector3 intendedPoint)
        {
            Transform muzzleTransform = currentWeapon.WeaponFrame.MuzzleTransform;
            _context.SpawnParticle(currentWeapon.WeaponFrame.FX_MuzzleFlash, muzzleTransform.position, muzzleTransform.rotation);
            Transform chamberTransform = currentWeapon.WeaponFrame.ChamberTransform;
            _context.SpawnParticle(currentWeapon.WeaponFrame.FX_ChamberCase, chamberTransform.position, chamberTransform.rotation);
            if (_currentMagazine % 2 == 0)
            {
                GameObject tracerObj = _context.SpawnTrail(currentWeapon.WeaponFrame.FX_BulletTrail, muzzleTransform.position, muzzleTransform.rotation);
                DTrailHander tracer = tracerObj.GetComponent<DTrailHander>();
                tracer.Init(muzzleTransform.position, intendedPoint, currentWeapon.WeaponFrame.ProjectileSpeed);
            }
        }
        void ConsumeAmmo()
        {
            if (_currentMagazine <= 0)
            {
                _chamberLoaded = false;
            }
            else
            {
                --_currentMagazine;
            }
        }

        IEnumerator DelayedHit(Vector3 beginPoint, Vector3 targetPoint, float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 direction = targetPoint - beginPoint;
            float maxDistance = direction.magnitude;
            if (maxDistance <= 0.001f) yield break;

            direction.Normalize();

            // 한번 더 레이케스팅
            if (Physics.Raycast(beginPoint, direction, out RaycastHit hit, MAX_DISTANCE, _hitMask))
            {
                var damageable = hit.collider.GetComponent<IDDamageable>();
                if (damageable != null)
                {
                    damageable.Hit();
                }

                var currentWeapon = _weapons[_currentType];
                if (currentWeapon != null)
                {
                    _context.SpawnParticle(currentWeapon.WeaponFrame.FX_ImpactShard, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }

        public void OnEjectMagazine(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return;
            
            // 게임패드
            _context.EjectRumble();
            _context.StopRumble(0.25f);

            // 이펙트
            Transform magazineTransform = currentWeapon.WeaponFrame.MagazineTransform;
            _context.SpawnParticle(currentWeapon.WeaponFrame.FX_MagazineEject, magazineTransform.position, magazineTransform.rotation);

            // 매쉬 숨기기
            currentWeapon.WeaponFrame.MagazineObject.SetActive(false);
            // 로직 처리
            _currentMagazine = 0;
        }
        public void OnInsertMagazine(AnimationEvent animationEvent)
        {
            var (success, currentWeapon) = TryGetWeapon();
            if (!success) return;

            _context.InsertRumble();
            _context.StopRumble(0.25f);

            // 매쉬 드러내기
            currentWeapon.WeaponFrame.MagazineObject.SetActive(true);
            // 로직 처리
            _currentMagazine = currentWeapon.WeaponFrame.MagazineCapacity;

        }

        public void OnChamberLoad(AnimationEvent animationEvent)
        {
            if (!IsArmed()) return;

            _context.ChamberLoadRumble();
            _context.StopRumble(0.25f);
            // 로직 처리
            --_currentMagazine;
            _chamberLoaded = true;
        }


        public bool IsArmed()
        {
            return _weapons[_currentType] != null;
        }
        public bool IsChamberLoaded()
        {
            return _chamberLoaded;
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