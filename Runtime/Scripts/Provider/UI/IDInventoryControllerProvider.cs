namespace David6.ShooterCore.Provider
{
    public interface IDInventoryControllerProvider
    {
        bool SetViewProvider(IDInventoryViewProvider viewProvider);

        void HandlePause();
        void HandleResume();
    }
}