namespace David6.ShooterCore.Provider
{
    public interface IDTickInitializerProvider
    {
        void InitializeTick(IDTickSchedulerProvider scheduler);
    }
}