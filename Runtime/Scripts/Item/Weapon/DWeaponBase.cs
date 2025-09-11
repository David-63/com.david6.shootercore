// using System.Collections;
// using System.Linq;
// using David6.ShooterCore.Data.Gear;
// using David6.ShooterCore.FX;
// using David6.ShooterCore.Provider;
// using UnityEngine;

// namespace David6.ShooterCore.Item.Weapon
// {
//     public class DWeaponBase : MonoBehaviour
//     {
//         IDContextProvider _context;
//         [SerializeField] DWeaponData _weaponData;
//         public DWeaponData WeaponData => _weaponData;

//         public DMuzzleModule MuzzleModule => _weaponData.Modules.OfType<DMuzzleModule>().FirstOrDefault();
//         public DFrameModule FrameModule => _weaponData.Modules.OfType<DFrameModule>().FirstOrDefault();
//         public DMagazineModule MagazineModule => _weaponData.Modules.OfType<DMagazineModule>().FirstOrDefault();

//         const float MAX_DISTANCE = 500.0f;
//         public LayerMask HitMask;

//         int _currentMagazine;
//         bool _chamberLoaded = false;

//         public void Initialize(IDContextProvider context, DWeaponData data)
//         {
//             _context = context;
//             _weaponData = data;
//             _currentMagazine = data.Stats.CurrentMagazineCapacity;
//             _chamberLoaded = true;
//         }

//         public bool Shoot(Vector3 intendedPoint)
//         {
//             if (!_chamberLoaded)
//             {
//                 // 빈 클립 사운드 재생

//                 return false;
//             }


//             Transform muzzleTransform = MuzzleModule.MuzzleTransform;
//             float travelDistance = Vector3.Distance(muzzleTransform.position, intendedPoint);
//             float delay = travelDistance / _weaponData.Stats.CurrentProjectileSpeed;

//             WeaponFireFX(intendedPoint);

//             _context.ExecuteCoroutine(DelayedHit(muzzleTransform.position, intendedPoint, delay));
//             ConsumeAmmo();

//             return true;
//         }

//         IEnumerator DelayedHit(Vector3 beginPoint, Vector3 targetPoint, float delay)
//         {
//             yield return new WaitForSeconds(delay);

//             Vector3 direction = targetPoint - beginPoint;
//             float maxDistance = direction.magnitude;
//             if (maxDistance <= 0.001f) yield break;

//             direction.Normalize();

//             // 한번 더 레이케스팅
//             if (Physics.Raycast(beginPoint, direction, out RaycastHit hit, MAX_DISTANCE, HitMask))
//             {
//                 var damageable = hit.collider.GetComponent<IDDamageable>();
//                 if (damageable != null)
//                 {
//                     damageable.Hit();
//                 }
//                 _context.SpawnParticle(MagazineModule.FX_ImpactShard, hit.point, Quaternion.LookRotation(hit.normal));
//             }
//         }

//         void WeaponFireFX(Vector3 intendedPoint)
//         {
//             Transform muzzleTransform = MuzzleModule.MuzzleTransform;
//             _context.SpawnParticle(MuzzleModule.FX_MuzzleFlash, muzzleTransform.position, muzzleTransform.rotation);
//             Transform chamberTransform = FrameModule.ChamberTransform;
//             _context.SpawnParticle(MagazineModule.FX_ChamberCase, chamberTransform.position, chamberTransform.rotation);
//             GameObject tracerObj = _context.SpawnTrail(MagazineModule.FX_BulletTrail, muzzleTransform.position, muzzleTransform.rotation);
//             DTrailHander tracer = tracerObj.GetComponent<DTrailHander>();
//             tracer.Init(muzzleTransform.position, intendedPoint, _weaponData.Stats.CurrentProjectileSpeed);
//         }

//         public void EjectMagazine()
//         {
//             Transform magazineTransform = MagazineModule.MagazineTransform;
//             _context.SpawnParticle(MagazineModule.FX_MagazineEject, magazineTransform.position, magazineTransform.rotation);

//             // 매쉬 숨기기
//             MagazineModule.MagazineObject.SetActive(false);
//             _currentMagazine = 0;
//         }

//         public void InsertMagazine()
//         {
//             MagazineModule.MagazineObject.SetActive(true);
//             _currentMagazine = _weaponData.Stats.CurrentMagazineCapacity;
//         }
//         public void ChamberLoad()
//         {
//             ConsumeAmmo();
//             _chamberLoaded = true;
//         }


//         void ConsumeAmmo()
//         {
//             if (_currentMagazine <= 0)
//             {
//                 _chamberLoaded = false;
//             }
//             else
//             {
//                 --_currentMagazine;
//             }
//         }

//         public bool IsChamberLoaded() => _chamberLoaded;

//     }
// }
