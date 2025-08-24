using UnityEngine;

public class DWeaponFrame : MonoBehaviour
{
    // Rig 정보
    [Header("Rig Transform")]
    [SerializeField] Transform _gripLeft;
    [SerializeField] Transform _gripRight;
    [SerializeField] Vector3 _aimRigOffset;

    [Header("Weapon Module")]
    [SerializeField] Transform _muzzleTransform;
    [SerializeField] GameObject _muzzleFlash;
    [SerializeField] Transform _chamberTransform;
    [SerializeField] GameObject _chamberCase;
    [SerializeField] Transform _magazineTransform;
    [SerializeField] GameObject _magazineEject;
    [SerializeField] GameObject _magazineObject;


    [Header("Impact")]
    [SerializeField] GameObject _impactShard;


    // 무기 스텟 (Structure or Scriptable)
    [SerializeField] readonly float _fireRate = 720f;
    [SerializeField] readonly float _projectileSpeed = 400.0f;
    [SerializeField] readonly int _magazineCapacity = 25;
    [SerializeField] readonly int _maxReserveAmmo = 100;


    // Rig 공유
    public Transform GripLeft => _gripLeft;
    public Transform GripRight => _gripRight;
    public Vector3 AimRigOffset => _aimRigOffset;

    // 모듈 공유
    public Transform MuzzleTransform => _muzzleTransform;
    public GameObject MuzzleFlash => _muzzleFlash;
    public Transform ChamberTransform => _chamberTransform;
    public GameObject ChamberCase => _chamberCase;
    public Transform MagazineTransform => _magazineTransform;
    public GameObject MagazineEject => _magazineEject;
    public GameObject MagazineObject => _magazineObject;
    public GameObject ImpactShard => _impactShard;

    // 무기 스텟
    public float FireRate => _fireRate;
    public float ProjectileSpeed => _projectileSpeed;
    public int MagazineCapacity => _magazineCapacity;
    public int MaxReserveAmmo => _maxReserveAmmo;
}
