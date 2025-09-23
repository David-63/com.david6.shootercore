using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterCore.UI
{
    public class DBasePanelView : MonoBehaviour, IDPanelViewProvider
    {
        protected CanvasGroup _canvasGroup;
        protected LayoutElement _layoutElement;

        protected virtual void Awake()
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
            _layoutElement = GetComponent<LayoutElement>();
            if (_layoutElement == null)
            {
                Log.AttentionPlease("layout component is missing");
            }
            else
            {
                _layoutElement.ignoreLayout = true;
            }
        }

        public virtual void ShowPanel() => _canvasGroup.alpha = 1f;
        public virtual void HidePanel() => _canvasGroup.alpha = 0f;
    }
}