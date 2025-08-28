using System.Collections;
using System.Collections.Generic;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.FX;
using David6.ShooterCore.Pool;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
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
        Camera _mainCamera;
        IDContextProvider _context;
        Dictionary<EDGearType, DWeaponInstance> _weapons = new();
        EDGearType _currentType;

        public DWeaponInstance GetCurrentWeapon => _weapons[_currentType];


        // 아직 안쓰는 기능
        public EDGearType CurrentType { get => _currentType; set => _currentType = value; }


        // 제어 변수
        const float MAX_DISTANCE = 500.0f;
        LayerMask _hitMask;



        // 총기 관련 변수

        bool _chamberLoaded = false;
        public bool ChamberLoaded { get => _chamberLoaded; }
        int _reserveAmmo; // maxReserveAmmo (_magazineCapacity * 4)
        int _currentMagazine; // _magazineCapacity

        float _fireRate;


        public DCombatHandler(IDContextProvider context)
        {
            _context = context;

            for (EDGearType type = EDGearType.Primary; type <= EDGearType.Sidearm; ++type)
            {
                if (!_weapons.ContainsKey(type))
                {
                    _weapons[type] = null;
                }
            }

            _hitMask.value = 1;
        }

        public void SetWeapon(EDGearType type, DGearData data)
        {

            // Weapon 인스턴스 등록
            if (!_weapons.TryGetValue(type, out var instance) || instance == null)
            {
                instance = new DWeaponInstance();
                _weapons[type] = instance;
            }
            _currentType = type;

            instance.GearData = data;
            if (instance.WeaponObject == null)
            {
                instance.WeaponObject = _context.MakeObject(data.GearPrefab, _context.WeaponSocket);
            }
            instance.WeaponFrame = instance.WeaponObject.GetComponent<DWeaponFrame>();

            _context.AnimatorProvider.SetFireRate(instance.WeaponFrame.FireRate);


            // 탄약 세팅 (나중에 딕셔너리로 캐싱하기)
            _chamberLoaded = true;
            _reserveAmmo = instance.WeaponFrame.MaxReserveAmmo;
            _currentMagazine = instance.WeaponFrame.MagazineCapacity - 1;


            float rps = 60 / instance.WeaponFrame.FireRate / 60f;
            float targetPeriod = 1f / rps;
            float originalClipLength = 0.15f;
            _fireRate = originalClipLength / targetPeriod;
        }

        public bool Fire()
        {
            if (!_chamberLoaded)
            {
                // no clip 사운드
                return false;
            }
            // 일단 무기 정보 필요함
            var currentWeapon = _weapons[_currentType];
            if (currentWeapon == null)
            {
                Log.WhatHappend("무기 업승");
                return false;
            }

            Vector3 intendedPoint = CalculateIntendedPoint();

            ScheduleHit(currentWeapon, intendedPoint);

            PlayFX(currentWeapon, intendedPoint);

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
        void PlayFX(DWeaponInstance currentWeapon, Vector3 intendedPoint)
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
            Log.WhatHappend("Eject!!");

            var currentWeapon = _weapons[_currentType];
            if (currentWeapon == null)
            {
                Log.WhatHappend("무기 업승");
                return;
            }

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
            Log.WhatHappend("Insert!!");
            var currentWeapon = _weapons[_currentType];
            if (currentWeapon == null)
            {
                Log.WhatHappend("무기 업승");
                return;
            }

            _context.InsertRumble();
            _context.StopRumble(0.25f);

            // 매쉬 드러내기
            currentWeapon.WeaponFrame.MagazineObject.SetActive(true);
            // 로직 처리
            _currentMagazine = currentWeapon.WeaponFrame.MagazineCapacity;

        }

        public void OnChamberLoad(AnimationEvent animationEvent)
        {
            Log.WhatHappend("Load!!");
            var currentWeapon = _weapons[_currentType];
            if (currentWeapon == null)
            {
                Log.WhatHappend("무기 업승");
                return;
            }

            _context.ChamberLoadRumble();
            _context.StopRumble(0.25f);
            // 로직 처리
            --_currentMagazine;
            _chamberLoaded = true;
        }

    }
    

    // var gamepad = Gamepad.current;
    //         if (gamepad != null)
    //         {
    //             // Example: Activate left impulse trigger on left trigger press
    //             if (gamepad.leftTrigger.isPressed)
    //             {
    //                 // Set left trigger rumble magnitude (0.0 to 1.0)
    //                 gamepad.SetMotorSpeeds(0f, 0f, 0.5f, 0f); 
    //             }
    //             else
    //             {
    //                 // Stop rumble when not pressed
    //                 gamepad.SetMotorSpeeds(0f, 0f, 0f, 0f); 
    //             }

    //             // Example: Activate right impulse trigger on right trigger press
    //             if (gamepad.rightTrigger.isPressed)
    //             {
    //                 // Set right trigger rumble magnitude (0.0 to 1.0)
    //                 gamepad.SetMotorSpeeds(0f, 0f, 0f, 0.5f); 
    //             }
    //             else
    //             {
    //                 // Stop rumble when not pressed
    //                 gamepad.SetMotorSpeeds(0f, 0f, 0f, 0f); 
    //             }
    //         }

}