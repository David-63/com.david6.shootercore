namespace David6.ShooterCore.Provider
{
    public interface IDTickSchedulerProvider
    {
        void Register(object tickableObject);
        void Unregister(object tickableObject);
    }
}