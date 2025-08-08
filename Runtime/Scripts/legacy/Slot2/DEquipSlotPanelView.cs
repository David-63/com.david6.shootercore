using System;
using System.Collections.Generic;
using David6.ShooterCore.Item.Gear;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipSlotPanelView2 : MonoBehaviour
    {
        [SerializeField] List<DEquipSlotButtonView2> _equipSlotButtons;

        public IReadOnlyList<DEquipSlotButtonView2> SlotButtons => _equipSlotButtons;

        public void RegisterButton(Action<EDGearType> handler)
        {
            foreach (var slotButton in _equipSlotButtons)
            {
                slotButton.OnClicked += handler;
            }
        }

    }
}