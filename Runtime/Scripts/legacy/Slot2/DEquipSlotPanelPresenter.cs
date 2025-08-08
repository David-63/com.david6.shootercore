using System.Collections.Generic;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Item.Gear;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipSlotPanelPresenter2
    {
        CanvasGroup _slotPanelGroup;

        readonly IDRootPanelControllerProvider _rootPanelController;
        readonly DEquipmentSlotModel2 _equipmentSlotModel;
        readonly DEquipmentModel _equipmentModel;
        readonly DEquipSlotPanelView2 _view;

        Dictionary<EDGearType, DEquipSlotButtonView2> _buttonMap = new();

        public DEquipSlotPanelPresenter2(IDRootPanelControllerProvider rootPanelController, DEquipmentSlotModel2 equipmentModel, DEquipSlotPanelView2 view)
        {
            _rootPanelController = rootPanelController;
            _equipmentSlotModel = equipmentModel;
            _view = view;
        }
        public DEquipSlotPanelPresenter2(IDRootPanelControllerProvider rootPanelController, DEquipmentModel equipmentModel, DEquipSlotPanelView2 view)
        {
            _rootPanelController = rootPanelController;
            _equipmentModel = equipmentModel;
            _view = view;
        }
        public void Dispose()
        {
            _equipmentModel.OnGearChanged -= HandleGearChanged;

            foreach (DEquipSlotButtonView2 button in _view.SlotButtons)
            {
                button.OnClicked -= HandleSlotClicked;
            }
        }

        public void Initialize()
        {
            foreach (DEquipSlotButtonView2 button in _view.SlotButtons)
            {
                var buttonSlot = button.GearType;
                var slotData = _equipmentModel.GetEquippedGear(buttonSlot);
                button.SlotIcon = slotData.GearIcon;
                button.OnClicked += HandleSlotClicked;

                _buttonMap[buttonSlot] = button;
            }

            _equipmentModel.OnGearChanged += HandleGearChanged;
        }

        void HandleGearChanged(EDGearType gearType, DGearData gearData)
        {
            if (_buttonMap.TryGetValue(gearType, out var button))
            {
                button.SlotIcon = gearData.GearIcon;
            }
        }

        void HandleSlotClicked(EDGearType gearType)
        {
            //_rootPanelController.PushPanel();
            //_equiplistPanelPresenter.SetFilter(gearType);
        }
    }
}