using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentListView : DBasePanelView
    {
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