using UnityEngine;

public class DWeaponFrame : MonoBehaviour
{
    // Rig 정보
    public Transform Grip_Left;
    public Transform Grip_Right;
    public Vector3 AimRigOffset;

    // 무기 스텟
    float _fireRate = 720f;
    public float FireRate => _fireRate;
    float _projectileSpeed = 10.0f;
    public float ProjectileSpeed => _projectileSpeed;


}
