using System.Collections.Generic;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.InventorySystem
{
    public class DInventoryView : MonoBehaviour, IDInventoryViewProvider
    {
        [SerializeField] GameObject InventoryPanel;
        [SerializeField] List<GameObject> SlotViews;

        public void ShowPanel()
        {
            Log.WhatHappend("View Panel Active");
            InventoryPanel.SetActive(true);
        }
        public void HidePanel() => InventoryPanel.SetActive(false);

        public void RenderItems()
        {
            for (int idx = 0; idx < SlotViews.Count; ++idx)
            {
                //SlotViews[idx].RenderSlot();
            }
        }
    }
}