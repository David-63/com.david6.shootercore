using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;

namespace David6.ShooterCore.Provider
{
    public interface IDCombatHandler
    {
        void SetWeapon(EDGearType type, DGearData data);
        void Fire();
    }
}