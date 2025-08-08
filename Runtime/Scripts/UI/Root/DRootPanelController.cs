using System;
using System.Collections.Generic;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using David6.ShooterCore.UI.Equipment;
using UnityEngine;

namespace David6.ShooterCore.UI
{
    public class DRootPanelController : MonoBehaviour, IDRootPanelControllerProvider
    {
        [SerializeField] DRootPanelView _rootPanelView;
        DEquipmentModel _equipmentModel;
        DEquipmentFactory _equipmentFactory;

        [SerializeField] DEquipmentSlotView _slotPanelView;
        [SerializeField] DEquipmentListView _listPanelView;


        Stack<IDPanelPresenterProvider> _panelStack = new();
        IDPanelPresenterProvider _currentPanel;
        public event Action OnCloseUI;

        public DEquipmentFactory EquipmentFactory => _equipmentFactory;


        private void Awake()
        {
            _equipmentModel = new DEquipmentModel();
            _equipmentFactory = new DEquipmentFactory();
            _equipmentFactory.Initialize(this, _equipmentModel, _slotPanelView, _listPanelView);
        }

        public void HandlePop()
        {
            PopPanel();
        }

        public void HandlePause()
        {
            _rootPanelView?.ShowPanel();
            if (_panelStack.Count == 0 || _currentPanel == null)
            {
                PushPanel(_equipmentFactory.PresenterCache[typeof(DEquipmentSlotPresenter)]);
            }
        }

        // tab 입력을 받을 경우
        public void HandleResume()
        {
            _rootPanelView?.HidePanel();
            OnCloseUI?.Invoke();
        }

        public void PushPanel(IDPanelPresenterProvider panel)
        {
            _currentPanel?.HidePanel();
            _panelStack.Push(panel);
            _currentPanel = panel;
            _currentPanel.ShowPanel();
        }

        public void PopPanel()
        {
            if (_panelStack.Count <= 1)
            {
                ClearPanel();
                return;
            }
            _currentPanel.HidePanel();
            _panelStack.Pop();
            _currentPanel = _panelStack.Peek();
            _currentPanel.ShowPanel();
        }

        void ClearPanel()
        {
            _currentPanel?.HidePanel();
            _rootPanelView?.HidePanel();
            _panelStack.Clear();
            _currentPanel = null;
            OnCloseUI?.Invoke();
        }
    }
}