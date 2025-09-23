using System;
using System.Collections.Generic;
using David6.ShooterCore.Item;
using David6.ShooterCore.Tools;
using TMPro;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentScrollView : MonoBehaviour
    {
        [SerializeField] Transform _contentRoot;
        [SerializeField] GameObject ItemButtonPrefab;
        [SerializeField] TMP_Text _scrollViewType;
        List<DItemButton> _itemButtons = new();

        public EDGearSlot CurrentType { get; set; }

        public List<DItemButton> GetItemButtons() => _itemButtons;

        public void SetScrollViewText(EDGearSlot type)
        {
            CurrentType = type;
            _scrollViewType.text = type.ToString();
        }

        public void DisplayItemList(List<DGear> items, Action<DGear> onClick)
        {
            foreach (Transform child in _contentRoot)
            {
                // 나중에 오브젝트 풀링으로 변경
                Destroy(child.gameObject);
            }
            _itemButtons.Clear();

            foreach (DGear item in items)
            {
                var itemObject = Instantiate(ItemButtonPrefab, _contentRoot);
                var button = itemObject.GetComponent<DItemButton>();

                button.Gear = item;
                button.ItemIcon = item.BaseData.ItemIcon;
                button.ItemName.text = item.BaseData.ItemName;
                button.OnItemSelected += onClick;

                _itemButtons.Add(button);
            }
        }
    }
}