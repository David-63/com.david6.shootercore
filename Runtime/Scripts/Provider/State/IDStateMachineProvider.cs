namespace David6.ShooterCore.Provider
{
    public interface IDStateMachineProvider
    {
        void SetInitialState(IDStateProvider initialState);
        void ChangeState(IDStateProvider newState);

        void OnUpdate();
    }
}