using System.Collections.Generic;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotView : DBasePanelView
    {
        [SerializeField] List<DEquipmentSlotButton> _equipSlotButtons;
        public IReadOnlyList<DEquipmentSlotButton> SlotButtons => _equipSlotButtons;

        protected override void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component is missing on DEquipmentSlotView.");
            }

            gameObject.SetActive(false);
        }

        public override void ShowPanel()
        {
            gameObject.SetActive(true);
        }
        public override void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}