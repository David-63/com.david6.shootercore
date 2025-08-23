using System;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.UI.Equipment;

namespace David6.ShooterCore.Provider
{
    public interface IDRootPanelControllerProvider : IDProvider
    {
        event Action OnCloseUI;
        DEquipmentFactory EquipmentFactory { get; }
        void HandlePause();
        void HandleResume();
        void HandlePop();
        void PushPanel(IDPanelPresenterProvider panel);
        void PopPanel();

        void RegisterOnEquip(Action<EDGearType, DGearData> callback);
    }
}