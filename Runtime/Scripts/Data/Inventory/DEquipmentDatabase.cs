using System.Collections.Generic;
using UnityEngine;

namespace David6.ShooterCore.Data.Gear
{
    [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "Inventory/Equipment Database")]
    public class DEquipmentDatabase : ScriptableObject
    {
        public List<DEquipmentItem> EquipmentItems;
    }
}
