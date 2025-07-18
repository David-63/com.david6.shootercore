namespace David6.ShooterCore.Provider
{
    public interface IDStateFactoryProvider2
    {
        IDStateProvider2 Grounded();
        IDStateProvider2 Airborne();
        IDStateProvider2 Idle();
        IDStateProvider2 Walk();
        IDStateProvider2 Run();
    }
}