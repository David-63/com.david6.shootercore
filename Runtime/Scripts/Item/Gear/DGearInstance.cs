using System;
using David6.ShooterCore.Data.Gear;

namespace David6.ShooterCore.Item.Gear
{
    [Serializable]
    public class DGearInstance
    {
        public DGearData GearData;

        public string InstanceID;
        //public int Durability;
        public int EnhancementLevel = 0;

        public DGearInstance(DGearData gearData)
        {
            GearData = gearData;
            InstanceID = Guid.NewGuid().ToString();
        }
    }
}