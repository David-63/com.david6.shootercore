using David6.ShooterCore.Item;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine.EventSystems;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentListPresenter : DBaseEquipmentPresenter
    {
        DEquipmentListView _panelView;

        public DEquipmentListPresenter(IDRootPanelControllerProvider rootPanelController, DEquipmentModel equipmentModel)
        : base(rootPanelController, equipmentModel) { }

        public override void Initialize()
        {
            _panelView = GetPanelView<DEquipmentListView>() as DEquipmentListView;
        }

        void RefreshScrollView()
        {
            EDGearSlot currentType = _equipmentModel.GetListDisplayGearType();
            DEquipmentScrollView scrollview = _panelView.GetScrollView();
            scrollview.DisplayItemList(_equipmentModel.GetItems(currentType), OnItemButtonSelected);
            scrollview.SetScrollViewText(currentType);

            var buttons = scrollview.GetItemButtons();
            if (buttons.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
            }
            else if (buttons.Count == 0)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public void OnItemButtonSelected(DGear data)
        {
            _equipmentModel.SetEquippedGear(data);
        }

        public override void ShowPanel()
        {
            _panelView.ShowPanel();

            RefreshScrollView();
        }
        public override void HidePanel()
        {
            _panelView.HidePanel();
        }

        public override void OnSubmit()
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null) return;

            if (selected.TryGetComponent<DItemButton>(out var button))
            {
                button.HandleSelectItem();
            }
        }
    }
}