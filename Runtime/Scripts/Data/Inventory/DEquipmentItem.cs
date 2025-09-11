// using David6.ShooterCore.Data.Enum;
// using David6.ShooterCore.Data.Gear;
// using UnityEngine;

// namespace David6.ShooterCore.Data.Inventory
// {
//     [CreateAssetMenu(fileName = "EquipmentItem", menuName = "Inventory/Equipment Item")]
//     public class DEquipmentItem : ScriptableObject
//     {
//         [Header("Gear Type")]
//         public EDGearSlot GearType;
//         [Header("Gear Data")]
//         public DGearData GearData;


//         public string DisplayName => GearData != null ? GearData.GearName : "Unnamed";
//         public Sprite DisplayIcon => GearData != null ? GearData.GearIcon : null;

//     }
// }