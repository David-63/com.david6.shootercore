using UnityEngine;

namespace David6.ShooterFramework
{
    // 안씀
    public class InventoryManager : MonoBehaviour
    {
        public InventorySystem Inventory;
        public ItemDataSO Item;
        public ItemDataSO Item2;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Inventory.AddItem(Item, 5);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Inventory.AddItem(Item2, 5);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                Inventory.RemoveItem(Item2, 1);
            }
        }
    }
}
