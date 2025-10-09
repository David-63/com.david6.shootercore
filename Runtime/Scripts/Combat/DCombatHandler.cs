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
        float _dissipationRate = 75.0f;
        float _accumulationPerShot = 4;

        // AimBlend

        float _aimBlend = 0f;
        const float AIM_BLEND_SPEED = 5f;

        float _moveBlend = 0f;
        const float MOVE_BLEND_SPEED = 1f;


        float _minSpreadAngle = 1f;
        float _maxSpreadAngle = 10f;
        
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
            if (!IsFocus) return;

            // 포커스 제어
            if (_context.CooldownProvider.IsReady(FOCUS_KEY))
            {
                IsFocus = false;
                OnFocusInactive?.Invoke();
            }

            // 과열 제어
            if (_context.CooldownProvider.IsReady(WEAPONCOOLING_KEY))
            {
                _overheat -= _dissipationRate * deltaTime;
                _overheat = Mathf.Clamp(_overheat, 0f, _overheatMax);
            }

            // 명중률 제어
            if (EquippedWeapon())
            {
                CalculateAccuracy(deltaTime);
            }

        }

        void CalculateAccuracy(float deltaTime)
        {
            _prevAccuracy = _currentAccuracy;

            // 임시 변수
            float fireValue = 0;
            float moveValue = 0;
            float aimValue = 0;

            /*
                . 사격

                - 과열 상태로 체크
                - 과열은 0 ~ 100으로 나뉨
            */

            fireValue = 1 - (_overheat / 100.0f);

            float maxOverheat = 100;
            float threshold = 30;
            float minAccuracy = 0.5f;
            float maxAccuracy = 1.0f;


            if (_overheat < threshold)
            {
                fireValue = maxAccuracy;
            }
            else if (_overheat >= maxOverheat)
            {
                fireValue = minAccuracy;
            }
            else
            {
                float normalized = Mathf.InverseLerp(threshold, maxOverheat, _overheat);
                fireValue = Mathf.Lerp(maxAccuracy, minAccuracy, normalized);
            }

            // 정확도 요소

            /*
                . 이동 (정확도 보단 안정성에 불이익)

                - 정지 상태 (zero)
                - 6m/s 미만: (-0.1) 미묘하게 감소
                - 6m/s 이상: (-0.3) 다소 감소
            */
            float speed = _context.HorizontalSpeed;
            // = _context.HasMovementInput() ? -0.3f : 0f
            float targetMove;
            if (speed <= 0.01f || !_context.HasMovementInput())
            {
                targetMove = 0;
            }
            else if (speed < 2.5f)
            {
                targetMove = -0.1f;
            }
            else
            {
                targetMove = -0.3f;
            }

            _moveBlend = Mathf.MoveTowards(_moveBlend, targetMove, MOVE_BLEND_SPEED * deltaTime);
            moveValue = _moveBlend;

            

            /*
                . 조준

                - 점진적으로 max(0.5) 수치까지 올라가야함
                - 해제할때도 똑같이 되게 하고싶은데
            */
            // if (IsAiming)
            // {
            //     aimValue += 0.5f;
            // }

            float targetAim = IsAiming ? 0.5f : 0f;
            _aimBlend = Mathf.MoveTowards(_aimBlend, targetAim, AIM_BLEND_SPEED * deltaTime);
            aimValue = _aimBlend;



            // 예상
            // Fire 계수 (1~0.5 베이스) + Move 계수 (- 합연산) + Aim 계수 (0~ 0.5) 
            _currentAccuracy = Mathf.Clamp(fireValue + moveValue + aimValue, 0.01f, 3f);
            // 업데이트
            if (MathF.Abs(_prevAccuracy - _currentAccuracy) > 0.001f)
            {

                float spreadAngleDeg = Mathf.Lerp(_minSpreadAngle, _maxSpreadAngle, 1f - _currentAccuracy);

                Vector3 begin = GetWeaponHandler().MuzzleTransform.position;
                Camera lookCam = _context.CameraHandlerProvider.LookCamera;
                Vector3 direction = lookCam.transform.forward;
                float travelDistance = 100f;
                float pixelRadius = CalculateCrosshairRadiusPixels(lookCam, begin, direction, travelDistance, spreadAngleDeg);
                
                OnAccuracyChanged?.Invoke(pixelRadius * 2f); // 지름
            }


            // Log.WhatHappend($"종합 명중률: {_currentAccuracy}");
            // Log.WhatHappend($"fireValue: {fireValue:F2}, overheat: {_overheat:F2}");
            // Log.WhatHappend($"speed: {speed:F2}, targetMove: {targetMove:F2}, moveBlend: {_moveBlend:F2}");
            // Log.WhatHappend($"targetAim: {targetAim:F2}, aimBlend: {_aimBlend:F2}");
            
        }

        private float CalculateCrosshairRadiusPixels(Camera lookCam, Vector3 begin, Vector3 direction, float travelDistance, float spreadAngleDeg)
        {
            Vector3 centerWorld = begin + direction * travelDistance;

            Vector3 right, up;
            GetOrthonormalBasis(direction, out right, out up);

            float spreadRad = spreadAngleDeg * Mathf.Deg2Rad;
            float spreadRadius = Mathf.Tan(spreadRad) * travelDistance;

            Vector3 offsetWorld = centerWorld + right * spreadRadius;

            Vector3 screenCenter = lookCam.WorldToScreenPoint(centerWorld);
            Vector3 screenOffset = lookCam.WorldToScreenPoint(offsetWorld);

            return (screenOffset - screenCenter).magnitude;
        }
        void GetOrthonormalBasis(Vector3 forward, out Vector3 right, out Vector3 up)
        {
            // 안정적인 기준벡터 생성 (forward와 거의 평행한 world up 처리)
            Vector3 worldUp = Vector3.up;
            right = Vector3.Cross(worldUp, forward);

            if (right.sqrMagnitude < 1e-6f)
            {
                right = Vector3.Cross(Vector3.forward, forward);
            }

            right.Normalize();
            up = Vector3.Cross(forward, right).normalized;
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