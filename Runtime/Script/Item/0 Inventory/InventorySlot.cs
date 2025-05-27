using UnityEngine;

namespace David6.ShooterFramework
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemDataSO ItemData;
        public int Quantity;

        public InventorySlot(ItemDataSO itemData, int quantity)
        {
            ItemData = itemData;
            Quantity = quantity;
        }

        

        public bool IsEmpty()
        {
            return ItemData == null || Quantity <= 0;
        }

        public void ClearSlot()
        {
            ItemData = null;
            Quantity = 0;
        }

        public void SetItem(ItemDataSO newItemData, int quantity)
        {
            ItemData = newItemData;
            Quantity = quantity;
        }
    }

}