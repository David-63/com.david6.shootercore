using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterCore.UI
{
    public class DRootPanelView : DBasePanelView
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