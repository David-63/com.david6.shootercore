using System;
using System.Collections.Generic;

namespace David6.ShooterFramework
{    
    public class ServiceLocator
    {
        /// <summary>
        /// 등록된 서비스들
        /// </summary>
        private readonly Dictionary<string, IGameManager> _services = new Dictionary<string, IGameManager>();

        public static ServiceLocator Current {get; private set; }
        public static void Initialize() { Current = new ServiceLocator(); }
        /// <summary>
        /// 주어진 타입의 서비스 인스턴스를 가져옴
        /// </summary>
        /// <typeparam name="T">조회할 서비스 타입</typeparam>
        /// <returns>서비스 인스턴스</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Get<T>() where T : IGameManager
        {
            string key = typeof(T).Name;
            if (!_services.ContainsKey(key))
            {
                Log.ErrorAlert($"{key}-{GetType().Name} 등록 등록 안되있음.");
                throw new InvalidOperationException();
            }
            return (T)_services[key];
        }
        /// <summary>
        /// 현재 서비스 로케이터에 새 서비스 등록.
        /// </summary>
        /// <typeparam name="T">등록할 서비스 타입.</typeparam>
        /// <param name="service">서비스 인스턴스.</param>
        public void Register<T>(T service) where T : IGameManager
        {
            string key = typeof(T).Name;
            if (_services.ContainsKey(key))
            {
                Log.ErrorAlert($"{key} 타입의 서비스를 등록 시도 했지만, 해당 서비스는 이미 {GetType().Name}에 등록되어 있음.");
                return;
            }
            _services.Add(key, service);
        }
        /// <summary>
        /// 현재 서비스 로케이터에서 서비스를 등록 해제.
        /// </summary>
        /// <typeparam name="T">해제할 서비스 타입.</typeparam>
        public void Unregister<T>() where T : IGameManager
        {
            string key = typeof(T).Name;
            if (!_services.ContainsKey(key))
            {
                Log.ErrorAlert($"등록 해제하려는 {key} 타입의 서비스가 {GetType().Name}에 등록되어 있지 않음.");
                return;
            }

            _services.Remove(key);
        }
    }
}
