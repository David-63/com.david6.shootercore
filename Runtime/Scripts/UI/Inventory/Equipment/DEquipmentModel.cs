using System;
using System.Collections.Generic;
using David6.ShooterCore.Item;
using David6.ShooterCore.Tools;
using Mono.Cecil.Cil;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentModel
    {
        // 장비중인 아이템
        public Dictionary<EDGearSlot, DGear> Equipped { get; private set; } = new();
        // 보유중인 아이템
        public Dictionary<EDGearSlot, List<DGear>> EquipmentItems { get; set; } = new();
        // 장비 변경 이벤트
        public event Action<DGear> OnGearChanged;

        // 현재 선택중인 타입
        EDGearSlot _selectedGearType;
        public Dictionary<EDAmmoType, int> Ammunition { get; private set; } = new();

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

            foreach (EDAmmoType ammoType in Enum.GetValues(typeof(EDAmmoType)))
            {
                if (ammoType == EDAmmoType.None) continue;
                Ammunition[ammoType] = 1200;
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

        public void SetEquippedGear(DGear gearData)
        {
            var gearType = gearData.GearSlot;
            if (!Equipped.ContainsKey(gearType)) return;

            Log.WhatHappend($"Equip Gear {gearType} : {gearData.DisplayName}");

            Equipped[gearType] = gearData;

            OnGearChanged?.Invoke(gearData);
        }

        public void AddItem(DGear gearData)
        {
            var gearType = gearData.GearSlot;
            if (gearData.GearSlot != gearType)
            {
                Log.WhatHappend($"장비 타입 불일치 {gearType} != {gearData.GearSlot}");
                return;
            }
            if (!EquipmentItems.ContainsKey(gearType))
            {
                EquipmentItems[gearType] = new List<DGear>();
            }

            Log.WhatHappend($"Add Item {gearType} : {gearData.DisplayName}");

            EquipmentItems[gearType].Add(gearData);
        }

        public List<DGear> GetItems(EDGearSlot type) => EquipmentItems.TryGetValue(type, out var list) ? list : new List<DGear>();
        public DGear GetEquippedGear(EDGearSlot gearType) => Equipped.TryGetValue(gearType, out var item) ? item : DGear.Empty;
        public void SetListDisplayGearType(EDGearSlot type) => _selectedGearType = type;
        public EDGearSlot GetListDisplayGearType() => _selectedGearType;

        public int CountingAmmunition(EDAmmoType type)
        {
            return Ammunition[type];
        }

        public int ConsumeAmmunition(EDAmmoType type, int rounds)
        {
            if (!Ammunition.TryGetValue(type, out int ammo) || ammo <= 0) return 0;

            int consumed = (rounds > ammo) ? ammo : rounds;
            Ammunition[type] = ammo - consumed;

            return consumed;
        }
    }
}