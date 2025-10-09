using David6.ShooterCore.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterCore.UI.Focus
{
    public class DCrossHairView : DBasePanelView
    {
        [SerializeField] Image _crosshair;

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

        public void SetCrosshairSize(float pixelSize)
        {
            _crosshair.rectTransform.sizeDelta = Vector2.one * pixelSize;
        }

    }
}