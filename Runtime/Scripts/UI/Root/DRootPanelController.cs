using System;
using System.Collections.Generic;
using David6.ShooterCore.Context;
using David6.ShooterCore.Data.Inventory;
using David6.ShooterCore.Item;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using David6.ShooterCore.UI.Equipment;
using UnityEngine;

namespace David6.ShooterCore.UI
{
    public class DRootPanelController : MonoBehaviour, IDRootPanelControllerProvider
    {
        [SerializeField] DRootPanelView _rootPanelView;
        [SerializeField] DEquipmentSlotView _slotPanelView;
        [SerializeField] DEquipmentListView _listPanelView;

        DEquipmentModel _equipmentModel;
        DEquipmentFactory _equipmentFactory;
        public DEquipmentFactory EquipmentFactory => _equipmentFactory;

        [SerializeField] DEquipmentDatabase _equipmentDatabase;

        void Awake()
        {
            var root = GetComponent<DPlayerBootstrapper>();
            root.Register<IDRootPanelControllerProvider>(this);

            _equipmentModel = new DEquipmentModel();

            if (_equipmentDatabase != null)
            {
                _equipmentModel.Initialize(_equipmentDatabase.EquipmentItems);
            }

            _equipmentFactory = new DEquipmentFactory();
            _equipmentFactory.Initialize(this, _equipmentModel, _slotPanelView, _listPanelView);
        }

        void Start()
        {
            //_equipmentFactory.EquipmentModel.AddItem(EDGearType.PrimaryWeapon, SampleItem);
        }

        #region UI Control
        Stack<IDPanelPresenterProvider> _panelStack = new();
        IDPanelPresenterProvider _currentPanel;
        public event Action OnCloseUI;
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

        public void HandleResume()
        {
            ClearAllPanel();
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
        void ClearAllPanel()
        {
            while (_panelStack.Count > 0)
            {
                var panel = _panelStack.Pop();
                panel?.HidePanel();
            }
            _currentPanel = null;
            _rootPanelView?.HidePanel();
            OnCloseUI?.Invoke();
        }
        #endregion

        public void RegisterOnEquip(Action<EDGearSlot, DGear> callback)
        {
            _equipmentModel.OnGearChanged += callback;
        }
        
    }
}