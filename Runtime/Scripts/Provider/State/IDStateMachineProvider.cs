namespace David6.ShooterCore.Provider
{
    public interface IDStateMachineProvider
    {
        public IDStateFactoryProvider Factory { get; }
        public IDStateProvider CurrentState { get; }

        void SetInitialState(IDStateProvider initialState);
        void ChangeState(IDStateProvider newState);
        void OnUpdate();
        void ActiveStateDebugMode();
    }
}