using System;
using System.Collections;
using David6.ShooterCore.FX;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
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

        public void AttachMuzzle(DMuzzleModule module) => _muzzleModule = module;
        public void AttachMagazine(DMagazineModule module) => _magazineModule = module;
        public void Initialize(IDContextProvider context, DWeaponData gearData)
        {
            _context = context;
            _weaponData = gearData;
            _currentRounds = _weaponData.MagazineCapacity;
        }

        public bool Shoot(Vector3 intendedPoint)
        {
            if (!_chamberLoaded)
            {
                return false;
            }

            float travelDistance = Vector3.Distance(MuzzleTransform.position, intendedPoint);
            float delay = travelDistance / _weaponData.ProjectileSpeed;
            _context.ExecuteCoroutine(DelayedHit(MuzzleTransform.position, intendedPoint, delay));
            WeaponFireFX(intendedPoint);
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

            // 한번 더 레이케스팅
            if (Physics.Raycast(beginPoint, direction, out RaycastHit hit, MAX_DISTANCE, HitMask))
            {
                var damageable = hit.collider.GetComponent<IDDamageable>();
                if (damageable != null)
                {
                    damageable.Hit();
                }
                _context.SpawnParticle(_magazineModule.ImpactShardFX, hit.point, Quaternion.LookRotation(hit.normal));
            }
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