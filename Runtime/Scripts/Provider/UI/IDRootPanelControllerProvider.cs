using System;
using David6.ShooterCore.UI.Equipment;

namespace David6.ShooterCore.Provider
{
    public interface IDRootPanelControllerProvider
    {
        event Action OnCloseUI;
        DEquipmentFactory EquipmentFactory { get; }
        void HandlePause();
        void HandleResume();
        void HandlePop();
        void PushPanel(IDPanelPresenterProvider panel);
        void PopPanel();
    }
}