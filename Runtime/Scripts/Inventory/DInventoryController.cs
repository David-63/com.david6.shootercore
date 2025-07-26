using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.InventorySystem
{
    public class DInventoryController : MonoBehaviour, IDInventoryControllerProvider
    {
        IDInventoryViewProvider _inventoryView;
        private DInventoryModel _inventoryModel = new();

        public bool SetViewProvider(IDInventoryViewProvider viewProvider)
        {
            bool flag = true;
            if (viewProvider != null)
            {
                _inventoryView = viewProvider;
            }
            else
            {
                flag = false;
            }
            return flag;
        }


        // void Awake()
        // {
        //     //_inventoryModel.OnInventoryChanged += RefreshRender;
        // }

        public void HandlePause()
        {
            Log.WhatHappend("Controller Pause active");
            _inventoryView?.ShowPanel();
        }
        public void HandleResume() => _inventoryView?.HidePanel();

        public void AddItem()
        {
            //_inventoryModel.AddItem();
        }
        public void RemoveItem()
        {
            //_inventoryModel.RemoveItem();
        }
        // private void RefreshRender()
        // {
        //     _inventoryView.RenderItems();
        // }
    }
}