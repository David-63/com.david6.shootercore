using UnityEngine;

namespace David6.ShooterFramework
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemDataSO ItemData;
        [SerializeField] private int _quantity;
        private ItemButtonSetting _itemButton;


        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value < 0 ? 0 : value;
                if (_itemButton != null)
                {
                    _itemButton.UpdateQuantityDisplay(_quantity);
                }
            }
        }

        public InventorySlot(ItemDataSO itemData, int quantity)
        {
            ItemData = itemData;
            Quantity = quantity;
            _itemButton = InventoryPanelManager.Instance.CreateInventoryButton(ItemData, this, quantity);
        }

        

        public bool IsEmpty()
        {
            return ItemData == null || Quantity <= 0;
        }

        public void ClearSlot()
        {
            ItemData = null;
            Quantity = 0;
            if (_itemButton != null)
            {
                InventoryPanelManager.Instance.DestroyInventoryButton(_itemButton.gameObject);
            }
        }

        public void SetItem(ItemDataSO newItemData, int quantity)
        {
            ItemData = newItemData;
            Quantity = quantity;
        }
    }

}