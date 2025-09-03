namespace David6.ShooterCore.Data.Enum
{
    public enum EDGearType
    {
        None = -1, // None 은 장비가 없는 상태를 나타냄
        Primary,
        Sidearm,
        Head,
        UppperBody,
        Armor,
        LowerBody,
        GadgetA,
        GadgetB,
    }

    public enum EDAmmoType
    {
        None = -1,
        HighVelocity,
        LowVelocity,
        Energy,
        Rockets,
    }
}