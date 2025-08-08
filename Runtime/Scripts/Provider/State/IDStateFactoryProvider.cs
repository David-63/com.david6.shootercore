using System;
namespace David6.ShooterCore.Provider
{
    public interface IDStateFactoryProvider
    {
        IDStateProvider GetState(Type stateType);
        void ActiveStateDebugMode();

    }
}