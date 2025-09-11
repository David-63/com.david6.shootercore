using System.Collections.Generic;
using David6.ShooterCore.Item;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotPresenter : DBaseEquipmentPresenter
    {
        DEquipmentSlotView _panelView;
        Dictionary<EDGearSlot, DEquipmentSlotButton> _buttonMap = new();


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

                if (slotData?.BaseData != null)
                {
                    button.SlotIcon = slotData.BaseData.ItemIcon;
                }

                button.OnClicked += HandleSlotClicked;

                _buttonMap[buttonSlot] = button;
            }
        }

        void HandleSlotClicked(EDGearSlot gearType)
        {
            //선택중인 기어 타입 변경
            _equipmentModel.SetListDisplayGearType(gearType);
            
            _rootPanelController.PushPanel(_rootPanelController.EquipmentFactory.PresenterCache[typeof(DEquipmentListPresenter)]);
        }

        void ChangeSlotIcon(EDGearSlot slotType, DGear slotData)
        {
            if (_buttonMap.TryGetValue(slotType, out var button))
            {
                button.SlotIcon = slotData.BaseData.ItemIcon;
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