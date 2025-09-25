using System;
using David6.ShooterCore.Item;
using David6.ShooterCore.UI.Equipment;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDEquipmentUIControllerProvider : IDProvider
    {
        event Action OnCloseUI;
        DEquipmentFactory EquipmentFactory { get; }
        void HandlePause();
        void HandleResume();
        void HandleCancel();
        void HandleSubmitPress();
        void HandleSubmitRelease();
        void HandleNavigate(Vector2 direction);

        void PushPanel(IDPanelPresenterProvider panel);
        void PopPanel();



        void RegisterOnGearChanged(Action<DGear> callback);
    }
}