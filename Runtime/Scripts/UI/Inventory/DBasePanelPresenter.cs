using David6.ShooterCore.Provider;
using David6.ShooterCore.UI.Equipment;

namespace David6.ShooterCore.UI
{
    public abstract class DBaseEquipmentPresenter : IDPanelPresenterProvider
    {
        protected IDRootPanelControllerProvider _rootPanelController;
        protected DEquipmentModel _equipmentModel;


        public DBaseEquipmentPresenter(IDRootPanelControllerProvider rootPanelController, DEquipmentModel equipmentModel)
        {
            _rootPanelController = rootPanelController;
            _equipmentModel = equipmentModel;
        }

        public abstract void Initialize();
        public abstract void ShowPanel();
        public abstract void HidePanel();


        
        protected IDPanelViewProvider GetPanelView<T>()
        {
            return _rootPanelController.EquipmentFactory.ViewCache[typeof(T)];
        }

    }
}