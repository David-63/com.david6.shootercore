using System;
using System.Collections.Generic;
using David6.ShooterCore.Item.Gear;

namespace David6.ShooterCore.Data.Gear
{
    [Serializable]
    public class SaveGearData
    {
        public List<GearSaveEntry> GearInventory = new();
        public List<GearSaveEntry> EquippedGear = new();
    }

    [Serializable]
    public class GearSaveEntry
    {
        public string InstanceID;
        public string GearDataName; // DGearData.Name으로 매칭
        public EDGearType GearType;
        public int EnhancementLevel;

        public GearSaveEntry(EDGearType type, DGearInstance instance)
        {
            GearType = type;
            InstanceID = instance.InstanceID;
            GearDataName = instance.GearData.name;
            EnhancementLevel = instance.EnhancementLevel;
        }
    }
}