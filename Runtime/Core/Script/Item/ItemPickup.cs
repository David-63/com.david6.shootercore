using UnityEngine;

namespace David6.ShooterFramework
{
    public class ItemPickup : MonoBehaviour
    {
        public ItemDataSO ItemData;
        public int Quantity = 1;

        public void Start()
        {
            gameObject.GetComponent<Collider>().isTrigger = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            PlayerInventorySystem playerInventory = other.GetComponent<PlayerInventorySystem>();
            if (playerInventory == null) return;

            Quantity = playerInventory.PickupItem(ItemData, Quantity);

            if (Quantity <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            ItemPickup other = collision.gameObject.GetComponent<ItemPickup>();
            if (other != null && ItemData.ItemID == other.ItemData.ItemID) return;

            gameObject.GetComponent<Collider>().isTrigger = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}

