namespace David6.ShooterCore.Provider
{
    public interface IDStateProvider2
    {
        bool IsRoot { get; set; }
        IDContextProvider Context { get; }
        IDStateMachineProvider2 StateMachine { get; }

        IDStateProvider2 SuperState { get; }
        IDStateProvider2 SubState { get; }
        /// <summary>
        /// 애니메이터 파라미터 수정 및 초기 세팅
        /// </summary>
        void EnterState();
        void UpdateSelf();
        void ExitState();
        void CheckTransition();
        void InitializeSubState();
        void UpdateAll();

        void SetSuperState(IDStateProvider2 superState);
        void SetSubState(IDStateProvider2 subState);

        void SwitchSubState(IDStateProvider2 newState);
    }
}