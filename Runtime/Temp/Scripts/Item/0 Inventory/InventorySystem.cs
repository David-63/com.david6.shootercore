using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace David6.ShooterFramework
{
    public class InventorySystem : MonoBehaviour
    {
        public List<InventorySlot> Slots = new List<InventorySlot>();
        public int MaxSlot = 20;
        public PlayerInventorySystem Player;

        public int AddItem(ItemDataSO itemData, int quantity)
        {
            if (quantity <= 0) return 0;
            int remainingItems = quantity;

            if (itemData.MaxStackSize > 1)
            {
                foreach (InventorySlot slot in Slots)
                {
                    if (slot.ItemData == itemData)
                    {
                        int spaceInStack = slot.ItemData.MaxStackSize - slot.Quantity;
                        if (spaceInStack > 0)
                        {
                            int itemsToAdd = Mathf.Min(remainingItems, spaceInStack);
                            slot.Quantity += itemsToAdd;
                            remainingItems -= itemsToAdd;

                            if (remainingItems <= 0)
                            {
                                return 0;
                            }
                        }
                    }
                }
            }

            while (remainingItems > 0 && Slots.Count < MaxSlot)
            {
                int itemsToAdd = Mathf.Min(remainingItems, itemData.MaxStackSize > 0 ? itemData.MaxStackSize : 1);
                Slots.Add(new InventorySlot(itemData, itemsToAdd));
                remainingItems -= itemsToAdd;
            }

            return remainingItems;
        }

        public int RemoveItem(ItemDataSO itemData, int quantity, bool removePartial = true)
        {
            int remainingItems = quantity;
            List<InventorySlot> slotsWithItem = Slots.Where(s => s.ItemData == itemData).ToList();

            int totalAvailableItems = slotsWithItem.Sum(s => s.Quantity);
            if (remainingItems > totalAvailableItems && !removePartial)
            {
                return quantity;
            }

            foreach (InventorySlot slot in slotsWithItem)
            {
                if (remainingItems <= 0)
                    break;

                if (slot.Quantity <= remainingItems)
                {
                    remainingItems -= slot.Quantity;
                    slot.ClearSlot();
                    Slots.Remove(slot);
                }
                else
                {
                    slot.Quantity -= remainingItems;
                    remainingItems = 0;
                }
            }

            return remainingItems;
        }

        public int RemoveItemFromSlot(InventorySlot slot, int quantity)
        {
            if (slot.Quantity >= quantity)
            {
                slot.Quantity -= quantity;
                DropItem(slot.ItemData, quantity);

                if (slot.Quantity == 0)
                {
                    slot.ClearSlot();
                    Slots.Remove(slot);
                }

                return 0;
            }
            else
            {
                int remainingQuantity = quantity - slot.Quantity;

                DropItem(slot.ItemData, slot.Quantity);

                slot.ClearSlot();
                Slots.Remove(slot);

                return remainingQuantity;
            }
        }

        public void DropItem(ItemDataSO itemData, int numberDropped)
        {
            if (numberDropped > 0)
            {
                if (itemData.GroupedPrefab)
                {
                    Instantiate(itemData.Prefab, Player.GetDropPosition(), Quaternion.identity).GetComponent<ItemPickup>().Quantity = numberDropped;
                }
                else
                {
                    for (int idx = 0; idx < numberDropped; ++idx)
                    {
                        Instantiate(itemData.Prefab, Player.GetDropPosition(), Quaternion.identity);
                    }
                }
            }
        }


        public void RemoveItemsFromSlot(int slotNumber)
        {
            Slots.RemoveAt(slotNumber);
        }

        public bool IsFull()
        {
            return Slots.Count >= MaxSlot;
        }

        public void ClearInventory()
        {
            Slots.Clear();
        }
    }
}
