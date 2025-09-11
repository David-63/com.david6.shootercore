using System.Collections.Generic;
using David6.ShooterCore.Item;
using UnityEngine;

namespace David6.ShooterCore.Data.Inventory
{
    [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "Inventory/Equipment Database")]
    public class DEquipmentDatabase : ScriptableObject
    {
        public List<DGear> EquipmentItems;
    }
}
