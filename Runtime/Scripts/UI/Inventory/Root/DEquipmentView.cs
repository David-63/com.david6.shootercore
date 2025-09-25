using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentView : DBasePanelView
    {
        protected override void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                Log.AttentionPlease("CanvasGroup component is missing.");
            }
            else
            {
                _canvasGroup.alpha = 0f; // 초기 상태는 숨김
            }
        }
    }
}