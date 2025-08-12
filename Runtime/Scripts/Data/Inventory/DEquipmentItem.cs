using David6.ShooterCore.Item.Gear;
using UnityEngine;

namespace David6.ShooterCore.Data.Gear
{
    [CreateAssetMenu(fileName = "EquipmentItem", menuName = "Inventory/Equipment Item")]
    public class DEquipmentItem : ScriptableObject
    {
        [Header("Gear Type")]
        public EDGearType GearType;
        [Header("Gear Data")]
        public DGearData GearData;

        public string DisplayName => GearData != null ? GearData.GearName : "Unnamed";
        public Sprite DisplayIcon => GearData != null ? GearData.GearIcon : null;

    }
}