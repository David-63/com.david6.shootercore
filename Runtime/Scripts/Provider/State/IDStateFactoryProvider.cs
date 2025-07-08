namespace David6.ShooterCore.Provider
{
    public interface IDStateFactoryProvider
    {
        IDStateProvider Grounded();
        IDStateProvider Airborne();
    }
}