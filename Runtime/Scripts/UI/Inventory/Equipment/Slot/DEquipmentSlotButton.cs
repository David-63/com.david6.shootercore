using System;
using David6.ShooterCore.Item.Gear;
using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotButton : MonoBehaviour
    {
        [SerializeField] EDGearType _gearType;
        public EDGearType GearType { get => _gearType; }
        [SerializeField] Image _gearImage;
        public Sprite SlotIcon
        {
            get => _gearImage.sprite;
            set
            {
                if (_gearImage.sprite != value)
                {
                    _gearImage.sprite = value;
                }
            }
        }

        public event Action<EDGearType> OnClicked;

        // Unity Inspector에서 이 함수 연결
        public void HandleClick()
        {
            Log.WhatHappend("HandleClick");
            OnClicked?.Invoke(GearType);
        }
    }
}