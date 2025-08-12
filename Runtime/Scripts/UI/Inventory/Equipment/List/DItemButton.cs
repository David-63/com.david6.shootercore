using System;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Item.Gear;
using David6.ShooterCore.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterCore.UI.Equipment
{
    public class DItemButton : MonoBehaviour
    {
        DGearData _gearData;
        public DGearData GearData { get => _gearData; set => _gearData = value; }

        [SerializeField] TMP_Text _itemName;
        public TMP_Text ItemName { get => _itemName; set => _itemName = value; }
        [SerializeField] Image _gearImage;
        public Sprite ItemIcon
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
        // 버튼에서 타입을 반환 안해도 되긴하는데
        public event Action<DGearData> OnClicked;

        // Unity Inspector에서 이 함수 연결
        public void HandleClick()
        {
            Log.WhatHappend("ItemClicked");
            OnClicked?.Invoke(GearData);
        }

    }
}