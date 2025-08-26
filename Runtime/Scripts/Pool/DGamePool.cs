using System.Collections.Generic;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.Pool
{
    public class DGamePool : MonoBehaviour
    {
        public static DGamePool Instance { get; private set; }

        Dictionary<string, Queue<GameObject>> _pool = new Dictionary<string, Queue<GameObject>>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject obj = null;

            // 큐 없으면 생성
            string key = prefab.name;
            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new Queue<GameObject>();
            }

            // 비활성 오브젝트 탐색
            int count = _pool[key].Count;
            for (int idx = 0; idx < count; ++idx)
            {
                GameObject candidate = _pool[key].Dequeue();
                if (!candidate.activeSelf)
                {
                    obj = candidate;
                    break;
                }
                else
                {
                    _pool[key].Enqueue(candidate);
                }
            }

            // 없으면 생성
            if (obj == null)
            {
                obj = Instantiate(prefab, position, rotation, parent);
                obj.name = key;
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(true);

            return obj;
        }
        public GameObject Get(GameObject prefab, Transform transform)
        {
            GameObject obj = null;

            // 큐 없으면 생성
            string key = prefab.name;
            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new Queue<GameObject>();
            }

            // 비활성 오브젝트 탐색
            int count = _pool[key].Count;
            
            for (int idx = 0; idx < count; ++idx)
            {
                GameObject candidate = _pool[key].Dequeue();
                if (!candidate.activeSelf)
                {
                    obj = candidate;
                    break;
                }
                else
                {
                    _pool[key].Enqueue(candidate);
                }
            }

            // 없으면 생성
            if (obj == null)
            {
                obj = Instantiate(prefab, transform);
                obj.name = key;
            }

            obj.transform.SetPositionAndRotation(transform.position, transform.rotation);
            obj.SetActive(true);

            return obj;
        }

        public GameObject GetActive(GameObject prefab, Transform transform = null)
        {
            string key = prefab.name;

            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new Queue<GameObject>();
            }

            GameObject obj = _pool[key].Count > 0 ? _pool[key].Dequeue() : Instantiate(prefab, transform);
            obj.name = key;

            obj.transform.SetParent(transform);
            obj.SetActive(true);

            return obj;
        }


        public void Return(GameObject obj)
        {
            string key = obj.name;
            obj.SetActive(false);

            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new Queue<GameObject>();
            }

            _pool[key].Enqueue(obj);
        }
    }
}