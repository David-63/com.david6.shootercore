using UnityEngine;

public class DWeaponFrame : MonoBehaviour
{
    // Rig 정보
    public Transform Grip_Left;
    public Transform Grip_Right;
    public Vector3 AimRigOffset;

    // Muzzle
    public Transform Muzzle;

    // 무기 스텟 (Structure or Scriptable)
    float _fireRate = 720f;
    float _projectileSpeed = 400.0f;


    public float FireRate => _fireRate;
    public float ProjectileSpeed => _projectileSpeed;

    public Transform GetMuzzle() => Muzzle;


}
