using System.Collections.Generic;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotView : DBasePanelView
    {
        [SerializeField] List<DEquipmentSlotButton> _equipSlotButtons;
        public IReadOnlyList<DEquipmentSlotButton> SlotButtons => _equipSlotButtons;

        // public void SlotUpdate(EDGearType currentType, List<DGearData> items, Action<EDGearType, DGearData> onClick)
        // {
        //     _scrollView.SetScrollViewText(currentType);
        //     _scrollView.SetItems(items, onClick);
        // }

        public override void ShowPanel()
        {
            _canvasGroup.alpha = 1.0f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _layoutElement.ignoreLayout = false;
        }
        public override void HidePanel()
        {
            _canvasGroup.alpha = 0.0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _layoutElement.ignoreLayout = true;
        }
    }
}