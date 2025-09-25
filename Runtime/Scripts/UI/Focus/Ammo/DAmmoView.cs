using TMPro;
using UnityEngine;

namespace David6.ShooterCore.UI.Focus
{
    public class DAmmoView : DBasePanelView
    {
        [SerializeField] TMP_Text _current;
        [SerializeField] TMP_Text _chamber;
        [SerializeField] TMP_Text _storage;

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