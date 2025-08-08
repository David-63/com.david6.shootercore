using System;
using System.Collections.Generic;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Item.Gear;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotModel2
    {
        public Dictionary<EDGearType, DGearData> Equipped { get; private set; } = new();
        public event Action<EDGearType, DGearData> OnGearChanged;

        public DEquipmentSlotModel2()
        {
            foreach (EDGearType gearType in Enum.GetValues(typeof(EDGearType)))
            {
                if (gearType == EDGearType.None) continue;

                Equipped[gearType] = DGearData.Empty;
            }
        }

        public void EquipGear(EDGearType gearType, DGearData gearData)
        {
            if (Equipped.ContainsKey(gearType))
            {
                Equipped[gearType] = gearData;

                OnGearChanged?.Invoke(gearType, gearData);
            }
        }

        public DGearData GetEquippedGear(EDGearType gearType) => Equipped.TryGetValue(gearType, out var item) ? item : null;

    }
}