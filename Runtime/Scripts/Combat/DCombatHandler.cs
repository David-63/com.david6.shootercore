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
        }

        public void Fire()
        {
            // 일단 무기 정보 필요함
            var currentWeapon = _weapons[_currentType];
            if (currentWeapon == null)
            {
                Log.WhatHappend("무기 업승");
                return;
            }

            Transform muzzleTransform = currentWeapon.WeaponFrame.GetMuzzle();

            Log.WhatHappend("Muzzle Position: " + muzzleTransform.transform);
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
                Log.WhatHappend("begin: " + beginPoint);
                Log.WhatHappend("hit.point: " + hit.point);

                var damageable = hit.collider.GetComponent<IDDamageable>();
                if (damageable != null)
                {
                    damageable.Hit();
                }
            }
            else
            {
                Debug.DrawLine(beginPoint, targetPoint, Color.yellow, 3f);
            }
        }

        public class DWeaponInstance
        {
            public DGearData GearData;
            public GameObject WeaponObject;
            public DWeaponFrame WeaponFrame;
        }
    }

}