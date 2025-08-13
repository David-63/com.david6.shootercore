using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Provider;

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

        void SetGearType()
        {
            EDGearType currentType = _equipmentModel.GetListDisplayGearType();
            DEquipmentScrollView scrollview = _panelView.GetScrollView();
            scrollview.SetItems(_equipmentModel.GetItems(currentType), EquipGear);
            scrollview.SetScrollViewText(currentType);
        }

        public void EquipGear(DGearData data)
        {
            _equipmentModel.EquipGear(_panelView.GetScrollView().CurrentType, data);
        }

        public override void ShowPanel()
        {
            _panelView.ShowPanel();

            SetGearType();
        }
        public override void HidePanel()
        {
            _panelView.HidePanel();
        }
    }
}