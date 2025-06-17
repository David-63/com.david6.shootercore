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
            Log.WhatHappend("아이탬 초기화");
            SpriteImage.sprite = data.ItemIcon;
            NumberInSlot.text = itemCount + "";
            ItemType = data.ItemType;
        }

        public void UpdateQuantityDisplay(int quantity)
        {
            Log.WhatHappend("아이템 텍스트 갱신");
            NumberInSlot.text = quantity + "";
        }
    }
}
