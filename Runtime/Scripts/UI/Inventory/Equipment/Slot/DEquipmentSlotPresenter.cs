using System.Collections.Generic;
using David6.ShooterCore.Item;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine.EventSystems;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotPresenter : DBaseEquipmentPresenter
    {
        DEquipmentSlotView _panelView;
        Dictionary<EDGearSlot, DSlotButton> _buttonMap = new();


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

            _equipmentModel.OnGearChanged += RefrashSlotIcon;

            foreach (var button in _panelView.SlotButtons)
            {
                var buttonSlot = button.GearType;
                var slotData = _equipmentModel.GetEquippedGear(buttonSlot);

                if (slotData?.BaseData != null)
                {
                    button.SlotIcon = slotData.BaseData.ItemIcon;
                }

                button.OnSlotSelected += OnSlotButtonClicked;

                _buttonMap[buttonSlot] = button;
            }
        }

        void OnSlotButtonClicked(EDGearSlot gearType)
        {
            _equipmentModel.SetListDisplayGearType(gearType);
            _rootPanelController.PushPanel(_rootPanelController.EquipmentFactory.PresenterCache[typeof(DEquipmentListPresenter)]);
        }

        void RefrashSlotIcon(DGear gear)
        {
            var gearData = gear.GearSlot;
            if (_buttonMap.TryGetValue(gearData, out var button))
            {
                button.SlotIcon = gear.BaseData.ItemIcon;
            }
        }

        public override void ShowPanel()
        {
            _panelView.ShowPanel();
            EventSystem.current.SetSelectedGameObject(_buttonMap[EDGearSlot.Primary].gameObject);
        }
        public override void HidePanel()
        {
            _panelView.HidePanel();
        }
        public override void OnSubmit()
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null) return;

            if (selected.TryGetComponent<DSlotButton>(out var button))
            {
                button.HandleClick();
            }
        }
    }
}