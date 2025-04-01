using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace David6.ShooterFramework
{
    /// <summary>
    /// 서비스 로케이터를 초기화해줌.
    /// </summary>
    public class Bootstraper
    {
        private const string AddressName = "PlayerManager";

        private const string MainSceneName = "MainScene";
        // Addressable 에셋의 주소(라벨 또는 이름)

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async Task Initialize()
        {
            ServiceLocator.Initialize();

            //Addressables.LoadAssetAsync<PlayerManager>(AddressName).Completed += OnPlayerManagerLoaded;




            // Addressables 비동기 로드를 Task로 변환하여 await합니다.
            AsyncOperationHandle<PlayerManager> handle = Addressables.LoadAssetAsync<PlayerManager>(AddressName);
            await handle.Task;  // Addressables의 Task를 await하면 로드가 완료될 때까지 기다립니다.

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                PlayerManager playerManager = handle.Result;
                ServiceLocator.Current.Register<IPlayerManager>(playerManager);
                Log.WhatHappend("서비스 초기화 진행.");

                // 메인 씬 전환. SceneManager.LoadSceneAsync는 기본적으로 AsyncOperation을 반환하는데,
                // 별도의 Awaiter가 없으므로, while루프를 통해 완료될 때까지 기다릴 수 있습니다.
                
                AsyncOperation sceneLoadOp = SceneManager.LoadSceneAsync(MainSceneName);
                while (!sceneLoadOp.isDone)
                {
                    await Task.Yield(); // 한 프레임 대기. 이렇게 하면 메인 쓰레드를 블로킹하지 않습니다.
                }
            }
            else
            {
                Debug.LogError("PlayerManager 에셋 로드 실패.");
            }


        }

        private static void OnPlayerManagerLoaded(AsyncOperationHandle<PlayerManager> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 만약 해당 라벨에 여러 에셋이 있다면, 필요에 따라 리스트 내부 모든 아이템을 처리할 수 있습니다.
                PlayerManager playerManager = handle.Result;
                ServiceLocator.Current.Register<IPlayerManager>(playerManager);
                Log.WhatHappend("서비스 초기화 진행.");
            }
            else
            {
                Debug.LogError("PlayerManager 에셋 로드 실패.");
            }
        }
    }
}
