namespace David6.ShooterCore.Provider
{
    public interface IDStateFactoryProvider
    {
        IDStateProvider Grounded();
        IDStateProvider Airborne();
        IDStateProvider Idle();
        IDStateProvider Walk();
        IDStateProvider Run();
    }
}