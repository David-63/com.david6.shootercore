using System;
using System.Collections.Generic;
using UnityEngine;

namespace David6.ShooterCore.Item
{
    public enum EDGearSlot
    {
        None = -1,
        Primary,
        Sidearm,
        Head,
        UppperBody,
        Armor,
        LowerBody,
        GadgetA,
        GadgetB,
    }
    public enum EDAmmoType
    {
        None = -1,
        Compact, // 평균 화력, 반동 회복
        Impact, // 높은 화력, 적은 장탄
        Assault, // 약간 낮은 화력, 관통
        Marksman, // 약간 높은 화력, 적은 장탄, 관통

    }

    [CreateAssetMenu(fileName = "Gear", menuName = "Item/Gear")]
    public class DGear : ScriptableObject
    {
        static DGear _empty;
        static DGear CreateEmpty() => CreateInstance<DGear>();
        public static DGear Empty => _empty ??= CreateEmpty();

        public EDGearSlot GearSlot;
        public DBaseItemData BaseData;
        public DBaseItemModule[] Modules;

        Dictionary<Type, DBaseItemModule> _moduleMap;


        public string DisplayName => BaseData != null ? BaseData.ItemName : "Unnamed";
        public Sprite DisplayIcon => BaseData != null ? BaseData.ItemIcon : null;


        void OnEnable()
        {
            _moduleMap = new Dictionary<Type, DBaseItemModule>();
            if (Modules == null) return;

            foreach (var module in Modules)
            {
                if (module == null) continue;
                var type = module.GetType();
                if (_moduleMap.ContainsKey(type)) continue;

                _moduleMap.Add(type, module);
            }
        }

        public T GetModule<T>() where T : DBaseItemModule
        {
            if (_moduleMap == null) return null;

            if (_moduleMap.TryGetValue(typeof(T), out var module))
            {
                return module as T;
            }
            return null;
        }


    }


}