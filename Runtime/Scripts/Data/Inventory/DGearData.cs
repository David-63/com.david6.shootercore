using UnityEngine;

namespace David6.ShooterCore.Data.Gear
{
    [CreateAssetMenu(fileName = "GearData", menuName = "Inventory/GearData")]
    public class DGearData : ScriptableObject
    {
        static DGearData _empty;
        static DGearData CreateEmpty() => CreateInstance<DGearData>();
        public static DGearData Empty => _empty ??= CreateEmpty();

        public string GearName; // Name of the gear item.
        public Sprite GearIcon;
        public string GearDescription;

    }
}