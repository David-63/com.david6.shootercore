using System;
using System.Collections.Generic;
using David6.ShooterCore.Item;
using David6.ShooterCore.Item.Weapon;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.AI;

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

        #endregion

        #region Accuracy value
        public event Action<float> OnAccuracyChanged;


        float _currentAccuracy;
        float _prevAccuracy;

        float _baseAccuracy;
        public float BaseAccuracy { get; set; }
        public float Accuracy { get; set; }

        // OverHeat
        const string WEAPONCOOLING_KEY = "WeaponCooling";
        const float COOLING_DELAY = 0.2f;
        float _overheat;
        const float _overheatMax = 100;
        float _dissipationRate = 100.0f;
        float _accumulationPerShot = 12;

        // AimBlend

        float _aimBlend = 0f;
        const float AIM_BLEND_SPEED = 5f;

        float _moveBlend = 0f;
        const float MOVE_BLEND_SPEED = 30f;


        float _minSpreadAngle = 1.2f;
        float _maxSpreadAngle = 12f;
        
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

        public void OnUpdate(float deltaTime)
        {
            if (EquippedWeapon())
            {
                // 과열 제어
                if (_overheat > 0)
                {
                    _overheat -= _dissipationRate * deltaTime;
                    _overheat = Mathf.Clamp(_overheat, 0f, _overheatMax);
                }
                // 명중률 제어
                CalculateAccuracy(deltaTime);
                //Log.WhatHappend($"_overheat: {_overheat:F2}");
            }

            if (!IsFocus) return;

            // 포커스 제어
            if (_context.CooldownProvider.IsReady(FOCUS_KEY))
            {
                IsFocus = false;
                OnFocusInactive?.Invoke();
            }

        }

        void CalculateAccuracy(float deltaTime)
        {
            _prevAccuracy = _currentAccuracy;

            // 임시 변수
            float BaseAccuracy = 100;
            float overheatFactor;
            float aimFactor;
            float moveValue;

            // 곱연산 계수
            aimFactor = CalculateAimBonus(deltaTime);                // 0.5 ~ 1
            overheatFactor = CalculateOverheatAccuracy();               // 0.4 ~ 1
            // 합연산 계수
            moveValue = CalculateMovePenaltyAccuracy(deltaTime);       // (-) 5 ~ 10

            // 베이스 합연산
            float baseAdjustedAccuracy = BaseAccuracy + moveValue;

            // 베이스 결과 값 곱연산
            float accuracyMultiplier = baseAdjustedAccuracy * overheatFactor * aimFactor;

            // TODO: 이후 추가 보정


            // 결과
            _currentAccuracy = Mathf.Clamp(accuracyMultiplier, 1f, 200f);


            Log.WhatHappend($"=================================");
            Log.WhatHappend($"moveFactor: {moveValue:F2}");
            Log.WhatHappend($"overheatFactor: {overheatFactor:F2}");
            Log.WhatHappend($"aimFactor: {aimFactor:F2}");
            Log.WhatHappend($"_currentAccuracy: {_currentAccuracy:F2}");
            Log.WhatHappend($"=================================");


            // 업데이트
            UpdateCrosshair();
            // if (MathF.Abs(_prevAccuracy - _currentAccuracy) > 0.0001f)
            // {
            //     UpdateCrosshair();
            // }

            /*
                적당한 변수명

                곱연산 종류
                multiplicativeFactor    모든 곱연산을 통합한 배율
                penaltyMultiplier       이동 반동 등으로 인한 감소율
                bonusMultiplier         조준 상태 등으로 인한 향상 비율

                합연산 종류
                additiveBonus           버프 등으로 추가되는 고정 보너스
                additivePenalty         고정 패널티
                accuracyOffset          보정용 오프셋
                baseAdditive            베이스에 더해지는 기본 합연산
            */
        }

        float CalculateMovePenaltyAccuracy(float deltaTime)
        {
            float speed = _context.HorizontalSpeed;
            float targetFactor = 0f;

            // 속도가 낮고 입력이 없는 경우
            if (_context.HasMovementInput())
            {
                targetFactor = (speed < 2.5f) ? -10f : -20f;
            }

            _moveBlend = Mathf.MoveTowards(_moveBlend, targetFactor, MOVE_BLEND_SPEED * deltaTime);
            return _moveBlend;
        }

        float CalculateOverheatAccuracy()
        {
            float maxOverheat = 100;        // 일단 상수로 두기
            float threshold = 1;
            float minAccuracy = 0.8f;       // 무기 스텟으로 두기
            float maxAccuracy = 1f;       // 무기 스텟으로 두기

            if (_overheat < threshold) return maxAccuracy;
            else if (_overheat >= maxOverheat) return minAccuracy;
            
            float normalized = Mathf.InverseLerp(threshold, maxOverheat, _overheat);
            return Mathf.Lerp(maxAccuracy, minAccuracy, normalized);
        }

        float CalculateAimBonus(float deltaTime)
        {
            float targetAim = IsAiming ? 1f : 0.6f;
            _aimBlend = Mathf.MoveTowards(_aimBlend, targetAim, AIM_BLEND_SPEED * deltaTime);
            return _aimBlend;
        }

        void UpdateCrosshair()
        {
            float normalizedAccuracy = Mathf.Clamp01(_currentAccuracy / 100f);
            float spreadAngleDeg = Mathf.Lerp(_minSpreadAngle, _maxSpreadAngle, 1f - normalizedAccuracy);

            Camera lookCam = _context.CameraHandlerProvider.LookCamera;
            Vector3 begin = GetWeaponHandler().MuzzleTransform.position;
            Vector3 direction = lookCam.transform.forward;
            float travelDistance = 100f;
            float pixelRadius = CalculateCrosshairRadiusPixels(lookCam, begin, direction, travelDistance, spreadAngleDeg);

            OnAccuracyChanged?.Invoke(pixelRadius * 2f); // 지름
        }

        float CalculateCrosshairRadiusPixels(Camera lookCam, Vector3 begin, Vector3 direction, float distance, float spreadAngleDeg)
        {
            Vector3 centerWorld = begin + direction * distance;

            DMathUtility.GetOrthonormalBasis(direction, out Vector3 right, out _);

            float spreadRad = spreadAngleDeg * Mathf.Deg2Rad;
            float spreadRadius = Mathf.Tan(spreadRad) * distance;

            Vector3 offsetWorld = centerWorld + right * spreadRadius;

            Vector3 screenCenter = lookCam.WorldToScreenPoint(centerWorld);
            Vector3 screenOffset = lookCam.WorldToScreenPoint(offsetWorld);

            return (screenOffset - screenCenter).magnitude;
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
            //_activeWeapon.FrameHandler.OnSpreedChanged += OnSpreedChanged;
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


            if (currentWeapon.Shoot(intendedPoint, _currentAccuracy))
            {
                _context.CooldownProvider.StartCooldown(WEAPONCOOLING_KEY, COOLING_DELAY);
                _context.FireRoundRumble();

                _overheat += _accumulationPerShot;
                if (_overheat > 100)
                {
                    _overheat = 100;
                }
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