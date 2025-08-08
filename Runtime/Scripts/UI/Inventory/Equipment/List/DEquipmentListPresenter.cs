using David6.ShooterCore.Provider;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentListPresenter : DBaseEquipmentPresenter
    {

        public DEquipmentListPresenter(IDRootPanelControllerProvider rootPanelController, DEquipmentModel equipmentModel)
        : base(rootPanelController, equipmentModel) { }

        public override void Initialize()
        {
            
        }

        public override void ShowPanel()
        {
            DEquipmentListView panelView = GetPanelView<DEquipmentListView>() as DEquipmentListView;
            panelView.ShowPanel();
        }
        public override void HidePanel()
        {
            DEquipmentListView panelView = GetPanelView<DEquipmentListView>() as DEquipmentListView;
            panelView.HidePanel();
        }
    }
}