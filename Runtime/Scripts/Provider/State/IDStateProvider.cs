namespace David6.ShooterCore.Provider
{
    public interface IDStateProvider
    {
        bool IsRoot { get; set; }
        bool DebugMode { get; set; }
        IDContextProvider Context { get; }
        IDStateMachineProvider StateMachine { get; }

        IDStateProvider SuperState { get; }
        IDStateProvider SubState { get; }
        /// <summary>
        /// 애니메이터 파라미터 수정 및 초기 세팅
        /// </summary>
        void EnterState();
        void UpdateSelf();
        void ExitState();
        void CheckTransition();
        void InitializeSubState();
        void UpdateAll();

        void SetSuperState(IDStateProvider superState);
        void SetSubState(IDStateProvider subState);

        void SwitchSubState(IDStateProvider newState);
    }
}