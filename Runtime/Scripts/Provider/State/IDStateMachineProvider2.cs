namespace David6.ShooterCore.Provider
{
    public interface IDStateMachineProvider2
    {
        public IDStateFactoryProvider2 Factory { get; }
        public IDStateProvider2 CurrentState { get; }
        void InitializeStateMachine();
        void SetInitialState(IDStateProvider2 initialState);
        void ChangeState(IDStateProvider2 newState);

        void OnUpdate();
    }
}