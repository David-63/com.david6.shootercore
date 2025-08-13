using System.Collections.Generic;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotPresenter : DBaseEquipmentPresenter
    {
        DEquipmentSlotView _panelView;
        Dictionary<EDGearType, DEquipmentSlotButton> _buttonMap = new();


        public DEquipmentSlotPresenter(IDRootPanelControllerProvider rootPanelController, DEquipmentModel equipmentModel)
        : base(rootPanelController, equipmentModel) { }

        public override void Initialize()
        {
            _panelView = GetPanelView<DEquipmentSlotView>() as DEquipmentSlotView;
            if (_panelView == null)
            {
                Log.AttentionPlease("DEquipmentSlotView is not found in ViewCache.");
                return;
            }

            _equipmentModel.OnGearChanged += ChangeSlotIcon;

            foreach (var button in _panelView.SlotButtons)
            {
                var buttonSlot = button.GearType;
                var slotData = _equipmentModel.GetEquippedGear(buttonSlot);
                if (slotData != null)
                {
                    button.SlotIcon = slotData.GearIcon;
                }

                button.OnClicked += HandleSlotClicked;

                _buttonMap[buttonSlot] = button;
            }
        }

        void HandleSlotClicked(EDGearType gearType)
        {
            //선택중인 기어 타입 변경
            _equipmentModel.SetListDisplayGearType(gearType);
            
            _rootPanelController.PushPanel(_rootPanelController.EquipmentFactory.PresenterCache[typeof(DEquipmentListPresenter)]);
        }

        void ChangeSlotIcon(EDGearType slotType, DGearData slotData)
        {
            if (_buttonMap.TryGetValue(slotType, out var button))
            {
                Log.WhatHappend("이미지 변경 성공");
                button.SlotIcon = slotData.GearIcon;
            }
        }

        public override void ShowPanel()
        {
            _panelView.ShowPanel();
        }
        public override void HidePanel()
        {
            _panelView.HidePanel();
        }
    }
}