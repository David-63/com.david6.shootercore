namespace David6.ShooterCore.Provider
{
    public interface IDStateProvider
    {
        void EnterState();
        void UpdateSelf();
        void ExitState();
        void CheckTransition();
        void InitializeSubState();
    }
}