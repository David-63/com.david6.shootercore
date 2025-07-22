namespace David6.ShooterCore.Provider
{
    public interface IDCooldownProvider
    {
        void StartCooldown(string key, float duration);
        bool IsReady(string key);

        void Tick(float deltaTime);
    }
}