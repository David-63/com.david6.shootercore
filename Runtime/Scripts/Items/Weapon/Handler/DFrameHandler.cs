using System;
using System.Collections;
using David6.ShooterCore.FX;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SocialPlatforms;

namespace David6.ShooterCore.Item.Weapon
{
    public class DFrameHandler : MonoBehaviour
    {
        [Header("Socket")]
        public Transform MuzzleSocket;
        public Transform MagazineSocket;

        [Header("Rig")]
        public Transform GripLeft;
        public Transform GripRight;
        public Vector3 RigAimOffset;

        [Header("Target Transform")]
        public Transform MuzzleTransform;
        public Transform MagazineTransform;
        public Transform ChamberTransform;

        // 다른 모듈 참조
        DMuzzleModule _muzzleModule;
        DMagazineModule _magazineModule;

        IDContextProvider _context;
        DWeaponData _weaponData;
        public DWeaponData WeaponData => _weaponData;
        const float MAX_DISTANCE = 500.0f;
        LayerMask HitMask;
        bool _chamberLoaded = false;
        public bool ChamberLoaded => _chamberLoaded;
        int _currentRounds;
        public int CurrentRounds => _currentRounds;
        public Action<bool, int> OnConsumeAmmo;

        //float _effectiveRange = 15f;
        float _hitSphereRadius = 0.5f;

        float _minSpreadAngle = 1f;
        float _maxSpreadAngle = 10f;


        public void AttachMuzzle(DMuzzleModule module) => _muzzleModule = module;
        public void AttachMagazine(DMagazineModule module) => _magazineModule = module;
        public void Initialize(IDContextProvider context, DWeaponData gearData)
        {
            _context = context;
            _weaponData = gearData;
            _currentRounds = _weaponData.MagazineCapacity;

            HitMask.value = 1;
            Log.WhatHappend($"{HitMask}");
        }

        public bool Shoot(Vector3 intendedPoint, float accuracy)
        {
            if (!_chamberLoaded) return false;

            // 시작 좌표 및 거리와 지연시간
            Vector3 beginPoint = MuzzleTransform.position;
            float travelDistance = Vector3.Distance(beginPoint, intendedPoint);
            float delay = travelDistance / _weaponData.ProjectileSpeed;

            // 방향 계산
            Vector3 targetPoint = CalculateTargetPoint(beginPoint, intendedPoint, accuracy);

            // 발사 처리 로직
            _context.ExecuteCoroutine(DelayedHit(beginPoint, targetPoint, delay));
            WeaponFireFX(targetPoint);
            ConsumeAmmo();

            return true;
        }
        IEnumerator DelayedHit(Vector3 beginPoint, Vector3 targetPoint, float delay)

        {            yield return new WaitForSeconds(delay);

            Vector3 direction = (targetPoint - beginPoint).normalized;

            float travelDistance = Vector3.Distance(beginPoint, targetPoint);

            if (Physics.Raycast(beginPoint, direction, out RaycastHit hit, travelDistance, HitMask))
            {
                if (hit.collider.TryGetComponent(out IDDamageable damageable))
                {
                    damageable.Hit();
                }

                _context.SpawnParticle(_magazineModule.ImpactShardFX, hit.point, Quaternion.LookRotation(hit.normal));

            }
        }

        /// <summary>
        /// 정확도에 맞게 타겟 방향 계산
        /// </summary>
        Vector3 CalculateTargetPoint(Vector3 beginPoint, Vector3 intendedPoint, float accuracy)
        {
            // accuracy 클램핑
            float normalizedAccuracy = Mathf.Clamp01(accuracy / 100);

            // accuracy -> spreadAngle 전환
            float spreadAngleDeg = Mathf.Lerp(_minSpreadAngle, _maxSpreadAngle, 1f - normalizedAccuracy);
            // 기준 방향
            Vector3 forward = (intendedPoint - beginPoint).normalized;
            Vector3 spreadDirection = SampleDirectionUniformCone(forward, spreadAngleDeg);

            return beginPoint + spreadDirection * MAX_DISTANCE;
        }

        /// <summary>
        /// 균등한 콘 샘플러 (uniform over cone surface / area)
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="maxAngleDeg"></param>
        /// <returns></returns>
        private Vector3 SampleDirectionUniformCone(Vector3 forward, float maxAngleDeg)
        {
            // maxAngleDeg: half-angle (degrees)
            float maxRad = maxAngleDeg * Mathf.Deg2Rad;
            float cosMax = Mathf.Cos(maxRad);

            // u in [0,1)
            float u = UnityEngine.Random.value;
            // 균등한 면적 분포를 위해 cosTheta을 균등하게 샘플
            float cosTheta = Mathf.Lerp(cosMax, 1f, u); 
            float sinTheta = Mathf.Sqrt(1f - cosTheta * cosTheta);
            float phi = UnityEngine.Random.value * Mathf.PI * 2f;

            // local 방향 (z 축이 forward)
            Vector3 local = new (Mathf.Cos(phi) * sinTheta, Mathf.Sin(phi) * sinTheta, cosTheta);

            DMathUtility.GetOrthonormalBasis(forward, out var right, out var up);            

            return (local.x * right + local.y * up + local.z * forward.normalized).normalized; // already normalized
        }

        void WeaponFireFX(Vector3 intendedPoint)
        {
            _context.SpawnParticle(_muzzleModule.MuzzleFlashFX, MuzzleTransform.position, MuzzleTransform.rotation);
            _context.SpawnParticle(_magazineModule.ChamberCaseFX, ChamberTransform.position, ChamberTransform.rotation);

            GameObject tracerObj = _context.SpawnTrail(_magazineModule.BulletTrailFX, MuzzleTransform.position, MuzzleTransform.rotation);
            DTrailHander tracer = tracerObj.GetComponent<DTrailHander>();

            if (tracer == null)
            {
                Log.AttentionPlease("BulletFX: Trail 컴포넌트가 없음!!");
            }

            tracer.Init(MuzzleTransform.position, intendedPoint, _weaponData.ProjectileSpeed);
        }

        public void EjectMagazine()
        {
            _context.SpawnParticle(_magazineModule.MagazineEjectFX, MagazineTransform.position, MagazineTransform.rotation);
            MagazineSocket.gameObject.SetActive(false);
            _currentRounds = 0;
        }

        public void InsertMagazine(int ammo)
        {
            MagazineSocket.gameObject.SetActive(true);
            _currentRounds = ammo;
        }

        public void ChamberLoad()
        {
            ConsumeAmmo();
        }

        public void ConsumeAmmo()
        {
            if (_currentRounds <= 0)
            {
                _currentRounds = 0;
                _chamberLoaded = false;
            }
            else
            {
                --_currentRounds;
                _chamberLoaded = true;
            }
            OnConsumeAmmo?.Invoke(_chamberLoaded, _currentRounds);
        }

    }
}