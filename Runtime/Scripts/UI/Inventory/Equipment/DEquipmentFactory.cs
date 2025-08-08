using System;
using System.Collections.Generic;
using David6.ShooterCore.Provider;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentFactory
    {
        IDRootPanelControllerProvider _rootPanelController;
        DEquipmentModel _equipmentModel;

        public Dictionary<Type, IDPanelViewProvider> ViewCache = new();
        public Dictionary<Type, IDPanelPresenterProvider> PresenterCache = new();

        public void Initialize(IDRootPanelControllerProvider rootPanelControllerProvider, DEquipmentModel equipmentModel, DEquipmentSlotView slotPanelView, DEquipmentListView listPanelView)
        {
            _rootPanelController = rootPanelControllerProvider;
            _equipmentModel = equipmentModel;
            ViewCache.Add(typeof(DEquipmentSlotView), slotPanelView);
            ViewCache.Add(typeof(DEquipmentListView), listPanelView);


            DEquipmentSlotPresenter slotPresenter = new DEquipmentSlotPresenter(_rootPanelController, _equipmentModel);
            PresenterCache.Add(typeof(DEquipmentSlotPresenter), slotPresenter);
            DEquipmentListPresenter listPresenter = new DEquipmentListPresenter(_rootPanelController, _equipmentModel);
            PresenterCache.Add(typeof(DEquipmentListPresenter), listPresenter);

            ClearPresenter();
        }


        void ClearPresenter()
        {
            PresenterCache[typeof(DEquipmentSlotPresenter)].Initialize();
            PresenterCache[typeof(DEquipmentListPresenter)].Initialize();
        }
    }
}