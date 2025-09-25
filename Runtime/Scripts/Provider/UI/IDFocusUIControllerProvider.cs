
namespace David6.ShooterCore.Provider
{
    public interface IDFocusUIControllerProvider : IDProvider
    {
        void HandleFocusOn();
        void HandleFocusOff();
    }
}