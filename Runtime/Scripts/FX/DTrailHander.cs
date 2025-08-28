using System.Collections;
using David6.ShooterCore.Pool;
using UnityEngine;

namespace David6.ShooterCore.FX
{
    public class DTrailHander : MonoBehaviour
    {
        TrailRenderer _trail;

        Vector3 _start;
        Vector3 _end;
        float _travelTime;


        void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
        }

        public void Init(Vector3 start, Vector3 end, float projectileSpeed)
        {
            _start = start;
            _end = end;
            _travelTime = Vector3.Distance(start, end) / projectileSpeed;

            _trail.Clear(); // 이전 흔적 제거
            transform.position = start;
            StartCoroutine(PlayFX());
        }

        IEnumerator PlayFX()
        {
            float elapsed = 0f;
            while (elapsed < _travelTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _travelTime);
                transform.position = Vector3.Lerp(_start, _end, t);
                yield return null;
            }

            // 도착 시점에 위치 고정
            transform.position = _end;

            // Trail이 자연스럽게 사라지도록 기다림
            yield return new WaitForSeconds(_trail.time);

            DGamePool.Instance.Return(gameObject);
        }
    }
}