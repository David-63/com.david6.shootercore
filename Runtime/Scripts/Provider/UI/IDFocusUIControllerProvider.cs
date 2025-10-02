
namespace David6.ShooterCore.Provider
{
    public interface IDFocusUIControllerProvider : IDProvider
    {
        void HandleFocusOn();
        void HandleFocusOff();
        void CountingRounds(bool chamber, int rounds);
        void CountingAmmunition(int ammo);
        void AccuracyControl(float size);

    }
}