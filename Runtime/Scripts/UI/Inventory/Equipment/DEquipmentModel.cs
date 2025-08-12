using System;
using System.Collections.Generic;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Item.Gear;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentModel
    {
        // 장비중인 아이템
        public Dictionary<EDGearType, DGearData> Equipped { get; private set; } = new();
        // 보유중인 아이템
        public Dictionary<EDGearType, List<DGearData>> EquipmentItems { get; set; } = new();
        // 장비 변경 이벤트
        public event Action<EDGearType, DGearData> OnGearChanged;

        // 현재 선택중인 타입
        EDGearType _selectedGearType;

        public DEquipmentModel()
        {
            Equipped.Clear();
            EquipmentItems.Clear();
            foreach (EDGearType gearType in Enum.GetValues(typeof(EDGearType)))
            {
                if (gearType == EDGearType.None) continue;

                Equipped[gearType] = DGearData.Empty;
                EquipmentItems[gearType] = new List<DGearData>();
            }
        }

        public void Initialize(List<DEquipmentItem> items)
        {
            foreach (var item in items)
            {
                if (item == null || item.GearData == null) continue;

                if (!EquipmentItems.ContainsKey(item.GearType))
                {
                    EquipmentItems[item.GearType] = new List<DGearData>();
                }

                EquipmentItems[item.GearType].Add(item.GearData);
            }
        }

        public void EquipGear(EDGearType gearType, DGearData gearData)
        {
            if (!Equipped.ContainsKey(gearType)) return;

            Equipped[gearType] = gearData;
            OnGearChanged?.Invoke(gearType, gearData);
        }

        public void AddItem(EDGearType type, DGearData gearData)
        {

            if (!EquipmentItems.ContainsKey(type))
            {
                EquipmentItems[type] = new List<DGearData>();
            }

            EquipmentItems[type].Add(gearData);
        }

        public List<DGearData> GetItems(EDGearType type) => EquipmentItems.TryGetValue(type, out var list) ? list : new List<DGearData>();
        public DGearData GetEquippedGear(EDGearType gearType) => Equipped.TryGetValue(gearType, out var item) ? item : DGearData.Empty;
        public void SetListDisplayGearType(EDGearType type) => _selectedGearType = type;
        public EDGearType GetListDisplayGearType() => _selectedGearType;


    }
}