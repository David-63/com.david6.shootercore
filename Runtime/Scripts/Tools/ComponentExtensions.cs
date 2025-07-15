using UnityEngine;

namespace David6.ShooterCore.Tools
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// 자기 자신과 모든 자식에서 T 컴포넌트를 탐색, out 파라미터와 bool로 반환.
        /// includeInactive = true 로 호출하면 비활성 자식도 검색함.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="component"> 대상 컴포넌트 </param>
        /// <param name="includeInactive"> 비활성 객체 탐색 여부 </param>
        /// <returns> 성공 여부 </returns>
        public static bool TryGetComponentInChildren<T>(this Component self, out T component, bool includeInactive = false) where T : Component
        {
            if (self.TryGetComponent<T>(out component)) return true;

            foreach (Transform child in self.transform)
            {
                if (!includeInactive && !child.gameObject.activeInHierarchy) continue;

                if (child.TryGetComponentInChildren(out component, includeInactive)) return true;
            }

            component = null;
            return false;
        }
    }
}