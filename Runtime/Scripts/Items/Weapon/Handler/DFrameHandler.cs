using System.Collections;
using David6.ShooterCore.FX;
using David6.ShooterCore.Provider;
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
        DMuzzleModule MuzzleModule;
        DMagazineModule MagazineModule;




        IDContextProvider _context;
        DGear _gear;
        DWeaponData _weaponData;
        const float MAX_DISTANCE = 500.0f;
        LayerMask HitMask;
        int _currentMagazine;
        bool _chamberLoaded = false;


        public void AttachMuzzle(Transform muzzleObject)
        {
            muzzleObject.transform.SetParent(MuzzleSocket);
            muzzleObject.transform.localPosition = Vector3.zero;
            muzzleObject.transform.localRotation = Quaternion.identity;
        }
        public void AttachMagazine(GameObject magazineObject)
        {
            magazineObject.transform.SetParent(MagazineSocket);
            magazineObject.transform.localPosition = Vector3.zero;
            magazineObject.transform.localRotation = Quaternion.identity;
        }


        public void Initialize(IDContextProvider context, DGear gear)
        {
            _context = context;
            _gear = gear;

            _weaponData = _gear.BaseData as DWeaponData;
            MuzzleModule = _gear.GetModule<DMuzzleModule>();
            MagazineModule = _gear.GetModule<DMagazineModule>();
        }

        public bool Shoot(Vector3 intendedPoint)
        {
            if (!_chamberLoaded)
            {
                // 빈 클립 사운드 재생

                return false;
            }


            float travelDistance = Vector3.Distance(MuzzleTransform.position, intendedPoint);
            float delay = travelDistance / _weaponData.ProjectileSpeed;

            WeaponFireFX(intendedPoint);

            _context.ExecuteCoroutine(DelayedHit(MuzzleTransform.position, intendedPoint, delay));
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
                _context.SpawnParticle(MagazineModule.ImpactShardFX, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        void WeaponFireFX(Vector3 intendedPoint)
        {
            _context.SpawnParticle(MuzzleModule.MuzzleFlashFX, MuzzleTransform.position, MuzzleTransform.rotation);
            _context.SpawnParticle(MagazineModule.ChamberCaseFX, ChamberTransform.position, ChamberTransform.rotation);
            GameObject tracerObj = _context.SpawnTrail(MagazineModule.BulletTrailFX, MuzzleTransform.position, MuzzleTransform.rotation);
            DTrailHander tracer = tracerObj.GetComponent<DTrailHander>();
            tracer.Init(MuzzleTransform.position, intendedPoint, _weaponData.ProjectileSpeed);
        }

        public void EjectMagazine()
        {
            _context.SpawnParticle(MagazineModule.MagazineEjectFX, MagazineTransform.position, MagazineTransform.rotation);

            // 매쉬 숨기기
            MagazineSocket.gameObject.SetActive(false);
            _currentMagazine = 0;
        }

        public void InsertMagazine()
        {
            MagazineSocket.gameObject.SetActive(true);
            _currentMagazine = _weaponData.MagazineCapacity;
        }
        public void ChamberLoad()
        {
            ConsumeAmmo();
            _chamberLoaded = true;
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

        public bool IsChamberLoaded() => _chamberLoaded;

    }
}