using David6.ShooterCore.Pool;
using UnityEngine;

namespace David6.ShooterCore.FX
{
    public class DParticleHander : MonoBehaviour
    {
        ParticleSystem _particleSystem;
        void Awake() => _particleSystem = GetComponent<ParticleSystem>();

        void OnDisable()
        {
            DGamePool.Instance.Return(gameObject);
        }
    }
}
