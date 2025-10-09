using System;
using System.Collections;
using David6.ShooterCore.FX;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

            // 시작 좌표
            Vector3 beginPoint = MuzzleTransform.position;
            // 사격 거리 & 지연시간
            float travelDistance = Vector3.Distance(beginPoint, intendedPoint);
            float delay = travelDistance / _weaponData.ProjectileSpeed;

            // 방향 계산
            Vector3 targetPoint = CalculateTargetPoint(beginPoint, intendedPoint, accuracy, travelDistance);

            // 실제 발사
            _context.ExecuteCoroutine(DelayedHit(beginPoint, targetPoint, delay));
            WeaponFireFX(targetPoint);
            ConsumeAmmo();
            return true;
        }
        IEnumerator DelayedHit(Vector3 beginPoint, Vector3 targetPoint, float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 direction = targetPoint - beginPoint;
            float maxDistance = direction.magnitude;
            if (maxDistance <= 0.001f) yield break;

            direction.Normalize();

            if (Physics.Raycast(beginPoint, direction, out RaycastHit hit, MAX_DISTANCE, HitMask))
            {
                var damageable = hit.collider.GetComponent<IDDamageable>();
                if (damageable != null)
                {
                    damageable.Hit();
                }
                _context.SpawnParticle(_magazineModule.ImpactShardFX, hit.point, Quaternion.LookRotation(hit.normal));
                //Log.WhatHappend($"Hit: {hit.collider.name}");
            }
        }


        // Range 계산
        /// <summary>
        /// 사격 방향 계산
        /// </summary>
        /// <param name="beginePoint"></param>
        /// <param name="intendedPoint"></param>
        /// <returns></returns>
        Vector3 CalculateHitPoint(Vector3 beginePoint, Vector3 intendedPoint)
        {
            Vector3 forward = intendedPoint - beginePoint;
            float intendedDistance = forward.magnitude;
            if (intendedDistance <= 0.001f)
            {
                forward = MuzzleTransform.forward;
                intendedDistance = 1f;
            }
            forward.Normalize();

            Vector3 targetPoint = beginePoint + forward * Mathf.Min(intendedDistance, _weaponData.EffectiveRange);

            return targetPoint;
        }

        // spreed 적용
        /// <summary>
        /// 탄퍼짐 적용
        /// </summary>
        /// <param name="spreed"></param>
        /// <returns></returns>
        Vector3 ApplySpreedOffset(float spreed)
        {
            Vector2 rand2D = UnityEngine.Random.insideUnitCircle * spreed;
            Vector3 right = MuzzleTransform.right * rand2D.x;
            Vector3 up = MuzzleTransform.up * rand2D.y;

            return right + up;
        }

        Vector3 CalculateTargetPoint(Vector3 beginPoint, Vector3 intendedPoint, float accuracy, float travelDistance)
        {
            // accuracy 클램핑
            accuracy = Mathf.Clamp01(accuracy);

            // accuracy -> spreadAngle 전환
            float spreadAngleDeg = Mathf.Lerp(_minSpreadAngle, _maxSpreadAngle, 1f - accuracy);
            // 기준 방향
            Vector3 forward = (intendedPoint - beginPoint).normalized;

            Vector3 totalDirection = SampleDirectionUniformCone(forward, spreadAngleDeg);
            return beginPoint + totalDirection * travelDistance;

            // float baseSpreadAngle = 2.0f;

            // // 정확도가 클스록 각도는 작아짐
            // float spreadAngle = baseSpreadAngle / Mathf.Max(accuracy, 0.01f);

            // // 각도를 거리로 변환
            // float spreadRadius = Mathf.Tan(spreadAngle * Mathf.Deg2Rad) * travelDistance;

            // // 랜덤 오프셋 (거리 보정)
            // Vector2 rand2D = UnityEngine.Random.insideUnitCircle * spreadRadius;

            // Vector3 right, up;
            // GetOrthonormalBasis(direction, out right, out up);

            // Vector3 offset = right * rand2D.x + up * rand2D.y;

            // // Vector3 hitPoint = CalculateHitPoint(beginPoint, intendedPoint);
            // // Vector3 offset = ApplySpreedOffset(accuracy);
            // //return hitPoint + offset;

            // return beginPoint + direction * travelDistance + offset;
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
            Vector3 local = new Vector3(Mathf.Cos(phi) * sinTheta, Mathf.Sin(phi) * sinTheta, cosTheta);

            // 회전하여 forward 축에 맞춤
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, forward.normalized);
            return rot * local; // already normalized
        }

        void GetOrthonormalBasis(Vector3 forward, out Vector3 right, out Vector3 up)
        {
            // 안정적인 기준벡터 생성 (forward와 거의 평행한 world up 처리)
            Vector3 worldUp = Vector3.up;
            right = Vector3.Cross(worldUp, forward);
            if (right.sqrMagnitude < 1e-6f)
                right = Vector3.Cross(Vector3.forward, forward);
            right.Normalize();
            up = Vector3.Cross(forward, right).normalized;
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