using David6.ShooterCore.Provider;
using UnityEngine;

namespace David6.ShooterCore.UI
{
    public class DBasePanelView : MonoBehaviour, IDPanelViewProvider
    {
        protected CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component is missing on DEquipmentSlotView.");
            }
            else
            {
                _canvasGroup.alpha = 0f; // 초기 상태는 숨김
            }
        }

        public virtual void ShowPanel() => _canvasGroup.alpha = 1f;
        public virtual void HidePanel() => _canvasGroup.alpha = 0f;
    }
}