using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace David6.ShooterFramework
{
    public class ItemButtonSetting : MonoBehaviour
    {
        [SerializeField] private Image SpriteImage;
        [SerializeField] private TextMeshProUGUI NumberInSlot;
        [SerializeField] private eItemType ItemType;

        public void Init(ItemDataSO data, int itemCount)
        {
            SpriteImage.sprite = data.ItemIcon;
            NumberInSlot.text = itemCount + "";
            ItemType = data.ItemType;
        }
    }
}
