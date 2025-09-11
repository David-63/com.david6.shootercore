using System;
using System.Collections.Generic;
using David6.ShooterCore.Item;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentModel
    {
        // 장비중인 아이템
        public Dictionary<EDGearSlot, DGear> Equipped { get; private set; } = new();
        // 보유중인 아이템
        public Dictionary<EDGearSlot, List<DGear>> EquipmentItems { get; set; } = new();
        // 장비 변경 이벤트
        public event Action<EDGearSlot, DGear> OnGearChanged;
        public event Action<DGear> OnGearEquipped;

        // 현재 선택중인 타입
        EDGearSlot _selectedGearType;


        // 보유중인 자원
        public Dictionary<EDAmmoType, float> Ammunition { get; private set; } = new();

        public DEquipmentModel()
        {
            Equipped.Clear();
            EquipmentItems.Clear();
            foreach (EDGearSlot gearType in Enum.GetValues(typeof(EDGearSlot)))
            {
                if (gearType == EDGearSlot.None) continue;

                Equipped[gearType] = DGear.Empty;
                EquipmentItems[gearType] = new List<DGear>();
            }
        }

        public void Initialize(List<DGear> items)
        {
            foreach (var item in items)
            {
                if (item == null || item.BaseData == null) continue;
                if (item.GearSlot == EDGearSlot.None) continue;

                if (!EquipmentItems.ContainsKey(item.GearSlot))
                {
                    EquipmentItems[item.GearSlot] = new List<DGear>();
                }

                EquipmentItems[item.GearSlot].Add(item);
            }
        }

        public void EquipGear(EDGearSlot gearType, DGear gearData)
        {
            if (!Equipped.ContainsKey(gearType)) return;
            Log.WhatHappend($"Equip Gear {gearType} : {gearData.DisplayName}");

            Equipped[gearType] = gearData;

            OnGearChanged?.Invoke(gearType, gearData);  // UI
            OnGearEquipped?.Invoke(gearData);           // 외부
        }

        public void AddItem(EDGearSlot type, DGear gearData)
        {
            if (gearData.GearSlot != type)
            {
                Log.WhatHappend($"장비 타입 불일치 {type} != {gearData.GearSlot}");
                return;
            }
            if (!EquipmentItems.ContainsKey(type))
            {
                EquipmentItems[type] = new List<DGear>();
            }

            Log.WhatHappend($"Add Item {type} : {gearData.DisplayName}");

            EquipmentItems[type].Add(gearData);
        }

        public List<DGear> GetItems(EDGearSlot type) => EquipmentItems.TryGetValue(type, out var list) ? list : new List<DGear>();
        public DGear GetEquippedGear(EDGearSlot gearType) => Equipped.TryGetValue(gearType, out var item) ? item : DGear.Empty;
        public void SetListDisplayGearType(EDGearSlot type) => _selectedGearType = type;
        public EDGearSlot GetListDisplayGearType() => _selectedGearType;


    }
}