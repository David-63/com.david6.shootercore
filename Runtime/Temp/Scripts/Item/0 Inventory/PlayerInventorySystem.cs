using UnityEngine;

namespace David6.ShooterFramework
{
    public class PlayerInventorySystem : MonoBehaviour
    {
        public InventorySystem Inventory;
        public ItemDataSO itemToDrop;

        [SerializeField] private InventoryPanelManager InventoryPanelUI;

        private void Start()
        {
            ToggleInventoryPanelUI(false);
            Inventory.Player = this;
        }

        public void ToggleInventoryPanelUI(bool enable)
        {
            InventoryPanelUI.SetPanelVisibility(enable);
        }

        public int PickupItem(ItemDataSO itemData, int quantity)
        {
            if (!Inventory.IsFull() || itemData.MaxStackSize > 0)
            {
                return Inventory.AddItem(itemData, quantity);
            }

            return quantity;
        }

        public void DropItemFromSlot(int slotNumber)
        {
            Inventory.RemoveItemsFromSlot(slotNumber);
        }

        public void DropItem(ItemDataSO itemData, int quantity)
        {
            int couldntBeDropped = Inventory.RemoveItem(itemData, quantity);
            int numberDropped = quantity - couldntBeDropped;

            if (numberDropped > 0)
            {
                if (itemData.GroupedPrefab)
                {
                    Instantiate(itemData.Prefab, GetDropPosition(), Quaternion.identity).GetComponent<ItemPickup>().Quantity = numberDropped;
                }
                else
                {
                    Vector3 location = GetDropPosition();
                    for (int idx = 0; idx < numberDropped; ++idx)
                    {
                        Instantiate(itemData.Prefab, GetDropPosition(), Quaternion.identity);
                    }
                }
            }
        }

        public Vector3 GetDropPosition()
        {
            Vector3 playerPosition = transform.position;
            Vector3 forwardDirection = transform.forward;
            Vector3 upDirection = transform.up;

            return playerPosition + forwardDirection * 2f + upDirection;
        }
    }
}
