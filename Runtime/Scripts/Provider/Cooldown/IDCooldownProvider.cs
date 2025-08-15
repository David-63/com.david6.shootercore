namespace David6.ShooterCore.Provider
{
    public interface IDCooldownProvider
    {
        void StartCooldown(string key, float duration);
        void CancelCooldown(string key);
        bool IsReady(string key);

        void Tick(float deltaTime);
    }
}