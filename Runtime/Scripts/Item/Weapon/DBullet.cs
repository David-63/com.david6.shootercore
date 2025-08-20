using UnityEngine;

namespace David6.ShooterCore.Item.Weapon
{
    public class DBullet : MonoBehaviour
    {
        Rigidbody _bulletRigidbody;

        [SerializeField] float _projectileSpeed = 10.0f;

        void Awake()
        {
            _bulletRigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            ProjectileMove();
        }

        void ProjectileMove()
        {
            //_bulletRigidbody.AddForce(transform.forward * _projectileSpeed);
            _bulletRigidbody.linearVelocity = transform.forward * _projectileSpeed;
        }
    }

}