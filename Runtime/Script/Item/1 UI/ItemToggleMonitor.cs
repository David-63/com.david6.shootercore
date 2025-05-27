using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterFramework
{
    public class ItemToggleMonitor : MonoBehaviour
    {
        public ToggleGroup ToggleGroupUI;

        private Toggle _lastSelectedToggle;

        void Start()
        {
            foreach (var toggle in ToggleGroupUI.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(isOn => { if (isOn) { HandleToggleChanged(toggle); } });

                if (toggle.isOn)
                {
                    InventoryPanelManager.Instance.FilterItemsByType(toggle.GetComponent<ToggleItemType>().ItemType);
                }
            }
        }

        private void HandleToggleChanged(Toggle selectToggle)
        {
            if (selectToggle != _lastSelectedToggle)
            {
                _lastSelectedToggle = selectToggle;
                eItemType selectedType = _lastSelectedToggle.GetComponent<ToggleItemType>().ItemType;

                InventoryPanelManager.Instance.FilterItemsByType(selectedType);
            }
        }
    }
}
