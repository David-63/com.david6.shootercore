
namespace David6.ShooterCore.UI.Focus
{
    public class DFocusView : DBasePanelView
    {

        public override void ShowPanel()
        {
            _canvasGroup.alpha = 1.0f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            //_layoutElement.ignoreLayout = false;
        }
        public override void HidePanel()
        {
            _canvasGroup.alpha = 0.0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            //_layoutElement.ignoreLayout = true;
        }
    }
}