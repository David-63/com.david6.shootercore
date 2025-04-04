//using System.Collections.Generic;
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async Task Initialize()
        {
            ServiceLocator.Initialize();

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
                Log.ErrorAlert("PlayerManager 에셋 로드 안됨.");
            }
        }
    }
}
