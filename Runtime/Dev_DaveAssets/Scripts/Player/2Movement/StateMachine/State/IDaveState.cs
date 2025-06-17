using UnityEngine;

namespace Dave6.ShooterFramework
{
    /// <summary>
    /// 여기에 State 인터페이스를 정의했지만, 다른 StateMachine 클래스는 전용 State 인터페이스를 가짐
    /// 그니까 여기에 있는건 사용하는 인터페이스가 아님
    /// 1. 캡슐화
    /// 2. 접근 경로 구분
    /// 3. 병렬 FSM 구조 설계
    /// </summary>
    public interface IDaveState
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();

    }
}