using System;
using David6.ShooterCore.Item.Gear;
using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipSlotButtonView2 : MonoBehaviour
    {
        [SerializeField] EDGearType _gearType;
        [SerializeField] Image _buttonImage;


        public EDGearType GearType => _gearType;

        public Sprite SlotIcon
        {
            get => _buttonImage.sprite;
            set
            {
                if (_buttonImage.sprite != value)
                {
                    _buttonImage.sprite = value;
                    Log.WhatHappend("버튼 아이콘 변경됨");
                }
            }
        }

        public event Action<EDGearType> OnClicked;

        // Unity Inspector에서 이 함수 연결
        public void HandleClick() => OnClicked?.Invoke(_gearType);

    }
}