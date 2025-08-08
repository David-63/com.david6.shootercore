using David6.ShooterCore.Item.Gear;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotPresenter : DBaseEquipmentPresenter
    {

        public DEquipmentSlotPresenter(IDRootPanelControllerProvider rootPanelController, DEquipmentModel equipmentModel)
        : base(rootPanelController, equipmentModel) { }

        public override void Initialize()
        {
            DEquipmentSlotView slotView = _rootPanelController.EquipmentFactory.ViewCache[typeof(DEquipmentSlotView)] as DEquipmentSlotView;
            if (slotView == null)
            {
                Log.WhatHappend("DEquipmentSlotView is not found in ViewCache.");
                return;
            }


            Log.WhatHappend("슬롯 패널 초기화");

            foreach (var button in slotView.SlotButtons)
            {
                var buttonSlot = button.GearType;
                var slotData = _equipmentModel.GetEquippedGear(buttonSlot);
                if (slotData != null)
                {
                    button.SlotIcon = slotData.GearIcon;
                }

                Log.WhatHappend("클릭 이벤트 바인딩");
                button.OnClicked += HandleSlotClicked;
            }
        }

        private void HandleSlotClicked(EDGearType gearType)
        {
            Log.WhatHappend("HandleClick");
            var gearData = _equipmentModel.GetEquippedGear(gearType);
            if (gearData != null)
            {
                _rootPanelController.PushPanel(_rootPanelController.EquipmentFactory.PresenterCache[typeof(DEquipmentListPresenter)]);
            }
        }

        public override void ShowPanel()
        {
            DEquipmentSlotView panelView = GetPanelView<DEquipmentSlotView>() as DEquipmentSlotView;
            panelView.ShowPanel();
        }
        public override void HidePanel()
        {
            DEquipmentSlotView panelView = GetPanelView<DEquipmentSlotView>() as DEquipmentSlotView;
            panelView.HidePanel();
        }
    }
}