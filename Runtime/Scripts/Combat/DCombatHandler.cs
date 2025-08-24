using System.Collections;
using System.Collections.Generic;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.Combat
{
    public class DCombatHandler : IDCombatHandler
    {
        // 무기나 장비를 알고 있어야함

        // 외부에서 호출함
        Camera _mainCamera;
        IDContextProvider _context;
        Dictionary<EDGearType, DWeaponInstance> _weapons = new();
        EDGearType _currentType;
        public EDGearType CurrentType { get => _currentType; set => _currentType = value; }

        const float MAX_DISTANCE = 500.0f;
        public LayerMask HitMask;



        // 총기 관련 변수

        bool _chamberLoaded = false;
        int _reserveAmmo; // maxReserveAmmo (_magazineCapacity * 4)
        int _currentMagazine; // _magazineCapacity


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

            HitMask.value = 1;
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


            // 탄약 세팅 (나중에 딕셔너리로 캐싱하기)
            _chamberLoaded = true;
            _reserveAmmo = instance.WeaponFrame.MaxReserveAmmo;
            _currentMagazine = instance.WeaponFrame.MagazineCapacity - 1;
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


            // 이펙트
            Transform muzzleTransform = currentWeapon.WeaponFrame.MuzzleTransform;

            _context.MakeObject(currentWeapon.WeaponFrame.MuzzleFlash, muzzleTransform);
            _context.MakeObject(currentWeapon.WeaponFrame.ChamberCase, currentWeapon.WeaponFrame.ChamberTransform);



            Camera mainCamera = Camera.main;
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            // Ray 생성
            Ray aimRay = mainCamera.ScreenPointToRay(screenCenter);

            Vector3 intendedPoint;

            if (Physics.Raycast(aimRay, out var camHit, MAX_DISTANCE, HitMask))
            {
                intendedPoint = camHit.point;
            }
            else
            {
                intendedPoint = aimRay.GetPoint(MAX_DISTANCE);
            }

            float travelDistance = Vector3.Distance(muzzleTransform.position, intendedPoint);
            float delay = travelDistance / currentWeapon.WeaponFrame.ProjectileSpeed;
            Log.WhatHappend("delay: " + delay);

            _context.ExecuteCoroutine(DelayedHit(muzzleTransform.position, intendedPoint, delay));

            Log.WhatHappend(_currentMagazine);

            if (_currentMagazine <= 0)
            {
                _chamberLoaded = false;
            }
            else
            {
                --_currentMagazine;    
            }            
            Log.WhatHappend(_currentMagazine);

            return true;
        }

        IEnumerator DelayedHit(Vector3 beginPoint, Vector3 targetPoint, float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 direction = targetPoint - beginPoint;
            float maxDistance = direction.magnitude;
            if (maxDistance <= 0.001f) yield break;

            direction.Normalize();

            // 한번 더 레이케스팅
            if (Physics.Raycast(beginPoint, direction, out RaycastHit hit, MAX_DISTANCE, HitMask))
            {
                // 타격 이펙트
                Debug.DrawLine(beginPoint, hit.point, Color.blue, 1f);

                var damageable = hit.collider.GetComponent<IDDamageable>();
                if (damageable != null)
                {
                    damageable.Hit();
                }

                var currentWeapon = _weapons[_currentType];
                if (currentWeapon == null)
                {
                    Log.WhatHappend("무기 업승");
                }
                else
                {
                    _context.MakeObject(currentWeapon.WeaponFrame.ImpactShard, hit.point, hit.normal);
                }
                
            }
        }


        public void OnEjectMagazine(AnimationEvent animationEvent)
        {
            // 무기 가져오기
            Log.WhatHappend("Eject!!");
            var currentWeapon = _weapons[_currentType];
            if (currentWeapon == null)
            {
                Log.WhatHappend("무기 업승");
                return;
            }

            // 탄창 파티클 발생
            _context.MakeObject(currentWeapon.WeaponFrame.MagazineEject, currentWeapon.WeaponFrame.MagazineTransform);
            // 매쉬 숨기기
            currentWeapon.WeaponFrame.MagazineObject.SetActive(false);
            // 로직 처리
            _currentMagazine = 0;
        }
        public void OnInsertMagazine(AnimationEvent animationEvent)
        {
            // 무기 가져오기
            Log.WhatHappend("Insert!!");
            var currentWeapon = _weapons[_currentType];
            if (currentWeapon == null)
            {
                Log.WhatHappend("무기 업승");
                return;
            }
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
            // 로직 처리
            --_currentMagazine;
            _chamberLoaded = true;
        }

        public class DWeaponInstance
        {
            public DGearData GearData;
            public GameObject WeaponObject;
            public DWeaponFrame WeaponFrame;
        }
    }

}