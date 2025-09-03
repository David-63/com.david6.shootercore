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
    [SerializeField] GameObject _fxMuzzleFlash;
    [SerializeField] Transform _chamberTransform;
    [SerializeField] GameObject _fxChamberCase;
    [SerializeField] Transform _magazineTransform;
    [SerializeField] GameObject _fxMagazineEject;
    [SerializeField] GameObject _magazineObject;

    [SerializeField] GameObject _fxBulletTrail;



    [Header("Impact")]
    [SerializeField] GameObject _fxImpactShard;


    // 무기 스텟 (Structure or Scriptable)
    [SerializeField] readonly float _fireRate = 720f;
    [SerializeField] readonly float _projectileSpeed = 100.0f;
    [SerializeField] readonly int _magazineCapacity = 25;
    [SerializeField] readonly int _maxReserveAmmo = 100;


    // Rig 공유
    public Transform GripLeft => _gripLeft;
    public Transform GripRight => _gripRight;
    public Vector3 AimRigOffset => _aimRigOffset;

    // 모듈 공유
    public Transform MuzzleTransform => _muzzleTransform;
    public GameObject FX_MuzzleFlash => _fxMuzzleFlash;
    public Transform ChamberTransform => _chamberTransform;
    public GameObject FX_ChamberCase => _fxChamberCase;
    public Transform MagazineTransform => _magazineTransform;
    public GameObject FX_MagazineEject => _fxMagazineEject;
    public GameObject MagazineObject => _magazineObject;

    public GameObject FX_BulletTrail => _fxBulletTrail;


    public GameObject FX_ImpactShard => _fxImpactShard;

    // 무기 스텟
    public float FireRate => _fireRate;
    public float ProjectileSpeed => _projectileSpeed;
    public int MagazineCapacity => _magazineCapacity;

    // 이건 Inventory에 있어야하는 개념
    public int MaxReserveAmmo => _maxReserveAmmo;

}
